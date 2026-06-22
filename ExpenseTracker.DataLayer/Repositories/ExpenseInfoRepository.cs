using AutoMapper;
using Azure;
using ExpenseTracker.Application.CustomException;
using ExpenseTracker.Application.Dtos;
using ExpenseTracker.Application.Dtos.Enums;
using ExpenseTracker.Application.Filters;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.DataLayer.Context;
using ExpenseTracker.DataLayer.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.DataLayer.Repositories
{
    public class ExpenseInfoRepository : IExpenseInfoRepository
    {

        #region Dependency Injection
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ExpenseInfoRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #endregion

        #region Create
        public async Task<ExpenseDto> CreateExpenseAsync(
            Guid userId, CreateExpenseDto expenseDto)
        {
            //پیدا کردن دسته بندی 
            var category = await _context.Categories
                .FirstOrDefaultAsync(p =>p.UserId == userId &&
                p.Id == expenseDto.CategoryId);
            if (category == null) 
                throw new KeyNotFoundException($"یافت نشد {expenseDto.CategoryId} دسته بندی با شناسه");

            //اگر تایتل هزینه تکراری بود اضافه نکن
            var isDuplicate = await _context.Expenses
                .AnyAsync(p => p.UserId == userId 
                && p.Title == expenseDto.Title.Trim()
                && p.CategoryId == expenseDto.CategoryId);
            
            if (isDuplicate)
                throw new BadRequestException("تایتل تکراری است");

            //مرحله افزودن
            var newExpense = new Expense
            {
                UserId = userId,
                Title = expenseDto.Title.Trim(),
                CategoryId = expenseDto.CategoryId,
                Amount = expenseDto.Amount,
                CreatedAt = DateTime.UtcNow,
                Note = expenseDto.Note,
                Date = expenseDto.Date,
                CategoryName = category.Name
            };

            await _context.Expenses.AddAsync(newExpense);
            await _context.SaveChangesAsync();

            //تبدیل به نوع قابل نمایش
            return _mapper.Map<ExpenseDto>(newExpense);
        }
        #endregion

        #region Read
        public async Task<IEnumerable<ExpenseDto>> GetAllExpenseAsync
            (Guid userId,ExpenseQueryString queryString)
        {
            //دریافت تمامی هزینه های کاربر مشخص شده
            IQueryable<Expense> expenses = _context.Expenses.Where(p =>p.UserId == userId)
                .AsNoTracking();

            #region Filtering

            if (queryString.FromDate.HasValue)
                expenses = expenses.Where(p => p.CreatedAt >= queryString.FromDate);

            if (queryString.ToDate.HasValue)
                expenses = expenses.Where(p => p.CreatedAt <= queryString.ToDate);

            if (queryString.MinAmount.HasValue)
                expenses = expenses.Where(p => p.Amount >= queryString.MinAmount);

            if (queryString.MaxAmount.HasValue)
                expenses = expenses.Where(p => p.Amount <= queryString.MaxAmount);

            #endregion

            #region Sorting

            switch (queryString.SortBy?.ToLower())
            {
                case "title":
                    expenses = queryString.SortDirection == "asc"
                        ? expenses.OrderBy(p => p.Title)
                        : expenses.OrderByDescending(p => p.Title);
                    break;
                
                case "categoryname":
                    expenses = queryString.SortDirection == "asc"
                        ? expenses.OrderBy(p => p.CategoryName)
                        : expenses.OrderByDescending(p => p.CategoryName);
                    break;

                case "categoryid":
                    expenses = queryString.SortDirection == "asc"
                        ? expenses.OrderBy(p => p.CategoryId)
                        : expenses.OrderByDescending(p => p.CategoryId);
                    break;

                case "amount":
                    expenses = queryString.SortDirection == "asc"
                        ? expenses.OrderBy(p => p.Amount)
                        : expenses.OrderByDescending(p => p.Amount);
                    break;

                default:
                    expenses = queryString.SortDirection == "asc"
                        ? expenses.OrderBy(p => p.Id)
                        : expenses.OrderByDescending(p => p.Id);
                    break;
            }
            #endregion

            #region Pagintaion

            expenses = expenses
                .Skip(queryString.PageSize * (queryString.PageNumber - 1))
                .Take(queryString.PageSize);
            
            #endregion

            // اجرای کوئری
            var result = await expenses.ToArrayAsync();

            //تبدیل به نوع قابل نمایش
            return _mapper.Map<IEnumerable<ExpenseDto>>(result);
        }
        
        public async Task<ExpenseDto> GetExpenseByIdAsync(Guid userId, int expenseId)
        {
            var expense = await _context.Expenses
                //خاموش میشود تا سرعت و مصرف حافظه را بهبود دهد Change Tracker
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == expenseId);

            if (expense == null)
                throw new KeyNotFoundException($"یافت نشد {expenseId} هزینه ای با شناسه");

            //تبدیل به نوع قابل نمایش
            return _mapper.Map<ExpenseDto>(expense);
        }
        #endregion

        #region Update
        public async Task<ExpenseDto> UpdateExpenseAsync(
            Guid userId, int expenseId, UpdateExpenseDto updateExpense)
        {
            //یافتن هزینه مد نظر
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == expenseId);
            // بررسی خالی بودن
            if (expense == null)
                throw new KeyNotFoundException($"یافت نشد {expenseId} هزینه ای با شناسه");

            //ارسال تغییرات
            expense.Amount = updateExpense.Amount;
            expense.Date = updateExpense.Date;
            expense.Note = updateExpense.Note;
            expense.Title = updateExpense.Title.Trim();

            //ذخیره تغییرات
            await _context.SaveChangesAsync();

            //تبدیل به نوع قابل نمایش
            return _mapper.Map<ExpenseDto>(expense);
        }
        #endregion

        #region Patch
        public async Task<ExpenseDto> PatchExpenseAsync(
            Guid userId, int expenseId,PatchExpenseDto expensePatch)
        {
            //پیدا کردن هزینه مشخص شده
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == expenseId);

            if (expense == null)
                throw new KeyNotFoundException($"یافت نشد {expenseId} هزینه ای با شناسه");

            //اعمال تغییرات
            expense.Amount = expensePatch.Amount;
            expense.Date = expensePatch.Date;
            expense.Note = expensePatch.Note;
            expense.Title = expensePatch.Title.Trim();

            //ذخیره تغییرات
            await _context.SaveChangesAsync();

            //تبدیل به نوع قابل نمایش
            return _mapper.Map<ExpenseDto>(expense);
        }
        #endregion

        #region Delete
        public async Task<DeleteExpenseResult> DeleteExpenseAsync(Guid userId, int expenseId)
        {
            //یافتن هزینه مد نظر
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == expenseId);
            // بررسی خالی بودن
            if (expense == null)
                return DeleteExpenseResult.NotFound;

            //حذف محصول
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            //حذف موفق
            return DeleteExpenseResult.Deleted;
        }
        #endregion
    }
}
