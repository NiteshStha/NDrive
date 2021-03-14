using Contract;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NDriveAPI.Models.DriveModels;
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
    public class DrivesController : ControllerBase
    {
        private readonly IRepositoryWrapper _repo;

        public DrivesController(IRepositoryWrapper repo)
        {
            this._repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var folders = await _repo.Folder.FindAll();
                var files = await _repo.File.FindAll();
                var driveDatas = new DriveDataModel
                {
                    Folders = folders,
                    Files = files
                };

                return Ok(new JsonResponse<DriveDataModel>
                    (
                        StatusCodes.Status200OK,
                        folders.Count() + files.Count(),
                        driveDatas
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
                var folders = await _repo.Folder.FindByCondition(f => f.ParentFolderId == null);
                var files = await _repo.File.FindByCondition(f => f.ParentFolderId == null);
                var driveDatas = new DriveDataModel
                {
                    Folders = folders,
                    Files = files
                };

                return Ok(new JsonResponse<DriveDataModel>
                    (
                        StatusCodes.Status200OK,
                        folders.Count() + files.Count(),
                        driveDatas
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

        [HttpGet("{id}", Name = "GetDriveData")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var folder = await _repo.Folder.FindWithSubFolders(id);
                var files = await _repo.File.FindByCondition(f => f.ParentFolderId == id);
                
                if (folder == null) return NotFound();

                var driveDatas = new DriveDataModel
                {
                    Folders = folder.Folders,
                    Files = files
                };

                return Ok(new JsonResponse<DriveDataModel>
                    (
                        StatusCodes.Status200OK,
                        null,
                        driveDatas
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
