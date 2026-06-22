using ExpenseTracker.Application.Dtos;
using ExpenseTracker.Application.Dtos.Enums;
using ExpenseTracker.Application.Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Interfaces
{
    public interface ICategoryInfoRepository
    {
        public Task<IEnumerable<CategoryDto>> GetAllCategoryAsync(Guid userId,QueryStringParameters parameters);
        public Task<CategoryDto> GetCategoryByIdAsync(Guid userId,int categoryId);
        public Task<CategoryDto> CreateCategoryAsync(Guid userId,CreateCategoryDto categoryDto);
        public Task<CategoryDto> UpdateCategoryAsync(Guid userId,int categoryId,UpdateCategoryDto categoryDto);
        public Task<DeleteCategoryResult> DeleteCategoryAsync(Guid userId, int categoryId);
    }
}
