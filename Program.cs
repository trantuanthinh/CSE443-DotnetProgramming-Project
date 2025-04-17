using Microsoft.EntityFrameworkCore;
using Project;
using Project.AppContext;
using Project.Interfaces;
using Project.MailServices;
using Project.Repositories;
using Project.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddSession();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<MailService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IBorrowTransactionService, BorrowTransactionService>();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ItemRepository>();
builder.Services.AddScoped<BorrowTransactionRepository>();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

if (args.Contains("seeddata"))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        SeedData.Initialize(services);
    }
    return;
}

app.Run();
