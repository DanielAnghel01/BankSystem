using BankSystem.Server.Infrastructure.DataAccess;
using BankSystem.Server.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddDbContext<BankDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("dbString"));
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
