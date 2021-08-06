using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Interacts with the Books table
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] //Global authorize if one is neglected below
    public class BooksController : ControllerBase
    {
        //inject dependencies
        private readonly IBookRepository _bookRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, 
            ILoggerService loggerService, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _logger = loggerService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all Books
        /// </summary>
        /// <returns>A List of books</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call");
                var books = await _bookRepository.FindAll();
                //client is getting the DTO object - not a Books object
                var response = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo($"{location}: Successful");
                return Ok(response); 

            }
            catch (Exception e)
            {

                return InternalError(location + e.Message + e.InnerException);
            }
        }

        
        /// <summary>
        /// Gets a Book by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A Book Record</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call for id: {id}");
                var book = await _bookRepository.FindById(id);
                //client is getting the DTO object - not a Books object
                if(book == null)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with id: {id}");
                    return NotFound(); //return 404
                }
                var response = _mapper.Map<BookDTO>(book);
                _logger.LogInfo($"{location}: Successfully got record with Id: {id}");
                return Ok(response); 
            }
            catch (Exception e)
            {

                return InternalError(location + e.Message + e.InnerException);
            }
        }


        /// <summary>
        /// Creates a new Book
        /// </summary>
        /// <param name="BookDTO"></param>
        /// <returns>Book object</returns>
        [Authorize(Roles = "Administrator")] //only an admin can hit this endpoint
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO BookDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Create attempted");
                if(BookDTO == null)
                {
                    _logger.LogWarn($"{location}: Empty request was submitted");
                    return BadRequest();
                }
                if(ModelState.IsValid == false)
                {
                    _logger.LogWarn($"{location}: Data was incomplete");
                    return BadRequest(ModelState);
                }
                //if here got a valid book so insert it
                //source here is a DTO from the db - heading to the client
                var book = _mapper.Map<Book>(BookDTO);
                var isSuccess = await _bookRepository.Create(book);
                if(isSuccess == false)
                {
                    return InternalError("Creation Failed");
                }
                _logger.LogInfo($"{location}: Creation was successful");
                _logger.LogInfo($"{location}: object created: {book}");
                return Created("Create", new { book }); ;


            }
            catch (Exception e)
            {

                return InternalError(location + e.Message + e.InnerException);
            }

        }

        /// <summary>
        /// Update a record in the Book database
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="BookDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")] //only an admin can hit this endpoint
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("Id")]
        public async Task<IActionResult> Update(int Id,[FromBody] BookUpdateDTO BookDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Update attempted on record with Id: {Id}");
                if(Id < 1 || BookDTO == null || Id != BookDTO.Id)
                {
                    _logger.LogWarn($"{location}: Update failed with bad data - Id: {Id}");
                    return BadRequest();
                }
                var isExists = await _bookRepository.IsExists(Id);
                if(isExists != true)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with Id: {Id}");
                    return NotFound();
                }
                //check for the required fields, string length etc
                if(ModelState.IsValid == false)
                {
                    _logger.LogWarn($"{location}: Data was incomplete");
                    return BadRequest();
                }

                var book = _mapper.Map<Book>(BookDTO);
                var isSuccess = await _bookRepository.Update(book);
                if(isSuccess == false)
                {
                    return InternalError($"{location}: Update Failed for record with Id {Id}");
                }
                _logger.LogInfo($"{location}: Record with Id {Id} was successfully updated");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError(location + e.Message + e.InnerException);
            }
        }

        /// <summary>
        /// Delete record with given Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="BookDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")] //only an admin can hit this endpoint
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("Id")]
        public async Task<IActionResult> Delete(int Id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Update attempted on record with Id: {Id}");
                if(Id < 1 )
                {
                    _logger.LogWarn($"{location}: Delete failed with bad data - Id: {Id}");
                    return BadRequest();
                }
                var isExists = await _bookRepository.IsExists(Id);
                if(isExists != true)
                {
                    _logger.LogWarn($"{location}: Failed to delete record with Id: {Id}");
                    return NotFound();
                }
                //check for the required fields, string length etc
                if(ModelState.IsValid == false)
                {
                    _logger.LogWarn($"{location}: Data was incomplete");
                    return BadRequest();
                }

                var book = await _bookRepository.FindById(Id);
                var isSuccess = await _bookRepository.Delete(book);
                if(isSuccess == false)
                {
                    return InternalError($"{location}: Delete Failed for record with Id {Id}");
                }
                _logger.LogInfo($"{location}: Record with Id {Id} was successfully deleted");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError(location + e.Message + e.InnerException);
            }
        }

        /// <summary>
        /// What controller and action is making a given call
        /// Just to improve the log
        /// </summary>
        /// <returns></returns>
        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}";
        }



        /// <summary>
        /// Present consistant error status message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private ObjectResult InternalError(string message)
        {
            //https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
            _logger.LogError(message);
            return StatusCode(500, "Something mad happened here");  //Internal server error code
        }

    }
}
