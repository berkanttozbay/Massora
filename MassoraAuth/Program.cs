using IdentityServer.Configurations;

using IdentityServer.Data;

using IdentityServer.Models;

using Microsoft.AspNetCore.Builder;

using Microsoft.AspNetCore.DataProtection;

using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Hosting;

using System.Reflection;



var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5139");



// --- SERVİSLER ---



builder.Services.AddControllersWithViews();

builder.Services.AddDataProtection()

  .PersistKeysToFileSystem(new DirectoryInfo(@"./keys"))

  .SetApplicationName("IdentityServer");



builder.Services.AddDbContext<ApplicationDbContext>(options =>

  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()

  .AddEntityFrameworkStores<ApplicationDbContext>()

  .AddDefaultTokenProviders();



builder.Services.ConfigureApplicationCookie(options =>

{
    // options.Cookie.SameSite = SameSiteMode.Lax;

    // Cookie'nin siteler arası yönlendirmelerde gönderilmesini sağlar.

     options.Cookie.SameSite = SameSiteMode.None;



    // SameSiteMode.None, cookie'nin "Secure" (sadece HTTPS üzerinden) olmasını gerektirir.

    // Geliştirme ortamında (localhost) bu genellikle sorun olmaz ama canlı ortamda sitenizin HTTPS olması şarttır.

     options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

});

// ▼▼▼ EN KRİTİK DÜZELTME ▼▼▼

builder.Services.AddIdentityServer()

// 1. ADIM: KALICI BİR GELİŞTİRME ANAHTARI OLUŞTURUN

// Bu, her yeniden başlatmada aynı anahtarın kullanılmasını sağlar.

    .AddDeveloperSigningCredential()

  .AddAspNetIdentity<ApplicationUser>()

  .AddConfigurationStore(options =>

  {

      options.ConfigureDbContext = b =>

        b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),

          sql => sql.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));

  })

// 2. ADIM: IN-MEMORY YERİNE TEKRAR VERİTABANINI KULLANIN

// .AddInMemoryPersistedGrants() satırını silin ve .AddOperationalStore'u etkinleştirin

    .AddOperationalStore(options =>

    {

        options.ConfigureDbContext = b =>

          b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),

            sql => sql.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));

    });

//.AddOperationalStore(options =>

//{

//    options.ConfigureDbContext = b =>

//        b.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),

//            sql => sql.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));

//});



// ▲▲▲ DÜZELTME BİTTİ ▲▲▲



builder.Services.AddCors(options =>

{

    options.AddPolicy("AllowAngularAndApi",

      policyBuilder => policyBuilder.WithOrigins("http://localhost:4200", "http://localhost:5260", "http://localhost:8081")

               .AllowAnyHeader()

               .AllowAnyMethod()

               .AllowCredentials());

});



var app = builder.Build();



// --- HTTP ISTEK HATTI (PIPELINE) ---



if (app.Environment.IsDevelopment())

{

    app.UseDeveloperExceptionPage();

}

else

{

    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();

}



app.UseHttpsRedirection();

app.UseStaticFiles();



app.UseRouting();

app.UseCors("AllowAngularAndApi");



// MIDDLEWARE SIRALAMASI DÜZELTİLDİ

app.UseAuthentication();

app.UseIdentityServer();

app.UseAuthorization();



// ENDPOINT EŞLEMESİ DÜZELTİLDİ

app.UseEndpoints(endpoints =>

{

    endpoints.MapControllerRoute(

      name: "default",

      pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllers();

});



// Veri tohumlama

using (var scope = app.Services.CreateScope())

{

    var services = scope.ServiceProvider;

    // SeedData.cs'nin çalıştığından emin olun (Adım 2'ye bakın)

    await SeedData.InitializeAsync(services);

}



app.Run();