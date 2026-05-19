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
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(AppDbContext db, IHttpClientFactory httpClientFactory, IConfiguration configuration,IMapper mapper, ILogger<RecommendationService> logger)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RecommendationResponseDTO> GetRecommendationAsync(RecommendationRequestDTO requestDTO)
        {
            RecommendationResponseDTO responseDTO = new();
            _logger.LogInformation("Recommendation request received. Genre: {Genre}, MinimumRating: {MinimumRating}, Prompt: {Prompt}",
               requestDTO.Genre ?? "Any",
               requestDTO.MinimumRating?.ToString() ?? "None",
               requestDTO.Prompt ?? "None");

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
            _logger.LogInformation("Internal recommendations fetched at {Time}: {Count} books.", DateTime.UtcNow, responseDTO.InternalRecommendations.Count);

            // Groq API call
            try
            {
                var apikey = _configuration["GroqSettings:ApiKey"];

                var prompt = $@"Suggest 5 books.
                Genre: {requestDTO.Genre}
                User Interest: {requestDTO.Prompt}
                Return ONLY in this exact format, one book per line:
                Title | Author
                No numbering, no extra text, no explanations.";

                var requestBody = new
                {
                    model = "llama-3.3-70b-versatile",
                    messages = new[]
                    {
            new { role = "user", content = prompt }
        },
                    max_tokens = 200,
                    temperature = 0.7
                };

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apikey}");

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var responseMessage = await client.PostAsync(
                    "https://api.groq.com/openai/v1/chat/completions",
                    jsonContent);

                responseMessage.EnsureSuccessStatusCode();

                var jsonResponse = await responseMessage.Content.ReadAsStringAsync();

                using JsonDocument document = JsonDocument.Parse(jsonResponse);

                var aiText = document
                    .RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
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
                        var parts = aiBook.Split('|', 2);
                        var title = parts[0].Trim();
                        var author = parts.Length > 1 ? parts[1].Trim() : string.Empty;

                        bool existsInDb = books.Any(b =>
                            b.Title.ToLower() == title.ToLower());

                        if (!existsInDb)
                        {
                            responseDTO.ExternalRecommendations.Add(
                                new RecommendationBookDTO
                                {
                                    Title = title,
                                    Author = author,
                                    IsInternal = false,
                                    SearchUrl =
                                        $"https://www.google.com/search?q={Uri.EscapeDataString(title + " book")}"
                                });
                        }
                    }
                }
                _logger.LogInformation("External recommendations fetched at {Time}: {Count} books.", DateTime.UtcNow, responseDTO.ExternalRecommendations.Count);
            }

            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Groq API call failed at {Time}. Status: {Status}", DateTime.UtcNow, ex.StatusCode);
                throw;
            }

            return responseDTO;
        }
    }
}
