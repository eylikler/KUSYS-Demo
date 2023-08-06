using KUSYS_Demo.WebApplication.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllersWithViews();

var sessionIdleTimeout = builder.Configuration["AppSettings:SessionIdleTimeout"];

services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(sessionIdleTimeout));
});

services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";    //Java script ile POST iþlemlemi sýrasýnda RequestVerificationToken ý tanýmasý için eklendi.
});

services.AddHttpClient();

services.AddControllersWithViews().AddRazorRuntimeCompilation(); // Razor file compilation

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

//app.UseMiddleware<JwtTokenMiddleware>();

//app.UseCookiePolicy();
app.UseAuthorization();
app.UseAuthentication();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
