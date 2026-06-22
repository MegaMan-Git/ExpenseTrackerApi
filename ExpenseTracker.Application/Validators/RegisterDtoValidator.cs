using ExpenseTracker.Application.Dtos.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(p => p.FullName)
                .NotEmpty().WithMessage("لطفا نام خود را وارد کنید")
                .MaximumLength(100).WithMessage("نام شما نمیتواند بیشتر از 100 حرف باشد");

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
