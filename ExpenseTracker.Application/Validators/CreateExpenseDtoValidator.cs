using FluentValidation;
using ExpenseTracker.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Validators
{
    public class CreateExpenseDtoValidator : AbstractValidator<CreateExpenseDto>
    {
        public CreateExpenseDtoValidator()
        {
            RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("نوشتن تایتل اجباری است")
            .MaximumLength(100).WithMessage("تایتل نمیتواند بیشتر از 100 کاراکتر باشد");

            RuleFor(p => p.Amount)
                .NotEmpty().WithMessage("مبلغ نمیتواند خالی باشد")
                .GreaterThanOrEqualTo(0).WithMessage("مبلغ نمیتواند منفی باشد")
                .PrecisionScale(18,2,true)
                .WithMessage("بیشتر دو رقم اعشار یا بیشتر از 18 رقم نمیتوان ذخیره کرد");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("نوشتن آیدی دسته بندی اجباری است")
                .GreaterThan(0).WithMessage("فرمت صحیح آیدی را وارد کنید");

            RuleFor(x => x.Note)
                .MaximumLength(300).WithMessage("یادداشت نمیتواند بیشتر از 300 حرف باشد")
                .When(x => !string.IsNullOrWhiteSpace(x.Note));

            RuleFor(p => p.Date)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("تاریخ نمیتواند برای آینده باشد")
                .NotEmpty().WithMessage("تاریخ نمیتواند خالی باشد");
        }
        
    }
}
