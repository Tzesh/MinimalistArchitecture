using FluentValidation;

namespace MinimalistArchitecture.User;
public class UserValidator : AbstractValidator<UserDTO>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(64).WithMessage("Name must be between 2 and 64 characters");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email must be a valid email address");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(64).WithMessage("Password must be between 8 and 64 characters");
        }
    }