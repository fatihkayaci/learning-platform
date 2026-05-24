using FluentValidation;

namespace Identity.Application.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email zorunludur")
            .EmailAddress().WithMessage("Geçerli bir email giriniz")
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalı");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("İsim zorunludur")
            .MinimumLength(2)
            .MaximumLength(100);

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Geçersiz rol");
    }
}