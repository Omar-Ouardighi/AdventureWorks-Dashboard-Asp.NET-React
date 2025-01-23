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

    var Kpis = new KpisDto(
        Math.Round(totalSales, 2),
        totalOrders,
        Math.Round(totalSales / totalOrders, 2)
    );
    
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
                select new ProductCategorySalesDto(
                
                    g.Key.ParentProductCategoryName,
                    g.Sum(x => x.LineTotal)
                );

    var result = (await query.ToListAsync()).OrderByDescending(x => x.TotalAmount).ToList();
    return Results.Ok(result);
});

// GET sales data by product subcategory.
app.MapGet("/product-subcategory-sales", async (AdventureWorksLt2022Context context) =>
{
    var query = from c in context.ProductCategories
                join p in context.Products on c.ProductCategoryId equals p.ProductCategoryId
                join d in context.SalesOrderDetails on p.ProductId equals d.ProductId
                group d by new { c.Name } into g
                select new ProductCategorySalesDto(
                    g.Key.Name,
                    g.Sum(x => x.LineTotal)
                );

    var result = (await query.ToListAsync()).OrderByDescending(x => x.TotalAmount).ToList();
    return Results.Ok(result);
});



app.Run();
