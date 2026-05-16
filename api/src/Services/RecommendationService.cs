using api.src.Data;
using api.src.Models.DTO;
using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace api.src.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly AppDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public RecommendationService(AppDbContext db, IHttpClientFactory httpClientFactory, IConfiguration configuration,IMapper mapper)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<RecommendationResponseDTO> GetRecommendationAsync(RecommendationRequestDTO requestDTO)
        {
            RecommendationResponseDTO responseDTO = new();

            //internal database filtering

            var booksQuery = _db.Books.Include(b => b.UserBooks).AsQueryable();

            if (!string.IsNullOrEmpty(requestDTO.Genre))
            {
                booksQuery = booksQuery.Where(b =>
                   b.Genre != null &&
                   b.Genre.ToLower()
                       .Contains(requestDTO.Genre.ToLower()));
            }

            var books = await booksQuery.ToListAsync();

            //filter by average rating

            if (requestDTO.MinimumRating.HasValue)
            {
                books = books.Where(b=>
                {
                    var ratings = b.UserBooks.Where(ub => ub.Rating.HasValue).Select(ub => ub.Rating!.Value);

                    if (!ratings.Any())
                    {
                        return false;
                    }

                    return ratings.Average() >= requestDTO.MinimumRating.Value;
                }).ToList();
            }

            //internal recommendation from database

            responseDTO.InternalRecommendations = _mapper.Map<List<BookDTO>>(books);

            //GEMINI Api call
            try
            {
                var apikey = _configuration["GeminiSettings:ApiKey"];
                var prompt = $@" Suggest 5 books.
                                 Genre: {requestDTO.Genre}
                                 User Interest: {requestDTO.Prompt}
                                 Return ONLY book names in separate lines.";
                var requestBody = new
                {
                    contents = new[]
                   {
                        new
                        {
                            parts = new[]
                            {
                                new
                                {
                                    text = prompt
                                }
                            }
                        }
                    }
                };
                var client = _httpClientFactory.CreateClient();

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var responseMessage = await client.PostAsync(
                   $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apikey}",
                   jsonContent);

                responseMessage.EnsureSuccessStatusCode();

                var jsonResponse = await responseMessage.Content.ReadAsStringAsync();

                using JsonDocument document = JsonDocument.Parse(jsonResponse);

                var aiText = document
                 .RootElement
                 .GetProperty("candidates")[0]
                 .GetProperty("content")
                 .GetProperty("parts")[0]
                 .GetProperty("text")
                 .GetString();

                if (!string.IsNullOrEmpty(aiText))
                {
                    var aiBooks = aiText
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Select(b => b.Trim())
                        .Where(b => !string.IsNullOrEmpty(b))
                        .Distinct()
                        .ToList();

                    foreach (var aiBook in aiBooks)
                    {
                        bool existsInDb = books.Any(b =>
                            b.Title.ToLower() == aiBook.ToLower());

                        if (!existsInDb)
                        {
                            responseDTO.ExternalRecommendations.Add(
                                new RecommendationBookDTO
                                {
                                    Title = aiBook,
                                    IsInternal = false,
                                    SearchUrl =
                                        $"https://www.google.com/search?q={Uri.EscapeDataString(aiBook + " book")}"
                                });
                        }
                    }
                }
            }
            catch
            {
                
            }

            return responseDTO;
        }
    }
}
