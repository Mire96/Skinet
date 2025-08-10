using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();
//This step is used to seed the database with initial data
//As well as Migrating any changes to the DB
//So we don't have to do it manually through the terminal
try
{
    //When you're outside of Dependency Injection, it is your responsibility to create the scope and dispose of it
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    //This way we can get any class we need 
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    throw;
}

app.Run();
