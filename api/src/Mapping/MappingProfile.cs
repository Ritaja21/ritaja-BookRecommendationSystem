using api.src.Models;
using api.src.Models.DTO;
using AutoMapper;

namespace api.src.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<BookCreateDTO, Book>();
            CreateMap<BookUpdateDTO, Book>();
            CreateMap<Book, BookDTO>()
             .ForMember(dest => dest.AverageRating,
                 opt => opt.MapFrom(src =>
                     src.UserBooks.Any(ub => ub.Rating.HasValue)
                         ? Math.Round(
                             src.UserBooks
                                 .Where(ub => ub.Rating.HasValue)
                                 .Average(ub => ub.Rating!.Value),
                             2)
                         : (double?)null
                 ));
            CreateMap<User, UserDTO>();
            CreateMap<UserBook, UserHistoryDTO>()
            .ForMember(dest => dest.Title,
                opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.Author,
                opt => opt.MapFrom(src => src.Book.Author));
        }
    }
}
