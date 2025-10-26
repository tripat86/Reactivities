using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {
    }
    
    public class IsHostRequirementHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<IsHostRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            // Implementation to check if the user is the host of the activity
            // This typically involves checking the user's claims and the resource being accessed

            // If the user is the host, call context.Succeed(requirement);
            // Otherwise, do nothing (the requirement will not be met)

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return;

            var httpContext = httpContextAccessor.HttpContext;

            //Below line of code means if there is no id in route, just return
            // If there is an id, get its value and assign it to activityId
            if (httpContext?.GetRouteValue("id") is not string activityId) return;

            // If the user that is editing the activity is an attendee 
            var attendee = await dbContext.ActivityAttendees
                .SingleOrDefaultAsync(x => x.ActivityId == activityId && x.UserId == userId);

            if (attendee == null) return;

            if (attendee.IsHost)
                context.Succeed(requirement);
        }
    }
}