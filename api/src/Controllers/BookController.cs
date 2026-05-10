using api.src.Data;
using api.src.Models;
using api.src.Models.DTO;
using api.src.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.src.Controllers
{
    [Authorize]
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
                return Ok(ApiResponse<List<BookDTO>>.Ok("Books retrieved successfully", bookDTOs));
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    ApiResponse<object>.Error(500, "Error fetching books", ex.Message));
               
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
                    return BadRequest(ApiResponse<object>.BadRequest("Book ID should be greater than 0"));

                }

                var book = await _service.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Book with ID {id} was not found"));
                   
                }

                var bookDTO = _mapper.Map<BookDTO>(book);

                return Ok(ApiResponse<object>.Ok("Book fetched successfully", bookDTO));
               

            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    ApiResponse<object>.Error(500, "Error retrieving book", ex.Message));
            }
        }

        //create book
        [Authorize(Roles = "Admin")]
        [HttpPost]
        
        
        public async Task<ActionResult<ApiResponse<BookDTO>>> CreateBook(BookCreateDTO bookDTO)
        {
            try
            {
                if (bookDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Book Data is Required"));
                   
                } 

                var book = await _service.CreateBookAsync(bookDTO);

                var bookDTOresult = _mapper.Map<BookDTO>(book);

                return CreatedAtAction(nameof(GetBookById), new { id = book.BookId }, ApiResponse<object>.CreatedAt("Book created successfully", bookDTOresult));
                
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("already exists"))
                {
                    return Conflict(ApiResponse<object>.Conflict("Book already exists"));
                   
                }
                return StatusCode(500,
                    ApiResponse<object>.Error(500, "Failed to create book", ex.Message));
              
            }
        }

        //update the book details
        [Authorize(Roles = "Admin")]
        
        [HttpPut("{id:int}")]


        public async Task<ActionResult<ApiResponse<BookDTO>>> UpdateBook(int id, BookUpdateDTO bookDTO)
        {
            try
            {
                if (bookDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Book data is required"));
                  
                }

                if (id != bookDTO.Id)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Book ID in the URL does not match Book ID in the request body"));
                 
                }

                var updatedBook = await _service.UpdateBookAsync(id, bookDTO);

                if (updatedBook == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Book with ID {id} was not found"));
                  
                }

                var bookDTOresult = _mapper.Map<BookDTO>(updatedBook);

                return Ok(ApiResponse<object>.Ok("Book data is updated successfully", bookDTOresult));
              
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("already exists"))
                {
                    return Conflict(ApiResponse<object>.Conflict("Book with this name already exists"));
                   
                }
                return StatusCode(500,
                    ApiResponse<object>.Error(500, "Failed to update book data", ex.Message));
               
            }
        }

        ////Delete Book
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]

        public async Task<ActionResult<ApiResponse<BookDTO>>> DeleteBook(int id)
        {
            try
            {

                var isDeleted = await _service.DeleteBookAsync(id);

                if (!isDeleted)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Book with ID {id} was not found"));
                }

                return Ok(ApiResponse<object>.NoContent());
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    ApiResponse<object>.Error(500,"Failed to delete book", ex.Message));
            }
        }

    }
}
