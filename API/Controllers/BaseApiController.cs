using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        //This is a lazy-loading property for IMediator
        private IMediator? _mediator;

        /*
        protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetService<IMediator>() ?? throw new InvalidOperationException("IMediator service is unavailable");

        Yes, this is a form of Property Dependency Injection, but implemented in a manual or lazy-loaded way.

        -This allows derived controllers to access the IMediator instance without needing to inject it through the constructor.
        -It also ensures that the IMediator instance is only created when it is first accessed, which can improve performance in some scenarios.
        -This is particularly useful in a base controller class like BaseApiController, where you want to provide access to 
        IMediator for all derived controllers without requiring each one to implement constructor injection.

        Step-by-Step Explanation
        1. protected IMediator Mediator =>
        This defines a read-only property named Mediator that can be used by derived controllers.

        2. _mediator ??= ...
        The ??= operator checks if _mediator is null.

        If it is null, it assigns the result of the right-hand side.

        If it’s already assigned, it simply returns the existing _mediator.

        3. HttpContext.RequestServices.GetService<IMediator>()
        This calls ASP.NET Core's built-in Dependency Injection (DI) container.

        It dynamically fetches the IMediator service from the container at runtime.

        4. ?? throw new InvalidOperationException(...)
        If the service is not found in the container, it throws an error right there.

        This prevents the app from silently continuing with a null _mediator.
        */

        //1) This is a lazy-loading property for IMediator.
        //2) Ensures that IMediator is only fetched once per request (first access).
        //3) Makes IMediator available to all derived API controllers without constructor injection.
        /* 
        This is a lazy-loading property for IMediator. why called lazy loaded?
        Great question!

        The property is called **"lazy-loaded"** because the `IMediator` instance is **not created or retrieved from the service container until it is first accessed**.

        ---

        ### 🔁 Lazy Loading Explained

        **Lazy loading** means:

        * *Don't create or fetch a resource until you actually need it.*

        ---

        ### 🧠 In your code:

        ```csharp
        protected IMediator Mediator =>
            _mediator ??= HttpContext.RequestServices.GetService<IMediator>()
                ?? throw new InvalidOperationException("IMediator service is unavailable");
        ```

        * When the controller is created, `_mediator` is `null`.
        * The first time you use `Mediator`, it checks:

        * Is `_mediator` null?

            * ✅ Yes → Get it from `HttpContext.RequestServices` and assign it.
            * ❌ No → Return the already stored `_mediator` (cached value).
        * Subsequent uses just return the cached `_mediator`.

        ---

        ### 🆚 Without Lazy Loading

        If this was not lazy-loaded, you'd use **constructor injection**, like:

        ```csharp
        private readonly IMediator _mediator;

        public ActivitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        ```

        Here, `IMediator` is injected **immediately** when the controller is constructed, even if the controller never uses it.

        ---

        ### ✅ Why use Lazy Loading?

        | Benefit           | Description                                                                                   |
        | ----------------- | --------------------------------------------------------------------------------------------- |
        | 🧠 Efficiency     | Only resolves the dependency when it’s actually needed.                                       |
        | 🔁 Flexibility    | Useful in base classes like `BaseApiController` where constructor injection isn’t convenient. |
        | 💥 Error handling | You can throw a clear error if the service is missing, instead of failing at startup.         |

        ---

        Let me know if you want to compare performance or choose between constructor injection and lazy loading!

        */
        protected IMediator Mediator =>
            _mediator ??= HttpContext.RequestServices.GetService<IMediator>()
                ?? throw new InvalidOperationException("IMediator service is unavailable");


        protected ActionResult HandleResult<T>(Result<T> result)
        {
            // If the activity is not found, return a NotFound response with the error message
            if (!result.IsSuceess && result.Code == 404) return NotFound();

            // If the activity is found, return it
            if (result.IsSuceess && result.Value != null) return Ok(result.Value);

            // If neither of the above conditions are met, return a BadRequest response with the error message
            return BadRequest(result.Error);
        }
    }
}
