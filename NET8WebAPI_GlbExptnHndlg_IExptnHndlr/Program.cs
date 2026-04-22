using Microsoft.EntityFrameworkCore;
using NET8WebAPI_GlbExptnHndlg_IExptnHndlr.GlobleExptnHandlers;
using NET8WebAPI_GlbExptnHndlg_IExptnHndlr.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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

//Register the Service: Use the AddExceptionHandler extension method to add your custom handler
//to the Dependency Injection (DI) container.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Enable the Middleware: You must call UseExceptionHandler after the application is built
//to actually activate the handling pipeline.
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
