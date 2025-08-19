using ControllerExample.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Add(new ServiceDescriptor(
    typeof(IPersonService),
    typeof(PersonService),
    ServiceLifetime.Singleton
));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

Rotativa.AspNetCore.RotativaConfiguration.Setup(
    // app.Environment.WebRootPath,
    "wwwroot",
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


app.Run();
