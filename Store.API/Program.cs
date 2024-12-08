using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Store.API.Controllers;
using Store.BLL.Interfaces;
using Store.BLL.Services;
using Store.DAL;
using Store.DAL.Repositories.Interfaces;
using Store.DAL.Repositories.Database;
using Store.DAL.Repositories.Files;

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

    builder.Services.AddScoped<IShopRepository, ShopRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IReserveRepository, ReserveRepository>();
}
else if (dalType == "Csv")
{
    var csvPaths = builder.Configuration.GetSection("CsvPaths");

    var storesPath = csvPaths["Stores"];
    var productsPath = csvPaths["Products"];
    var reservesPath = csvPaths["Reserves"];

    builder.Services.AddScoped<IShopRepository>(provider => new ShopCsvRepository(storesPath));
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