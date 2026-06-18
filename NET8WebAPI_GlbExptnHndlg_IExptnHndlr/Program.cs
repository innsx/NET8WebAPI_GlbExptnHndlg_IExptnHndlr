using Microsoft.EntityFrameworkCore;
using NET8WebAPI_GlbExptnHndlg_IExptnHndlr.GlobleExptnHandlers;
using NET8WebAPI_GlbExptnHndlg_IExptnHndlr.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TestDbContext>(options =>
{
    var dbConnStrng = builder.Configuration.GetConnectionString("DbConnStrg");
    options.UseSqlServer(dbConnStrng);
});

//Add Problem Details (Recommended): Register the Problem Details service
//to generate standardized RFC 7807 error responses.
builder.Services.AddProblemDetails();


//This AddExceptionHandler() method registers a custom error handler in ASP.NET Core (.NET 8+).
//The system registers this service with a Singleton lifetime.
//Do not inject scoped dependencies directly into its constructor
//To make builder.Services.AddExceptionHandler<GlobalExceptionHandler>(); work,
//you must also add the exception handling middleware to the HTTP request pipeline
//using app.UseExceptionHandler(); in your Program.cs file see below in the Middlware pipeline.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Activate the Exception Handler Middleware at the beginning of the pipeline
//Enable the Middleware: You must call UseExceptionHandler after the application is built
//to actually activate the handling pipeline.
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
