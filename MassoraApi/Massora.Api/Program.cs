using Massora.Business.Mappings;
using Massora.Business.Services;
using Massora.Common.Repositories;
using Massora.DataAccess.Context;
using Massora.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- Servislerin Eklenmesi ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient(); // Bu satırı ekleyin

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()  // Herhangi bir kaynaktan gelen isteğe izin ver.
            .AllowAnyHeader()  // Herhangi bir başlığa (header) izin ver.
            .AllowAnyMethod());
});

// ===== JWT AUTHENTICATION AYARLARI BURADA BAŞLIYOR =====
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://localhost:5139"; // IdentityServer’ın URL’i
        options.RequireHttpsMetadata = false; // DEV ortamında false, PROD’da true
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization();

// Veritabanı, AutoMapper, Repository, Service kayıtları
builder.Services.AddDbContext<MassoraDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//addscope dinamik yap
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IPartnerCompanyService, PartnerCompanyService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IWorkHistoryService, WorkHistoryService>();
builder.Services.AddScoped<IVehicleFuelHistoryService, VehicleFuelHistoryService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();


var app = builder.Build();

// --- HTTP İstek Hattının (Pipeline) Yapılandırılması ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// UseRouting, diğer middleware'lerden önce gelmelidir (minimal API'lerde örtülü olarak çağrılır).
app.UseRouting();

app.UseCors("AllowAll");

// DÜZELTME (Kritik Hata): UseAuthentication, UseAuthorization'dan ÖNCE gelmelidir.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();