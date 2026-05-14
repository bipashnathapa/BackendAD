using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Vehicle.Application.Interface.IRepositories;
using Vehicle.Application.Interface.IServices;
using Vehicle.Domain.Models;
using Vehicle.Infrastructure.Data;
using Vehicle.Infrastructure.Repositories;
using Vehicle.Infrastructure.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IEmailOtpService, EmailOtpService>();
builder.Services.AddScoped<ICustomerProfileService, CustomerProfileService>();
builder.Services.AddScoped<IAdminStaffService, AdminStaffService>();

builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerVehicleService, CustomerVehicleService>();
builder.Services.AddScoped<ICustomerServiceRepository, CustomerServiceRepository>();
builder.Services.AddScoped<ICustomerServiceService, CustomerServiceService>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();

builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IPartRepository, PartRepository>();
builder.Services.AddScoped<IPartService, PartService>();
builder.Services.AddScoped<ISaleInvoiceRepository, SaleInvoiceRepository>();
builder.Services.AddScoped<ISaleInvoiceService, SaleInvoiceService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IStaffServiceRequestsRepository, StaffServiceRequestsRepository>();
builder.Services.AddScoped<IStaffServiceRequestsService, StaffServiceRequestsService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<NotificationWorker>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        RoleClaimType = ClaimTypes.Role,
    };
});

builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var smtp = app.Configuration.GetSection(SmtpOptions.SectionName);
    app.Logger.LogInformation(
        "SMTP enabled={Enabled} host={Host} credentialsConfigured={HasCredentials}",
        smtp.GetValue<bool>("Enabled"),
        smtp["Host"],
        !string.IsNullOrWhiteSpace(smtp["User"]) && !string.IsNullOrWhiteSpace(smtp["Pass"]));

    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var r in new[] { "Admin", "Staff", "Customer" })
        if (!await roleMgr.RoleExistsAsync(r))
            await roleMgr.CreateAsync(new IdentityRole(r));

    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var adminEmail = "admin@vms.local";
    var admin = await userMgr.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "System Admin",
            UserRole = "Admin"
        };
        var created = await userMgr.CreateAsync(admin, "Admin@123");
        if (created.Succeeded) await userMgr.AddToRoleAsync(admin, "Admin");
    }
    else if (!await userMgr.IsInRoleAsync(admin, "Admin"))
    {
        await userMgr.AddToRoleAsync(admin, "Admin");
    }

    foreach (var u in userMgr.Users.ToList())
    {
        if (string.IsNullOrWhiteSpace(u.UserRole)) continue;
        if (!await userMgr.IsInRoleAsync(u, u.UserRole) && await roleMgr.RoleExistsAsync(u.UserRole))
            await userMgr.AddToRoleAsync(u, u.UserRole);
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
