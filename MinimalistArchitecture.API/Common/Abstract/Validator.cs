using FluentValidation;

namespace MinimalistArchitecture.Common.Abstract;

/// <summary>
/// Base class for all validators
/// </summary>
public abstract class Validator<T> : AbstractValidator<T>
{
    public Validator()
    {
        // add common rules here
    }
}