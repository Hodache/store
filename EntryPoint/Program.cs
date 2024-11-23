using AbstractServices;
using Domain.AbstractRepositories;
using BLL;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DatabaseDAL.Repositories;
using SwaggerClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DatabaseDAL;
using FilesDAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers().AddApplicationPart(typeof(Controllers).Assembly);

builder.Services.AddScoped<IStoreService, StoreService>();

var dalType = builder.Configuration["DalType"];

if (dalType == "Database")
{
    string connection = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<StoreDatabaseContext>(options =>
        options.UseSqlServer(connection));

    builder.Services.AddScoped<IStoreRepository, StoreRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IReserveRepository, ReserveRepository>();
} 
else if (dalType == "Csv")
{
    var csvPaths = builder.Configuration.GetSection("CsvPaths");

    var storesPath = csvPaths["Stores"];
    var productsPath = csvPaths["Products"];
    var reservesPath = csvPaths["Reserves"];

    builder.Services.AddScoped<IStoreRepository>(provider => new StoreCsvRepository(storesPath));
    builder.Services.AddScoped<IProductRepository>(provider => new ProductCsvRepository(productsPath));
    builder.Services.AddScoped<IReserveRepository>(provider => new ReserveCsvRepository(reservesPath));
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Store API",
        Version = "v1",
        Description = "Система сети магазинов"
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store API v1"));

app.MapControllers();

app.Run();