using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Processing.QueryPipelines;

internal class QueryValidationBehavior<T, TResult> : IPipelineBehavior<T, TResult> where T : IQuery<TResult>
{
    private readonly IEnumerable<IValidator<T>> _validators;

    public QueryValidationBehavior(IEnumerable<IValidator<T>> validators = null)
    {
        _validators = validators ?? Array.Empty<IValidator<T>>();
    }

    public Task<TResult> Handle(T request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var errorMessages = GetErrorMessages(request);

        if (errorMessages.Any())
        {
            ThrowException(errorMessages);
        }

        return next();
    }

    private IEnumerable<ValidationFailure> GetErrorMessages(T request)
    {
        if (!_validators.Any())
            return Enumerable.Empty<ValidationFailure>();

        return _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error != null);
    }

    private static void ThrowException(IEnumerable<ValidationFailure> errors)
    {
        var builder = new StringBuilder("Reason: " + Environment.NewLine);
        builder.Append(string.Join(Environment.NewLine, errors.Select(x => x.ErrorMessage)));

        var message = "InvalidQuery " + builder;
        throw new ValidationException(message, errors);
    }
}
