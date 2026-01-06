using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var categories = await _unitOfWork.Category.dbSet.Where(c => c.IsActive == true).OrderBy(c => c.DisplayOrder).ToListAsync();
            return Ok(categories);
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetRequestsCategories()
        {
            var categories = await _unitOfWork.Category.dbSet.Where(c => (c.IsActive == true) && (c.requestType == RequestType.Regular)).OrderBy(c => c.DisplayOrder).ToListAsync();
            return Ok(categories);
        }

        [HttpGet("bugs")]
        public async Task<IActionResult> GetBugsCategories()
        {
            var categories = await _unitOfWork.Category.dbSet.Where(c => (c.IsActive == true) && (c.requestType == RequestType.Bug)).OrderBy(c => c.DisplayOrder).ToListAsync();
            return Ok(categories);
        }

        [HttpGet("orderedbytype")]
        public async Task<IActionResult> GetCategoriesOrderedByType()
        {
            var requestsCategories = await _unitOfWork.Category.dbSet.Where(c => (c.IsActive == true) && (c.requestType == RequestType.Regular)).OrderBy(c => c.DisplayOrder).ToListAsync();
            var bugsCategories = await _unitOfWork.Category.dbSet.Where(c => (c.IsActive == true) && (c.requestType == RequestType.Bug)).OrderBy(c => c.DisplayOrder).ToListAsync();
            return Ok(requestsCategories.Concat(bugsCategories));
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
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto category)
        {
            Category newCategory = new();
            newCategory.Name = category.Name;
            newCategory.requestType = category.Type;
            newCategory.IsActive = true;
            newCategory.Color = category.Color;
            newCategory.DisplayOrder = 100;
            await _unitOfWork.Category.Add(newCategory);
            await _unitOfWork.Save();
            return CreatedAtAction(nameof(Get), new { id = newCategory.Id }, newCategory);
        }

        [HttpPost("reorder")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> ReOrder([FromBody] List<int> orderedIds)
        {
            var categories = await _unitOfWork.Category.dbSet.Where(c => c.IsActive == true).OrderBy(c => c.DisplayOrder).ToListAsync();
            Dictionary<int, int> idsNewOrder = new Dictionary<int, int>();

            for (int i = 0; i < orderedIds.Count; i++) {             
                idsNewOrder[orderedIds[i]] = i+1;
            }

            foreach (var category in categories) {
                if (idsNewOrder.ContainsKey(category.Id) == true) {
                    category.DisplayOrder = idsNewOrder[category.Id];
                }
            }

            await _unitOfWork.Save();
            return Ok();

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto category)
        {
            var existingCategory = await _unitOfWork.Category.Get(c => c.Id == id, null, true);
            if (existingCategory == null)
                return NotFound();
            existingCategory.Name = category.Name;
            existingCategory.Color = category.Color;
            await _unitOfWork.Save();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Category.Get(c => c.Id == id, null, true);
            if (category == null)
                return NotFound();
            category.IsActive = false;
            await _unitOfWork.Save();
            return NoContent();
        }
    }
}
