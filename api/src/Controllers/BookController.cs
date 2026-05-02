using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.src.Data;
using api.src.Models.DTO;
using api.src.Models;
using api.src.Services;
using AutoMapper;

namespace api.src.Controllers
{
    [ApiController]
    [Route("api/book")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _service;
        private readonly IMapper _mapper;
        public BookController(IBookService service,IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }


        //get books api
        [HttpGet]

        public async Task<ActionResult<ApiResponse<List<BookDTO>>>> GetBook()
        {
            try
            {
                var books = await _service.GetBooksAsync();
                var bookDTOs = _mapper.Map<List<BookDTO>>(books);
                var response = new ApiResponse<List<BookDTO>>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Books fetched successfully",
                    Data = bookDTOs
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Error fetching books",
                    Errors = ex.Message
                });
            }

         }

        //get books by id

        [HttpGet("{id:int}")]

        public async Task<ActionResult<ApiResponse<BookDTO>>> GetBookById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Book ID must be greater than 0"
                    });

                }

                var book = await _service.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = $"Book with ID {id} was not found"
                    });
                }

                var bookDTO = _mapper.Map<BookDTO>(book);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Book fetched successfully",
                    Data = bookDTO
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Error retrieving book",
                    Errors = ex.Message
                });
            }
        }

        //create book
        [HttpPost]

        public async Task<ActionResult<Book>> CreateBook(BookCreateDTO bookDTO)
        {
            try
            {
                if (bookDTO == null)
                {
                    return BadRequest("Book data is required");
                } 

                var book = await _service.CreateBookAsync(bookDTO);

                return CreatedAtAction(nameof(GetBookById), new { id = book.BookId }, book);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("already exists"))
                {
                    return Conflict(ex.Message); // 409
                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An Error occured while creating the book : {ex.Message}");
            }
        }

        //update the book details
        [HttpPut("{id:int}")]

        public async Task<ActionResult<Book>> UpdateBook(int id, BookUpdateDTO bookDTO)
        {
            try
            {
                if (bookDTO == null)
                {
                    return BadRequest("Book data is required");
                }

                if (id != bookDTO.Id)
                {
                    return BadRequest("Book ID in URL does not match Book ID in the request body");
                }

                var updatedBook = await _service.UpdateBookAsync(id, bookDTO);

                if (updatedBook == null)
                {
                    return NotFound($"Book with ID {id} was not found");
                }

                return Ok(updatedBook);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("already exists"))
                {
                    return Conflict(ex.Message); // 409
                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An Error occured while updating the book : {ex.Message}");
            }
        }

        ////Delete Book
        [HttpDelete("{id:int}")]

        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            try
            {

                var isDeleted = await _service.DeleteBookAsync(id);

                if (!isDeleted)
                {
                    return NotFound($"Book with ID {id} was not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An Error occured while deleting the book : {ex.Message}");
            }
        }

    }
}
