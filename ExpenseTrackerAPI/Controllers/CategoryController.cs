using ExpenseTracker.Application.Dtos;
using ExpenseTracker.Application.Dtos.Enums;
using ExpenseTracker.Application.Filters;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.DataLayer.Entities;
using ExpenseTracker.DataLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/category")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        #region Dependency Injection
        private readonly ICategoryInfoRepository _categoryInfoRepository;
        public CategoryController(ICategoryInfoRepository categoryInfoRepository)
        {
            _categoryInfoRepository = categoryInfoRepository;
        }
        #endregion

        #region GetUserId
        public Guid GetUserId()
        {
            //دریافت شناسه فردی که احراز هویت کرده از طریق کلایم
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Guid userId;
            if (!Guid.TryParse(claimValue, out userId))
            {
                throw new Exception("UserId is invalid");
            }

            return userId;
        }
        #endregion

        #region Action Create
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto create)
        {
            //دریافت شناسه کاربری
            Guid userId = GetUserId();

            //دریافت دسته بندی بعد اضافه شدن
            var categoryDto = await _categoryInfoRepository.CreateCategoryAsync(userId, create);

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { categoryid = categoryDto.Id },categoryDto);

        }
        #endregion

        #region Action Read

        //ReadAll
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories
            ([FromQuery] QueryStringParameters parameters)
        {
            //دریافت شناسه کاربری
            Guid userId = GetUserId();

            var categoryDto = await _categoryInfoRepository
                .GetAllCategoryAsync(userId, parameters);


            return Ok(categoryDto);
        }

        //ReadOne
        [HttpGet("{categoryid}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int categoryid)
        {
            //دریافت شناسه کاربری
            Guid userId = GetUserId();

            var categoryDto = await _categoryInfoRepository
                .GetCategoryByIdAsync(userId, categoryid);
            

            return Ok(categoryDto);
        }
        
        #endregion

        #region Action update
        [HttpPut("{categoryId}")]
        public async Task<ActionResult> UpdateCategory(int categoryId, UpdateCategoryDto update)
        {
            //دریافت شناسه کاربری
            Guid userId = GetUserId();

            //بررسی تغییرات
            var categoryDto = await _categoryInfoRepository
                .UpdateCategoryAsync(userId, categoryId, update);
            

            return NoContent();
        }
        #endregion

        #region Ation Delete

        [HttpDelete("{categoryId}")]
        public async Task<ActionResult> DeleteCategory(int categoryId)
        {
            //دریافت شناسه کاربری
            Guid userId = GetUserId();

            //بررسی وضعیت حذف 
            var result = await _categoryInfoRepository.DeleteCategoryAsync(userId, categoryId);
            if (result == DeleteCategoryResult.Deleted)
                return NoContent();

            if (result == DeleteCategoryResult.NotFound)
                return NotFound("دسته بندی یافت نشد");

            return BadRequest("شما نمی ‌توانید این دسته را حذف کنید زیرا هزینه ‌هایی دارد");
        }
        #endregion
    }
}
