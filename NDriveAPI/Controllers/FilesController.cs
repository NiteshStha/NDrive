using Contract;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NDriveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IRepositoryWrapper _repo;

        public FilesController(IRepositoryWrapper repo)
        {
            this._repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var files = await _repo.File.FindAll();
                return Ok(new JsonResponse<IEnumerable<File>>
                    (
                        StatusCodes.Status200OK,
                        files.Count(),
                        files
                    ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet("root")]
        public async Task<IActionResult> GetRoot()
        {
            try
            {
                var files = await _repo.File.FindByCondition(f => f.ParentFolderId == null);
                return Ok(new JsonResponse<IEnumerable<File>>
                    (
                        StatusCodes.Status200OK,
                        files.Count(),
                        files
                    ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet("{id}", Name = "GetFile")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var file = await _repo.File.FindById(id);
                if (file == null) return NotFound();

                return Ok(new JsonResponse<File>
                    (
                        StatusCodes.Status200OK,
                        null,
                        file
                    ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
    }
}