using ExpenseTracker.Application.Dtos;
using ExpenseTracker.Application.Dtos.Enums;
using ExpenseTracker.Application.Filters;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/expense")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        #region Dependency Injection
        private readonly IExpenseInfoRepository _expenseInfoRepository;
        public ExpenseController(IExpenseInfoRepository expenseInfoRepository)
        {
            _expenseInfoRepository = expenseInfoRepository;
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
        public async Task<ActionResult<ExpenseDto>> CreateExpense(CreateExpenseDto create)
        {
            //دریافت شناسه کاربری
            Guid userId = GetUserId();
         
            var expenseDto = await _expenseInfoRepository.CreateExpenseAsync(userId,create);
            
            return CreatedAtAction(nameof(GetExpense),
                new {expenseId = expenseDto.Id },
                expenseDto);
        }
        #endregion

        #region Action Read
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetAllExpenses
            ([FromQuery] ExpenseQueryString queryString)
        {
            //دریافت شناسه کاربری
            Guid userId = GetUserId();

            var expenseDto = await _expenseInfoRepository
                .GetAllExpenseAsync(userId,queryString);

            if (expenseDto == null)
                return NotFound("هزینه ها یافت نشدند");

            return Ok(expenseDto);
        }

        [HttpGet("{expenseId}")]
        public async Task<ActionResult<ExpenseDto>> GetExpense(int expenseId)
        {
            //دریافت شناسه کاربری
            Guid userId = GetUserId();

            var expenseDto = await _expenseInfoRepository
                .GetExpenseByIdAsync(userId, expenseId);

            if (expenseDto == null)
                return NotFound("هزینه ای یافت نشد");

            return Ok(expenseDto);
        }
        #endregion

        #region Action Update
        [HttpPut("{expenseId}")]
        public async Task<ActionResult<ExpenseDto>> UpdateExpense(
            int expenseId,UpdateExpenseDto update)
        {
            //دریافت شناسه کاربری
            Guid userId = GetUserId();

            var expenseDto = await _expenseInfoRepository
                .UpdateExpenseAsync(userId, expenseId, update);
            
            return Ok(expenseDto);
        }
        #endregion

        #region Action Patch
        [HttpPatch("{expenseId}")]
        public async Task<ActionResult<ExpenseDto>> PatchExpense(
            int expenseId,JsonPatchDocument<PatchExpenseDto> patch)
        {
            //بررسی خالی نبودن سند پچ
            if (patch == null)
                return BadRequest("Patch document is required.");

            //دریافت شناسه کاربری
            var userId = GetUserId();

            //دریافت اطلاعات هزینه درخواست شده
            var expenseDto = await _expenseInfoRepository
                .GetExpenseByIdAsync(userId, expenseId);

            //ارسال اطلاعات قابل تغییر در پچ به مدل مخصوص اینکار
            var patchExpense = new PatchExpenseDto()
            {
                Amount = expenseDto.Amount,
                Date = expenseDto.Date,
                Note = expenseDto.Note,
                Title = expenseDto.Title
            }; 

            //بررسی تغییرات و اعمال آن در صورت ارسال صحیح مقادیر از سمت کلاینت
            patch.ApplyTo(patchExpense, ModelState);

            //اگر مقادیر صحیح ارسال نشده بود متوقف شو
            if (!ModelState.IsValid)
                return BadRequest("مقادیر به درستی ارسال نشده اند");

            //جهت جلوگیری از اعمال تغییراتی مثل حذف کردن یک فیلد اجباری و اعمال اجباری آن
            if (!TryValidateModel(patchExpense))
                return BadRequest("مقادیر به درستی ارسال نشده اند");

            //ارسال تغییرات به ریپازتوری جهت ثبت در دیتابیس
            expenseDto = await _expenseInfoRepository
                .PatchExpenseAsync(userId, expenseId, patchExpense);

            //نمایش تغییرات
            return Ok(expenseDto);
        }
        #endregion

        #region Action Delete
        [HttpDelete("{expenseId}")]
        public async Task<ActionResult> DeleteExpense(int expenseId)
        {
            //دریافت شناسه کاربری
            Guid userId = GetUserId();

            var isDelete = await _expenseInfoRepository
                .DeleteExpenseAsync(userId, expenseId);

            if (isDelete == DeleteExpenseResult.NotFound)
                return NotFound("هزینه ای یافت نشد");

            return NoContent();

        }
        #endregion
    }
}
