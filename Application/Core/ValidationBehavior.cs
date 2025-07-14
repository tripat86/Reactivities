using FluentValidation;
using MediatR;

namespace Application.Core;
public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // If no validator is provided, skip validation and proceed to the next handler in Mediator Pipeline
        if (validator == null) return await next();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            // If validation fails, throw an exception with the validation errors
            throw new ValidationException(validationResult.Errors);
        }
        
        return await next();
    }
}
