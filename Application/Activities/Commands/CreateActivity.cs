using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities.Commands
{
    public class CreateActivity
    {
        public class Command : IRequest<Result<string>>
        {
            public required CreateActivityDto ActivityDto { get; set; }
        }

        public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor)
            : IRequestHandler<Command, Result<string>>
        {
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await userAccessor.GetUserAsync();

                var activity = mapper.Map<Activity>(request.ActivityDto);

                context.Activities.Add(activity);

                var attendee = new ActivityAttendee
                {
                    ActivityId = activity.Id,
                    UserId = user.Id,
                    IsHost = true
                };

                activity.Attendees.Add(attendee);

                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                if (!result) return Result<string>.Failure("Failed to create the activity", 400);

                return Result<string>.Success(activity.Id);
            }
        }
    }
}

/*
Below is the full flow how request that goes to CreateActivity in ActivitiesController.cs gets validated:
1. The request is sent to the ActivitiesController.
2. The ActivitiesController calls the CreateActivity.Command with the CreateActivityDto.
3. The MediatR sends the CreateActivity.Command to the ValidationBehavior.
4. The ValidationBehavior checks if there is a validator registered for CreateActivity.Command.
5. If there is a validator, it validates the CreateActivityDto inside the command.
6. If the validation fails, it throws a ValidationException with the validation errors.
7. If the validation passes, it continues to the CreateActivity.Handler.
8. The CreateActivity.Handler processes the command and returns the result.
This ensures that the CreateActivityDto is validated before it reaches the handler, preventing invalid data from being processed.

Below is the full flow how request that goes to CreateActivity in ActivitiesController.cs gets validated:
Excellent! You're asking **how `CreateActivityValidator` gets called automatically**, even though you never explicitly invoke it anywhere.

Let’s walk through the **flow** step by step.

---

## ✅ High-Level Flow (Request → Validation → Handler)

```plaintext
POST /activities            ← HTTP request to API
↓
ActivitiesController.CreateActivity() ← Controller method
↓
MediatR.Send(CreateActivity.Command) ← Sends command
↓
ValidationBehavior<Command, Result<string>> ← Automatically runs before handler
↓
DI resolves IValidator<Command> → finds CreateActivityValidator
↓
CreateActivityValidator (and base validator) run and validate ActivityDto
↓
If valid → Handler runs
If invalid → ValidationException is thrown
```

---

## 🔍 Key Components and How It Works

### 1. **Your Command Class**

```csharp
public class Command : IRequest<Result<string>>
{
    public required CreateActivityDto ActivityDto { get; set; }
}
```

This command will be validated.

---

### 2. **CreateActivityValidator**

```csharp
public class CreateActivityValidator 
    : BaseActivityValidator<CreateActivity.Command, CreateActivityDto>
{
    public CreateActivityValidator() : base(x => x.ActivityDto) { }
}
```

This inherits from `AbstractValidator<Command>`, because `BaseActivityValidator<T, TDto>` does.

> ✅ This means: `CreateActivityValidator` becomes a `IValidator<Command>`.

---

### 3. **Dependency Injection: Registered Validators**

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();
```

This line tells the DI container to:

* Scan the assembly where `CreateActivityValidator` lives
* Automatically register **all classes that implement `IValidator<T>`**
* So `CreateActivityValidator` is registered as `IValidator<Command>`

---

### 4. **ValidationBehavior** intercepts MediatR pipeline

```csharp
x.AddOpenBehavior(typeof(ValidationBehavior<,>));
```

This registers a custom **pipeline behavior** that runs *before* MediatR handlers.

Inside your `ValidationBehavior<TRequest, TResponse>`:

```csharp
var validator = _validator; // Injected by DI
var validationResult = await validator.ValidateAsync(request);
```

Because your `Command` has a registered `IValidator<Command>` — i.e., your `CreateActivityValidator` — it is resolved and called automatically.

---

### 🔁 What actually calls `CreateActivityValidator`?

Indirectly:

* You never call `CreateActivityValidator` yourself.
* MediatR runs `ValidationBehavior<,>`.
* `ValidationBehavior` **asks the DI container** for `IValidator<Command>`.
* DI returns an instance of `CreateActivityValidator`.
* Then `CreateActivityValidator.Validate(...)` is executed.

---

### 🧠 Summary in Plain English

> When you call `Mediator.Send(...)`, MediatR’s pipeline runs all registered behaviors first.
> One of those behaviors is `ValidationBehavior`, which looks up any validators for the command you're sending.
>
> Since you've registered `CreateActivityValidator` with DI, and it matches the type `Command`, the validation logic is automatically applied.

---

Let me know if you want to trace what happens when the validation **fails** and how it bubbles up as a 400 Bad Request in the response.

*/