using Microsoft.EntityFrameworkCore;
using QuartzNetJobMvc.Extensions;
using QuartzNetJobMvc.Extensions.Email;
using QuartzNetJobMvc.Extensions.Logging;
using QuartzNetJobMvc.Extensions.Quartz;

var builder = WebApplication.CreateBuilder(args);

// Configure the services using extension methods
builder.ConfigureSerilog();
builder.ConfigureFluentEmail();
builder.Services.ConfigureQuartz();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();