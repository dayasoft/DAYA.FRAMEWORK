using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Processing.CommandPipelines;

/// <summary>
/// Pipeline behavior that validates commands using FluentValidation validators
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class CommandValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public CommandValidationBehavior(IEnumerable<IValidator<TRequest>> validators = null)
    {
        _validators = validators ?? Enumerable.Empty<IValidator<TRequest>>();
    }

    private IEnumerable<ValidationFailure> GetErrorMessages(TRequest request)
    {
        if (_validators == null || !_validators.Any())
        {
            return Array.Empty<ValidationFailure>();
        }

        // Use ValidationContext for more accurate validation
        var context = new ValidationContext<TRequest>(request);

        return _validators
            .Where(v => v != null)
            .SelectMany(v => v.Validate(context).Errors)
            .Where(error => error != null);
    }

    private static void ThrowException(IEnumerable<ValidationFailure> errors)
    {
        var errorList = errors.ToList();
        var builder = new StringBuilder("Reason: " + Environment.NewLine);

        // Group errors by property for better readability
        var errorsByProperty = errorList
            .GroupBy(e => e.PropertyName)
            .OrderBy(g => g.Key);

        foreach (var propertyErrors in errorsByProperty)
        {
            var propertyName = string.IsNullOrEmpty(propertyErrors.Key) ? "[General]" : propertyErrors.Key;
            builder.AppendLine($"- {propertyName}:");

            foreach (var error in propertyErrors)
            {
                builder.AppendLine($"  * {error.ErrorMessage}");
            }
        }

        var message = "InvalidCommand " + builder;
        throw new ValidationException(message, errorList);
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationFailures = GetErrorMessages(request).ToList();

        // If there are validation errors, throw an exception
        if (validationFailures.Any())
        {
            ThrowException(validationFailures);
        }

        // Continue with the pipeline
        return await next();
    }
}