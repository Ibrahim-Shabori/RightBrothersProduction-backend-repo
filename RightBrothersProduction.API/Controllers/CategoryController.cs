using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using RightBrothersProduction.Models;
using System.Threading.Tasks;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _unitOfWork.Category.GetAll();
            return Ok(categories);
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetRequestsCategories()
        {
            var categories = await _unitOfWork.Category.GetAll(c => c.requestType == RequestType.Regular);
            return Ok(categories);
        }

        [HttpGet("bugs")]
        public async Task<IActionResult> GetBugsCategories()
        {
            var categories = await _unitOfWork.Category.GetAll(c => c.requestType == RequestType.Bug);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _unitOfWork.Category.Get(c => c.Id == id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto category)
        {
            Category newCategory = new();
            newCategory.Name = category.Name;
            newCategory.Description = category.Description;
            newCategory.requestType = category.Type;
            await _unitOfWork.Category.Add(newCategory);
            await _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { id = newCategory.Id }, category);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto category)
        {
            var existingCategory = await _unitOfWork.Category.Get(c => c.Id == id);
            if (existingCategory == null)
                return NotFound();
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.requestType = category.Type;
            await _unitOfWork.Save();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Category.Get(c => c.Id == id);
            if (category == null)
                return NotFound();
            _unitOfWork.Category.Remove(category);
            await _unitOfWork.Save();
            return NoContent();
        }
    }
}
