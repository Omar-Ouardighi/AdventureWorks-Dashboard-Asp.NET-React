using Microsoft.EntityFrameworkCore;
using Server.Dtos;
using Server.Models;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("AdventureWorks");
builder.Services.AddDbContext<AdventureWorksLt2022Context>(options =>
    options.UseSqlServer(connString));


var app = builder.Build();

// GET Key Performance Indicators (KPIs)
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


// GET sales data by product category.
app.MapGet("/product-category-sales", async (AdventureWorksLt2022Context context) =>
{
    var query = from c in context.ProductCategories
                join s in context.VGetAllCategories on c.ProductCategoryId equals s.ProductCategoryId
                join p in context.Products on c.ProductCategoryId equals p.ProductCategoryId
                join d in context.SalesOrderDetails on p.ProductId equals d.ProductId
                group d by new { s.ParentProductCategoryName} into g
                select new ProductCategorySalesDto
                {
                    
                    CategoryName = g.Key.ParentProductCategoryName,
                    TotalAmount = g.Sum(x => x.LineTotal)
                };

    var result = await query.OrderBy(x => x.TotalAmount).ToListAsync();
    return Results.Ok(result);
});


app.Run();
