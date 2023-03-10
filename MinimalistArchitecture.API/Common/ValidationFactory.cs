using System.Net;
using System.Reflection;
using FluentValidation;

namespace MinimalistArchitecture.Common
{
    /// <summary>
    /// A class that contains a factory method for creating a validation filter
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ValidateAttribute : Attribute
    {
    }

    public static class ValidationFactory
    {
        /// <summary>
        /// A factory method for creating a validation filter
        /// </summary>
        /// <param name="context">The context for the filter</param>
        /// <param name="next">The next filter in the pipeline</param>
        /// <returns>A validation filter</returns>
        public static EndpointFilterDelegate ValidationFilterFactory(EndpointFilterFactoryContext context, EndpointFilterDelegate next)
        {
            // get the validators
            IEnumerable<ValidationDescriptor> validationDescriptors = GetValidators(context.MethodInfo, context.ApplicationServices);
            
            // if there are any validators, create a validation filter
            if (validationDescriptors.Any())
            {
                // return the validation filter
                return invocationContext => Validate(validationDescriptors, invocationContext, next);
            }

            // pass-thru
            return invocationContext => next(invocationContext);
        }

        /// <summary>
        /// A validation filter
        /// </summary>
        /// <param name="validationDescriptors">A list of validation descriptors</param>
        /// <param name="invocationContext">The context for the filter</param>
        /// <param name="next">The next filter in the pipeline</param>
        /// <returns>A validation filter</returns>
        private static async ValueTask<object?> Validate(IEnumerable<ValidationDescriptor> validationDescriptors, EndpointFilterInvocationContext invocationContext, EndpointFilterDelegate next)
        {
            // validate each argument
            foreach (ValidationDescriptor descriptor in validationDescriptors)
            {
                // get the argument
                var argument = invocationContext.Arguments[descriptor.ArgumentIndex];
                
                // validate the argument
                if (argument is not null)
                {
                    var validationResult = await descriptor.Validator.ValidateAsync(
                        new ValidationContext<object>(argument)
                    );

                    // if the argument is invalid, return a validation problem
                    if (!validationResult.IsValid)
                    {
                        return Results.ValidationProblem(validationResult.ToDictionary(),
                            statusCode: (int)HttpStatusCode.UnprocessableEntity);
                    }
                }
            }

            // pass-thru
            return await next.Invoke(invocationContext);
        }

        /// <summary>
        /// A class that contains information about a validation descriptor
        /// </summary>
        static IEnumerable<ValidationDescriptor> GetValidators(MethodInfo methodInfo, IServiceProvider serviceProvider)
        {
            // get the parameters
            ParameterInfo[] parameters = methodInfo.GetParameters();

            // iterate over the parameters
            for (int i = 0; i < parameters.Length; i++)
            {
                // get the parameter
                ParameterInfo parameter = parameters[i];

                // if the parameter has a [Validate] attribute, get the validator
                if (parameter.GetCustomAttribute<ValidateAttribute>() is not null)
                {
                    // get the validator type
                    Type validatorType = typeof(IValidator<>).MakeGenericType(parameter.ParameterType);

                    // note that FluentValidation validators needs to be registered as singleton
                    IValidator? validator = serviceProvider.GetService(validatorType) as IValidator;

                    // if the validator is not null, return it
                    if (validator is not null)
                    {
                        yield return new ValidationDescriptor { ArgumentIndex = i, ArgumentType = parameter.ParameterType, Validator = validator };
                    }
                }
            }
        }
        
        /// <summary>
        /// A class that contains information about a validation
        /// </summary>
        private class ValidationDescriptor
        {
            public required int ArgumentIndex { get; init; }
            public required Type ArgumentType { get; init; }
            public required IValidator Validator { get; init; }
        }
    }
}