using JapanApp.Data;
using JapanApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Đăng ký services TRƯỚC build
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<FestivalService>();

// 🔥 SESSION phải ở đây (TRƯỚC build)
builder.Services.AddSession();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// 🔥 SESSION phải ở đây
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();