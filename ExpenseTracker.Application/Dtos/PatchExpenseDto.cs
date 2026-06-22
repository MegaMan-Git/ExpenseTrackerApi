using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Dtos
{
    public class PatchExpenseDto
    {
        [Required(ErrorMessage ="تایتل نمیتواند خالی باشد")]
        [MaxLength(100, ErrorMessage = "تایتل نمیتواند بیشتر از 100 کاراکتر باشد")]
        public string Title { get; set; } = string.Empty;

        
        [Range(0.01, double.MaxValue, ErrorMessage = "مبلغ باید از صفر بزرگتر باشد")]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }
        
        [MaxLength(300, ErrorMessage = "یادداشت نمیتواند بیشتر 300 حرف باشد")]
        public string? Note { get; set; }
    }
}
