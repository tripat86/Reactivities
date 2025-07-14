using Application.DTOs;
using FluentValidation;

namespace Application.Activities.Validators
{
    public class BaseActivityValidator<T, TDto> : AbstractValidator<T>
        where TDto : BaseActivityDto
    {
        public BaseActivityValidator(Func<T, TDto> selector)
        {
            // selector(x) becomes x.ActivityDto when called from CreateActivityValidator
            // or x.EditActivityDto when called from EditActivityValidator
            // This allows us to validate the properties of the DTOs directly.
            // For example, in CreateActivityValidator, selector(x) will return x.ActivityDto,
            // and in EditActivityValidator, it will return x.EditActivityDto.
            // selector is a function that extracts the DTO from the entity being validated. 
            // so when we call selector(x), it invokes/runs and returns the DTO that we want to validate.
            // x => x.ActivityDto is a lambda expression that takes an instance of T (which is CreateActivity.Command in this case)
            // so x here is an instance of CreateActivity.Command
            RuleFor(x => selector(x).Title)
             .NotEmpty().WithMessage("Title is required")
             .MaximumLength(100).WithMessage("Title must not exceed 100 characters");
            RuleFor(x => selector(x).Description)
                .NotEmpty().WithMessage("Description is required");
            RuleFor(x => selector(x).Date)
                .GreaterThan(DateTime.UtcNow).WithMessage("Date must be in the future");
            RuleFor(x => selector(x).Category)
                .NotEmpty().WithMessage("Category is required");
            RuleFor(x => selector(x).City)
                .NotEmpty().WithMessage("City is required");
            RuleFor(x => selector(x).Venue)
                .NotEmpty().WithMessage("Venue is required");
            RuleFor(x => selector(x).Latitude)
                .NotEmpty().WithMessage("Latitude is required")
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90 degrees");
            RuleFor(x => selector(x).Longitude)
                .NotEmpty().WithMessage("Longitude is required")
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180 degrees");
        }
    }
}