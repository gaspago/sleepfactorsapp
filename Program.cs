using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using SleepFactorsApp.Components;
using SleepFactorsApp.Data;
using SleepFactorsApp.Hubs;
using SleepFactorsApp.Services;

var builder = WebApplication.CreateBuilder(args);

var databasePath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "sleepfactors.db");
Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
var dataProtectionKeysPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "keys");
Directory.CreateDirectory(dataProtectionKeysPath);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<SleepFactorsDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
    .SetApplicationName("SleepFactorsApp");

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<SleepFactorsDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/access-denied";
});

builder.Services.AddScoped<DailyLogService>();
builder.Services.AddScoped<SleepAnalysisService>();

builder.Services.AddSignalR();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
builder.Services.AddControllers();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SleepFactorsDbContext>();
    db.Database.EnsureCreated();
}

await IdentitySeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<SleepFactorsHub>("/hubs/sleep-factors");
app.MapControllers();


app.Run();
