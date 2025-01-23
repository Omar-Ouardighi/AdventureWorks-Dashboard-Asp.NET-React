using Microsoft.EntityFrameworkCore;
using Server.Dtos;
using Server.Models;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("AdventureWorks");
builder.Services.AddDbContext<AdventureWorksLt2022Context>(options =>
    options.UseSqlServer(connString));


var app = builder.Build();

app.MapGet("/kpis", async (AdventureWorksLt2022Context context) =>
{
    var totalSales = await context.SalesOrderDetails.SumAsync(s => s.LineTotal);
    var totalOrders = await context.SalesOrderDetails.SumAsync(s => s.OrderQty);

    var Kpis = new KpisDto()
    {
        TotalSales = Math.Round(totalSales, 2),
        TotalOrderQuantity = totalOrders,
        AverageMoneySpent = Math.Round(totalSales / totalOrders, 2)
    };
    
    return Results.Ok(Kpis);
});

// Get a single ProductModel by ID
app.MapGet("/productmodels/{id:int}", async (int id, AdventureWorksLt2022Context context) =>
{
    var productModel = await context.ProductModels
        .Include(p => p.ProductModelProductDescriptions)
        .Include(p => p.Products)
        .FirstOrDefaultAsync(p => p.ProductModelId == id);

    if (productModel == null)
    {
        return Results.NotFound(new { message = "ProductModel not found" });
    }

    return Results.Ok(productModel);
});

app.Run();
