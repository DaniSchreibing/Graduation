using Application.Interfaces;
using Application.Use_cases;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var allowedHosts = "_allowedHosts";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedHosts,
        policy =>
        {
			policy.AllowAnyOrigin();
        });
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IConverter, Converter>();
builder.Services.AddScoped<ICsv, Csv>();
builder.Services.AddScoped<IInputValidation, InputValidation>();
builder.Services.AddScoped<IPaths, PathUC>();
builder.Services.AddScoped<IReaderUC, Reader>();
builder.Services.AddScoped<ISolver, Solver>();
builder.Services.AddScoped<IStatus, Status>();
builder.Services.AddScoped<IValidate, Validate>();


builder.Services.AddRateLimiter(options =>
{
	options.AddFixedWindowLimiter("FixedPolicy", opt =>
	{
		opt.Window = TimeSpan.FromMinutes(1);
		opt.PermitLimit = 100;
		opt.QueueLimit = 2;
		opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
	});
}

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
