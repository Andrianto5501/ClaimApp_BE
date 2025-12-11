using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Project.Application.Services;
using Project.Application.Validators;
using Project.Domain.DTOs.Requests;
using Project.Domain.Interfaces.Repositories;
using Project.Domain.Interfaces.Services;
using Project.Infrastructure.Persistence;
using Project.Infrastructure.Repositories;
using Project.Worker;
using Project.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Repositories
builder.Services.AddScoped<IClaimRepository, ClaimRepository>();

// Services
builder.Services.AddScoped<IClaimService, ClaimService>();

// Background Worker
builder.Services.AddHostedService<ClaimAutoApproveWorker>();

builder.Services.AddMemoryCache();

// Validators
builder.Services.AddScoped<IValidator<AddClaimRequestDto>, ClaimAddValidator>();
builder.Services.AddScoped<IValidator<ClaimModifyStatusRequestDto>, ClaimModifyStatusValidator>();
builder.Services.AddValidatorsFromAssembly(typeof(ClaimService).Assembly);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
