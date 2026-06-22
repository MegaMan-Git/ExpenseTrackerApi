using ExpenseTracker.Application.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Validators
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator()
        {
            RuleFor(p => p.Name).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("نام دسته بندی نمیتواند خالی باشد")
                .MaximumLength(50).WithMessage("دسته بندی نمیتواند بیشتر از 50 حرف باشد");
        }
    }
}
