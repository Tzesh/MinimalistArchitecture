using FluentValidation;
using MinimalistArchitecture.Abstract;

namespace MinimalistArchitecture.Todo;
public class TodoValidator : Validator<TodoDTO>
    {
        public TodoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(500).WithMessage("Name must be between 3 and 500 characters");
        }
    }