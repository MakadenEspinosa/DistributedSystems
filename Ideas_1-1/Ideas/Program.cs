using Ideas.Models;
using Ideas.Services;

var builder = WebApplication.CreateBuilder(args);

// https://aka.ms/aspnet/openapi - In case Documentation needs to be referenced again for future use
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IdeaService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();

// GET all
app.MapGet("/api/ideas", (IdeaService service) =>
{
    return Results.Ok(service.GetAll());
})
.WithName("GetAllIdeas");

// GET idea by ID
app.MapGet("/api/ideas/{id}", (int id, IdeaService service) =>
{
    var idea = service.GetById(id);
    return idea is not null ? Results.Ok(idea) : Results.NotFound();
})
.WithName("GetIdeaById");

// POST 
app.MapPost("/api/ideas", (Idea idea, IdeaService service) =>
{
    var created = service.Add(idea);
    return Results.Created($"/api/ideas/{created.Id}", created);
})
.WithName("CreateIdea");

// PUT 
app.MapPut("/api/ideas/{id}", (int id, Idea idea, IdeaService service) =>
{
    var success = service.Update(id, idea);
    return success ? Results.NoContent() : Results.NotFound();
})
.WithName("UpdateIdea");

//PATCH 
app.MapPatch("/api/ideas/{id}", (int id, Idea updatedIdea, IdeaService service) =>
{
    var success = service.Update(id, updatedIdea);
    return success ? Results.NoContent() : Results.NotFound();
})
.WithName("PatchIdea");

// DELETE 
app.MapDelete("/api/ideas/{id}", (int id, IdeaService service) =>
{
    var success = service.Delete(id);
    return success ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteIdea");

app.Run();

// Citing and Attribution 
// This code is generated based on the ASP.NET Core Web API template provided by Microsoft's ASP.NET Core framework template.
// This can be replicated by creating a new ASP.NET Core Web API project using the .NET CLI or Visual Studio, which includes similar boilerplate code for setting up a basic web API with a weather forecast endpoint.
// terminal command to create a new project:
// dotnet new webapi -n Ideas_1-1

// Project structure was influenced by the structure used to teach restful API development in Developing Scalable Web Applications (Java Path)
// Syntax and code patterns are based on standard practices in ASP.NET Core Web API development as documented in the official Microsoft documentation.
// Help and refrences provided by ChatGPT based on knowledge of ASP.NET Core and RESTful API design principles.
