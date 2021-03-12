using Contract;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NDriveAPI.Models.Folder;
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
    public class FoldersController : ControllerBase
    {
        private readonly IRepositoryWrapper _repo;

        public FoldersController(IRepositoryWrapper repo)
        {
            this._repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var folders = await _repo.Folder.FindAll();
                return Ok(new JsonResponse<IEnumerable<Folder>>
                    (
                        StatusCodes.Status200OK,
                        folders.Count(),
                        folders
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

        [HttpGet("{id}", Name = "GetFolder")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var folder = await _repo.Folder.FindWithSubFolders(id);
                return Ok(new JsonResponse<Folder>
                    (
                        StatusCodes.Status200OK,
                        null,
                        folder
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FolderCreateModel model)
        {
            try
            {
                var folders = await _repo.Folder
                    .FindByCondition(f => f.ParentFolderId == model.ParentFolderId && f.FolderName.ToLower() == model.FolderName.ToLower());
                if (folders.Any()) return BadRequest("Folder of same name already created");

                var folder = new Folder();
                folder.FolderName = model.FolderName;
                folder.ParentFolderId = model.ParentFolderId;

                await _repo.Folder.Create(folder);
                await _repo.Commit();
                return Created("GetFolder", folder);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Error posting data to the database");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] FolderCreateModel model)
        {
            try
            {
                var folder = await _repo.Folder.FindById(id);
                if (folder == null) return NotFound();

                var folders = await _repo.Folder
                    .FindByCondition(f => f.ParentFolderId == folder.ParentFolderId && f.FolderName.ToLower() == model.FolderName.ToLower());
                if (folders.Any()) return BadRequest("Folder of same name already created");

                folder.FolderName = model.FolderName;
                _repo.Folder.Update(folder);
                await _repo.Commit();

                return StatusCode(StatusCodes.Status204NoContent, "Folder Updated Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Error updating data to the database");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var folder = await _repo.Folder.FindById(id);
                if (folder == null) return NotFound();
                _repo.Folder.Delete(folder);
                await _repo.Commit();
                return StatusCode(StatusCodes.Status204NoContent, "Folder Deleted Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Error deleting data to the database");
            }
        }
    }
}
