using ExpenseTracker.Application.Dtos;
using ExpenseTracker.Application.Dtos.Enums;
using ExpenseTracker.Application.Filters;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Interfaces
{
    public interface IExpenseInfoRepository
    {
        public Task<IEnumerable<ExpenseDto>> GetAllExpenseAsync
            (Guid userId,ExpenseQueryString queryString);
        public Task<ExpenseDto> GetExpenseByIdAsync(Guid userId, int expenseId);
        public Task<ExpenseDto> CreateExpenseAsync(Guid userId, CreateExpenseDto expenseDto);
        public Task<ExpenseDto> UpdateExpenseAsync
            (Guid userId, int expenseId, UpdateExpenseDto expenseDto);
        public Task<ExpenseDto> PatchExpenseAsync
            (Guid userId, int expenseId, PatchExpenseDto expenseDto);
        public Task<DeleteExpenseResult> DeleteExpenseAsync(Guid userId, int expenseId);
    }
}
