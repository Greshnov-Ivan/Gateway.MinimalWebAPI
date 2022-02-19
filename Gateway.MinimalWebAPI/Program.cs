using Gateway.MinimalWebAPI;
using Gateway.MinimalWebAPI.Models;
using Gateway.MinimalWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddHttpClient();
builder.Services.Configure<ApisConfigure>(builder.Configuration.GetSection("ApisConfigure"));
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("yarp"));
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IAccountsService, AccountsService>();
builder.Services.AddTransient<IGrpcService, GrpcService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "I'am Gateway API!")
    .ExcludeFromDescription();
app.MapGet("/api/usersWithAccounts", async (IGrpcService grpcService) => await grpcService.GetUsersWithAccounts())
    .Produces<List<UserWithAccountViewModel>>(StatusCodes.Status200OK)
    .WithName("GetAllUsersWithAccounts")
    .WithTags("Getters");
app.MapPost("/api/initialData", async (IUsersService usersService) => await usersService.InitialData(new CancellationToken()))
    .Produces(StatusCodes.Status201Created)
    .WithName("InitialData")
    .WithTags("Creators");
app.MapDelete("/api/users", async (IUsersService usersService) =>
{
    await usersService.ClearUsers(new CancellationToken());
    return Results.NoContent();
})
    .Produces(StatusCodes.Status204NoContent)
    .WithName("DeleteAllUsers")
    .WithTags("Deleters");
app.MapDelete("/api/accounts", async (IAccountsService accountsService) =>
{
    await accountsService.ClearAccounts(new CancellationToken());
    return Results.NoContent();
})
    .Produces(StatusCodes.Status204NoContent)
    .WithName("DeleteAllAccounts")
    .WithTags("Deleters");

app.UseHttpsRedirection();
app.MapReverseProxy();
app.Run();