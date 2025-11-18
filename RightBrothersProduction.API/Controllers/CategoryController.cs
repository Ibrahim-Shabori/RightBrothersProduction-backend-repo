using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using RightBrothersProduction.Models;

namespace RightBrothersProduction.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _unitOfWork.Category.GetAll();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public IActionResult Create([FromForm] CreateCategoryDto category)
        {
            Category newCategory = new();
            newCategory.Name = category.Name;
            newCategory.Description = category.Description;
            newCategory.requestType = category.Type;
            _unitOfWork.Category.Add(newCategory);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { id = newCategory.Id }, category);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CreateCategoryDto category)
        {
            var existingCategory = _unitOfWork.Category.Get(c => c.Id == id);
            if (existingCategory == null)
                return NotFound();
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.requestType = category.Type;
            _unitOfWork.Save();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category == null)
                return NotFound();
            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            return NoContent();
        }
    }
}
