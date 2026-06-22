using AutoMapper;
using ExpenseTracker.Application.CustomException;
using ExpenseTracker.Application.Dtos;
using ExpenseTracker.Application.Dtos.Enums;
using ExpenseTracker.Application.Filters;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.DataLayer.Context;
using ExpenseTracker.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.DataLayer.Repositories
{
    public class CategoryInfoRepository : ICategoryInfoRepository
    {
        #region Dependency Injection
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CategoryInfoRepository(AppDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #endregion

        #region Create
        public async Task<CategoryDto> CreateCategoryAsync
            (Guid userId, CreateCategoryDto categoryDto)
        {
            //بررسی میکند آیا کتکوری که قرار است ایجاد شود اسمش تکراری نباشد
            var Result = await _context.Categories
                .FirstOrDefaultAsync(p=>p.UserId==userId&&p.Name==categoryDto.Name.Trim());
            if (Result != null)
                throw new BadRequestException("نام دسته بندی تکراری است");

            //مرحله افزودن
            var NewCategory = new Category{
                UserId = userId,
                Name = categoryDto.Name.Trim(),
            };
            await _context.Categories.AddAsync(NewCategory);
            await _context.SaveChangesAsync();

            //تبدیل به نوع قابل نمایش
            return _mapper.Map<CategoryDto>(NewCategory);
        }
        #endregion
        
        #region Read
        public async Task<IEnumerable<CategoryDto>> GetAllCategoryAsync
            (Guid userId, QueryStringParameters parameters)
        {
            //پیداکردن تمامی دسته بندی ها
            IQueryable<Category> categories = _context.Categories
                .Where(p => p.UserId == userId)
                //خاموش میشود تا سرعت و مصرف حافظه را بهبود دهد Change Tracker
                .AsNoTracking();

            #region Sorting 

            if (parameters.SortDirection == "asc")
            {
                if (parameters.SortBy.ToLower() == "name")
                {
                    categories = categories.OrderBy(p => p.Name);
                }
                else
                {
                    categories = categories.OrderBy(p => p.Id);
                }
            }
            else
            {
                if (parameters.SortBy == "name")
                {
                    categories = categories.OrderByDescending(p => p.Name);
                }
                else
                {
                    categories = categories.OrderByDescending(p => p.Id);
                }
            }
            
            #endregion

            #region Pagination

            categories = categories
                .Skip(parameters.PageSize * (parameters.PageNumber - 1))
                .Take(parameters.PageSize);

            #endregion

            //اجرای کوئری
            var result = await categories.ToArrayAsync();

            //تبدیل به نوع قابل نمایش
            return _mapper.Map<IEnumerable<CategoryDto>>(result);
        }
        public async Task<CategoryDto> GetCategoryByIdAsync
            (Guid userId, int categoryId)
        {
            //پیداکردن دسته بندی مشخص شده
            var Category = await _context.Categories
                //خاموش میشود تا سرعت و مصرف حافظه را بهبود دهد Change Tracker
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>p.UserId == userId && p.Id == categoryId);
            if (Category == null)
                throw new KeyNotFoundException("دسته بندی مورد نظر یافت نشد");

            //تبدیل به نوع قابل نمایش
            return _mapper.Map<CategoryDto>(Category);
        }
        #endregion

        #region Update
        public async Task<CategoryDto> UpdateCategoryAsync
            (Guid userId,int categoryId ,UpdateCategoryDto categoryDto)
        {
            //بررسی میکند آیا کتکوری که قرار است تغییر کند اسمش تکراری نباشد
            var Result = await _context.Categories
                .FirstOrDefaultAsync(
                p => p.UserId == userId &&
                p.Name == categoryDto.Name.Trim() &&
                p.Id != categoryId
                );
            if (Result != null)
                throw new BadRequestException("نمیتوانید اسم قبلی آن را بگذارید");

            //پیداکردن دسته بندی مشخص شده
            var Category = await _context.Categories
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == categoryId);
            if(Category == null)
                throw new KeyNotFoundException("دسته بندی مورد نظر یافت نشد");
            
            //تغییرنام دسته بندی
            Category.Name = categoryDto.Name.Trim();
            await _context.SaveChangesAsync();

            //تبدیل به نوع قابل نمایش
            return _mapper.Map<CategoryDto>(Category);
        }
        #endregion

        #region Delete
        public async Task<DeleteCategoryResult> DeleteCategoryAsync(Guid userId, int categoryId)
        {
            //پیداکردن دسته بندی مشخص شده
            var Category = await _context.Categories
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == categoryId);
            if (Category == null)
                //پیدانشد
                return DeleteCategoryResult.NotFound;
            
            //اگر زیرمجموعه داشت نگذار حذف شود
            var hasExpenses = await _context.Expenses.AnyAsync(e => e.UserId == userId && e.CategoryId == categoryId);
            if (hasExpenses)
                return DeleteCategoryResult.HasExpense;
                

            //حذف کردن
            _context.Categories.Remove(Category);
            await _context.SaveChangesAsync();

            //حذف شد
            return DeleteCategoryResult.Deleted;
        }
        #endregion
    }
}
