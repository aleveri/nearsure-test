using ConwayGameMinimalService.Data;
using ConwayGameMinimalService.Entities;
using ConwayGameMinimalService.Filters;
using ConwayGameMinimalService.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddDbContext<ConwayGameDB>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add new board
app.MapPost("Boards/{id}", async (int id, int x, int y, IGameService service) =>
{
    await service.Start(new GameOfLife(id, x, y));
}).AddEndpointFilter<ValidIdentityFilter>(); ;

// Allows uploading a new board state, returns id of board
app.MapPut("Boards/{id}", async (int id, int x, int y, IGameService service) =>
{
    if (!await service.Exist(id))
        Results.NotFound(id);

    await service.Update(new GameOfLife(id, x, y));

    return Results.Ok(id);
})
.AddEndpointFilter<ValidIdentityFilter>()
.AddEndpointFilter<ValidPositionsFilter>();

//Get next state for board, returns next state
app.MapGet("BoardNextState/{id}", async (int id, IGameService service) =>
{
    if (!await service.Exist(id))
        Results.NotFound(id);

    GameOfLife game = await service.Get(id);
    game = await service.SpawnNextGeneration(game);

    return Results.Ok(game);
}).AddEndpointFilter<ValidIdentityFilter>();

//Gets x number of states away for board
app.MapGet("BoardStates/{id}", async (int id, int generations, IGameService service) =>
{
    if (!await service.Exist(id))
        Results.NotFound(id);

    GameOfLife game = await service.Get(id);
    return Results.Ok(await service.SpawnNextGenerations(game, generations));
}).AddEndpointFilter<ValidIdentityFilter>();

//Gets final state for board. If board doesn't go to conclusion after x number of attempts, returns error
app.MapGet("BoardFinalState/{id}", async (int id, IGameService service) =>
{
    if (!await service.Exist(id))
        Results.NotFound(id);

    GameOfLife game = await service.Get(id);

    if (await service.GetFinalState(game))
        return Results.Ok();
    else
        return Results.Problem();

}).AddEndpointFilter<ValidIdentityFilter>();

app.Run();