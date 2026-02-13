using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClientContactManager.Application.Common.Behaviors;
using ClientContactManager.Application.Common.Interfaces;
using ClientContactManager.Application.Commands.Clients;
using ClientContactManager.Application.Validators;
using ClientContactManager.Domain.Interfaces;
using ClientContactManager.Infrastructure.Persistence;
using ClientContactManager.Infrastructure.Persistence.Repositories;
using ClientContactManager.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

// Application services
builder.Services.AddScoped<IClientCodeGenerator, ClientCodeGenerator>();

// MediatR + validation pipeline
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<CreateClientCommand>());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssemblyContaining<CreateClientCommandValidator>();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure database and tables are created on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
