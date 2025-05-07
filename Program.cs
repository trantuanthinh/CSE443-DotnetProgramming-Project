using Microsoft.EntityFrameworkCore;
using Project;
using Project.AppContext;
using Project.AutoJobs;
using Project.AutoMapperHelper;
using Project.Core;
using Project.Interfaces;
using Project.MailServices;
using Project.Repositories;
using Project.Services;
using Quartz;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddSession();
builder.Services.AddSignalR();

builder.Services.AddControllersWithViews();
builder.Services.AddQuartz(store =>
{
    store.UsePersistentStore(option =>
    {
        option.UseProperties = true;
        option.RetryInterval = TimeSpan.FromSeconds(15);
        option.UseMySql(connectionString);
        option.UseSystemTextJsonSerializer();
    });

    var jobKey = new JobKey("CheckOverDue");

    store.AddJob<CheckOverDue>(opts => opts.WithIdentity(jobKey));

    store.AddTrigger(opts =>
        opts.ForJob(jobKey)
            .WithIdentity("CheckOverDue-trigger")
            .WithSimpleSchedule(x => x.WithIntervalInSeconds(15).RepeatForever())
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<BaseController>();
builder.Services.AddScoped<MappingHelper>();
builder.Services.AddSingleton<MailService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IBorrowTransactionService, BorrowTransactionService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ItemRepository>();
builder.Services.AddScoped<BorrowTransactionRepository>();
builder.Services.AddScoped<ConversationRepository>();
builder.Services.AddScoped<MessageRepository>();

builder.Services.AddScoped<CheckOverDue>();

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
