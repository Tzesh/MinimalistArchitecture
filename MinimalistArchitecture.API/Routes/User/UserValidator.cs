using FluentValidation;

namespace MinimalistArchitecture.Routes.User;
public class UserValidator : AbstractValidator<UserDTO>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email must be a valid email address");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(64).WithMessage("Password must be between 8 and 64 characters");
        }
    }