using ExpenseTracker.Application.Dtos.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(p => p.Email).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ایمیل نمیتواند خالی باشد")
                .EmailAddress().WithMessage("فرمت ایمیل صحیح نمیباشد");

            RuleFor(p => p.Password).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("لطفا رمز را وارد کنید")
                .MinimumLength(4).WithMessage("رمز کوتاه هست")
                .MaximumLength(20).WithMessage("رمز نمیتواند بیشتر از 20 حرف باشد");
        }
    }
}
