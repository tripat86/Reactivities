using API.Middleware;
using Application.Activities.Queries;
using Application.Activities.Validators;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt =>
{
  var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
  opt.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddDbContext<AppDbContext>(opt =>
{
  opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
builder.Services.AddMediatR(x =>
{
  x.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>();
  x.AddOpenBehavior(typeof(ValidationBehavior<,>));
  //Angle brackets are empty because we dont know the request and response types at this point
  // AddOpenBehavior registers the ValidationBehavior for all requests
  // that implement IRequest<TResponse> in the assembly containing GetActivityList.Handler
  /*
  Following shows how request comes to ActivityController and how it is validated:
  ActivitiesController receives HTTP POST request
    ↓
  Controller receives CreateActivityDto
    ↓
  You create new CreateActivity.Command { ActivityDto = dto }
    ↓
  MediatR sends it
    ↓
  ValidationBehavior<CreateActivity.Command, Result<string>> runs
    ↓
  DI resolves IValidator<CreateActivity.Command> → CreateActivityValidator
    ↓
  Validator validates ActivityDto inside command
    ↓
  If valid → continue to CreateActivity.Handler
  If invalid → throw ValidationException

  */

});

builder.Services.AddScoped<IUserAccessor, UserAccessor>();
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();
builder.Services.AddTransient<ExceptionMiddleware>();
builder.Services.AddIdentityApiEndpoints<User>(opt =>
{
  opt.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddAuthorization(opt =>
{
  opt.AddPolicy("IsActivityHost", policy =>
  {
    policy.Requirements.Add(new IsHostRequirement());
  });
});
builder.Services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();


var app = builder.Build();


// Configure the HTTP request pipeline. ordering is important here
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
  .AllowCredentials()
    .WithOrigins("http://localhost:3000", "https://localhost:3000"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<User>(); // api/login, api/register, etc.

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
  var context = services.GetRequiredService<AppDbContext>();
  var userManager = services.GetRequiredService<UserManager<User>>();

  await context.Database.MigrateAsync();
  await DbInitializer.SeedData(context, userManager);
}
catch (Exception ex)
{
  var logger = services.GetRequiredService<ILogger<Program>>();
  logger.LogError(ex, "An error occurred during migration.");
}

app.Run();
