using CSharpSoChiTieu.Data;
using CSharpSoChiTieu.Business.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

/***
 * Add Serviecs to businice
 * 
 */
builder.Services.AddScoped<CTDbContext>();

builder.Services.AddScoped<IAccountHandler, AccountHandler>();
builder.Services.AddScoped<IIncomeExpenseHandler, IncomeExpenseHandler>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    //options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
    });


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


/***
 * AutoMapper
 */
builder.Services.AddAutoMapper(typeof(UserAutoMapper));
builder.Services.AddAutoMapper(typeof(IncomeExpenseAutoMapper));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=IncomeExpense}/{action=Index}/{id?}");

app.Run();
