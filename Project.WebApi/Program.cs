using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Project.Application.Services;
using Project.Application.Validators;
using Project.Domain.DTOs.Requests;
using Project.Domain.Interfaces.Repositories;
using Project.Domain.Interfaces.Services;
using Project.Infrastructure.Persistence;
using Project.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Cors
var corsOriginAll = "_corsOriginAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsOriginAll,
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Repositories
builder.Services.AddScoped<IClaimRepository, ClaimRepository>();

// Services
builder.Services.AddScoped<IClaimService, ClaimService>();

// Validators
builder.Services.AddScoped<IValidator<AddClaimRequestDto>, ClaimAddValidator>();
builder.Services.AddScoped<IValidator<ClaimModifyStatusRequestDto>, ClaimModifyStatusValidator>();

// FluentValidation auto pipeline
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsOriginAll);
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
