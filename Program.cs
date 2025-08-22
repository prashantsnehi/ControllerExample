using ControllerExample.Filters;
using ControllerExample.Services;
using Microsoft.Extensions.Logging.Configuration;

var builder = WebApplication.CreateBuilder(args);

// ILogger Configurations. 
builder.Host.ConfigureLogging(loggerConfiguration =>
{
    loggerConfiguration.ClearProviders();
    loggerConfiguration.AddDebug();
    loggerConfiguration.AddEventLog();
    loggerConfiguration.AddConsole();
});

//builder.Logging.AddDebug();
//builder.Logging.AddConsole();
//builder.Logging.AddEventLog();

// Add services to the container.

// add custom filter as global filter
builder.Services.AddControllersWithViews(options =>
{
    var logger = builder.Services?.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderFilter>>();
    options.Filters.Add(new ResponseHeaderFilter(logger, "X-Developer-Info", "Prashant"));
});
builder.Services.Add(new ServiceDescriptor(
    typeof(IPersonService),
    typeof(PersonService),
    ServiceLifetime.Singleton
));

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
    options.RequestHeaders.Add("User-Agent");
    options.ResponseHeaders.Add("Server");
    options.RequestHeaders.Add("Accept");
    options.RequestHeaders.Add("Content-Type");
    options.ResponseHeaders.Add("Content-Type");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

Rotativa.AspNetCore.RotativaConfiguration.Setup(
    app.Environment.WebRootPath,
    //"wwwroot",
    // app.Environment.ContentRootPath,
    wkhtmltopdfRelativePath: "Rotativa"
);

app.Use(async (context, next) =>
{
    context.Response.Headers["Server"] = "Prashant";
    await next(context);
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// add httpLoging
app.UseHttpLogging();
app.Run();