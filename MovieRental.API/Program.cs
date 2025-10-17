using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MovieRental.API.Filters;
using MovieRental.Domain.DTOs;
using MovieRental.Domain.Entities;
using MovieRental.Domain.Features;
using MovieRental.Domain.Interfaces;
using MovieRental.Domain.PaymentProviders;
using MovieRental.Infrastructure.Data;
using MovieRental.Infrastructure.Repositories;
using MovieRental.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MovieRental API", Version = "v1" });
});

// Register DbContext with SQLite
builder.Services.AddDbContext<MovieRentalDbContext>(options =>
    options.UseSqlite("Data Source=movieRental.db"));

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register repositories
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();

// Register application services
builder.Services.AddScoped<IMovieFeatures, MovieFeatures>();
builder.Services.AddScoped<IRentalFeatures, RentalFeatures>();

// Register payment providers
builder.Services.AddScoped<PayPalProvider>();
builder.Services.AddScoped<MbWayProvider>();
builder.Services.AddScoped<CreditCardProvider>();
builder.Services.AddScoped<IPaymentProviderFactory, PaymentProviderFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();