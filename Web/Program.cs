using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Web;
using System.Net;
using Web.DbConnection;
using Web.IRepository;
using Web.Repository;
using Web.Controllers;
using Web.Util;
using Web.Services;
using ProtoBuf.Meta;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddScoped<UploadFile>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/forbidden";
    options.LogoutPath = "/logout";  // Add this line for logout path
}).AddGoogle(googleOptions =>
{
    googleOptions.ClientId = "886015841712-v16ddvquo185i7oieqhi1j0if4n2uef7.apps.googleusercontent.com";
    googleOptions.ClientSecret = "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";
    googleOptions.Scope.Add("email");
}).AddDiscord(options =>
{
    options.ClientId = "1179457452387860550";
    options.ClientSecret = "N_0TFhzHjYhbxVa_VKH_bqPswzHWwLMd";
    options.Scope.Add("email");
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpClient();

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<MarkReportServices>();
builder.Services.AddScoped<UploadFile>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddDbContext<WebContext>
    (opt => opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();
builder.Services.AddSession();
var app = builder.Build();

app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//app.UseStatusCodePagesWithRedirects("/Index");
app.UseStatusCodePagesWithReExecute("/Status/{0}");

app.UseStatusCodePages(context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == (int)HttpStatusCode.Forbidden)
    {
        response.Redirect("/forbidden");
    }

    return Task.CompletedTask;
});


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<PostHub>("/postHub");
    endpoints.MapHub<NotificationHub>("/notificationHub");
    endpoints.MapHub<ChatHub>("/chatHub");
    endpoints.MapHub<UserUpdateHub>("/userUpdateHub");
    endpoints.MapRazorPages();
    endpoints.MapControllers();
});

app.Run();
