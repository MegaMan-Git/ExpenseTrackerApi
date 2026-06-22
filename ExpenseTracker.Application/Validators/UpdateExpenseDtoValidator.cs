using ExpenseTracker.Application.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Validators
{
    public class UpdateExpenseDtoValidator : AbstractValidator<UpdateExpenseDto>
    {
        public UpdateExpenseDtoValidator()
        {


            RuleFor(x => x.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("نوشتن تایتل اجباری است")
            .MaximumLength(100).WithMessage("تایتل نمیتواند بیشتر از 100 کاراکتر باشد");

            RuleFor(p => p.Amount)
                .NotEmpty().WithMessage("مبلغ نمیتواند خالی باشد")
                .GreaterThanOrEqualTo(0).WithMessage("مبلغ نمیتواند منفی باشد")
                .PrecisionScale(18, 2, true);

            RuleFor(x => x.Note)
                .MaximumLength(300)
                .WithMessage("یادداشت نمیتواند بیشتر از 300 حرف باشد")
                .When(x => !string.IsNullOrWhiteSpace(x.Note));

            RuleFor(p => p.Date)
                .NotEmpty().WithMessage("تاریخ نمیتواند خالی باشد")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("تاریخ نمیتواند برای آینده باشد");

        }
    }
}
