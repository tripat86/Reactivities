using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Queries
{
    public class GetActivityDetails
    {
        public class Query : IRequest<Result<Activity>>
        {
            public required string Id { get; set; }
        }

        // Here Result<Activity> is type of the response we expect from the query.
        // It will return an Activity object if successful, or an error message if not.
        public class Handler(AppDbContext context) : IRequestHandler<Query, Result<Activity>>
        {
            public async Task<Result<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities.FindAsync([request.Id], cancellationToken);

                if (activity == null) return Result<Activity>.Failure("Activity not found", 404);

                return Result<Activity>.Success(activity);
            }

        }
    }
}