using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the Authors in the book store's database
    /// Controllers talk to the DTO objects
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)] //for documentation
    public class AuthorsController : ControllerBase
    {

        //dependency injection
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, 
            ILoggerService loggerService, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = loggerService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All Authors
        /// need an endpoint for getting all books
        /// ActionResult comes from MVC
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuthors()
        {
            // This controller talks to the data transfer object
            // Automapper will transfer data between the AuthorDTO and the Author data class
            // The repository (injected into this class)
            //      calls the data object which then gets mapped to the DTO
            // And finally return the DTO to whoever is calling the API

            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call");
                //get authors
                var authors = await _authorRepository.FindAll();
                //do the mapping of authors coming back from the database to a list of AuthorDTO's
                //The DTO allows for control of what type of data is released
                //The DTO come from the caller of the API and may not require all the fields in the
                //database for any given table - it may only ask for the author name and isbn number
                //only these would be sent from the this method
                var response = _mapper.Map<IList<AuthorDTO>>(source: authors);

                //return response payload
                _logger.LogInfo($"{location}: Successful");
                return Ok(response);
            }
            
            catch (Exception e)
            {
                return InternalError(location + e.Message + e.InnerException);
            }
        }

        /// <summary>
        /// Get an author by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An Author's record</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            // This controller talks to the data transfer object
            // Automapper will transfer data between the AuthorDTO and the Author data class
            // The repository (injected into this class)
            //      calls the data object which then gets mapped to the DTO
            // And finally return the DTO to whoever is calling the API

            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted call for id: {id}");

                //get author
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with id {id}");
                    return NotFound();

                }

                var response = _mapper.Map<AuthorDTO>(source: author);

                //return response payload
                _logger.LogInfo($"{location}: Successfully got record with id: {id}");
                return Ok(response);
            }
            
            catch (Exception e)
            {
                return InternalError(location + e.Message + e.InnerException);
            }
        }

        /// <summary>
        /// Creates an Author
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            //Getting the DTO from the client with the data
            //USe the mapper to take this data and map it to the underlying Author class
            //(which is the superset dataclass and contains more fields)
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Create attempted");
                if(authorDTO == null)
                {
                    _logger.LogWarn($"{location}: Empty request was submitted");
                    return BadRequest(ModelState);
                }
                if(ModelState.IsValid == false)
                {
                    _logger.LogWarn($"{location}: Data was incomplete");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);
                if(isSuccess == false)
                {
                    return InternalError($"{location}: Author creation failed");
                }

                _logger.LogInfo("Author created succsssfully");
                return Created("Create", new { author });

            }
            catch (Exception e)
            {
                return InternalError(location + e.Message + e.InnerException);
            }

        }

        /// <summary>
        /// Updates an author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int Id, [FromBody] AuthorUpdateDTO authorUpdateDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Update attempted on record with Id: {Id}");
                //check for null payload and bad id
                if(Id < 1 || authorUpdateDTO == null || Id != authorUpdateDTO.Id)
                {
                    _logger.LogWarn($"{location}: Update failed with bad data - Id: {Id}");
                    return BadRequest();
                }
                if(ModelState.IsValid == false)
                {
                    _logger.LogWarn($"{location}: Author data was incomplete");
                    return BadRequest(ModelState);
                }

                if (await _authorRepository.IsExists(Id) == false)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with Id: {Id}");
                    return NotFound();
                }

                var author = _mapper.Map<Author>(authorUpdateDTO);
                var isSuccess = await _authorRepository.Update(author);
                if(isSuccess == false)
                {
                    return InternalError($"{location}: Update Failed for record with Id {Id}");
                }
                _logger.LogInfo($"{location}: Author Update attempt: id {Id} was successful");
                return NoContent();

            }
            catch (Exception e)
            {

                return InternalError(location + e.Message + e.InnerException);
            }
        }


        /// <summary>
        /// Updates an author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempting to delete: id {id}");
                if (id < 1)
                {
                    _logger.LogWarn($"{location}:Author Id cannot be < 0");
                    return BadRequest();
                }

                var author = await _authorRepository.FindById(id);
                if(author == null)
                {
                    _logger.LogWarn($"{location}:Failed to find Author {id} in db");
                    return NotFound();
                }

                var isSuccess = await _authorRepository.Delete(author);
                if(isSuccess == false)
                {
                    return InternalError($"{location}: Failed to delete {id}");
                }

                _logger.LogInfo($"{location}:Successfully deleted Author {id}");
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
