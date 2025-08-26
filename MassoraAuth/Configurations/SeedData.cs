using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using IdentityServer.Models;
using System;
using IdentityServer.Data;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Bu using ifadesini ekleyin

namespace IdentityServer.Configurations
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var context = serviceProvider.GetRequiredService<ConfigurationDbContext>();

            // --- Kullanıcı ve Rol Tohumlama (Bu kısım aynı kalıyor) ---
            var adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new ApplicationRole(adminRole));
            }

            if (await userManager.FindByNameAsync("admin") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@massora.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin123*");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }
                else
                {
                    throw new Exception("Admin kullanıcısı oluşturulamadı: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // --- IdentityServer Konfigürasyon Tohumlama (GÜNCELLENMİŞ MANTIK) ---

            // Clients
            foreach (var client in Clients.GetClients())
            {
                // Veritabanında bu ClientId ile bir kayıt var mı diye kontrol et
                if (!await context.Clients.AnyAsync(c => c.ClientId == client.ClientId))
                {
                    context.Clients.Add(client.ToEntity());
                }
            }
            await context.SaveChangesAsync();


            // IdentityResources
            foreach (var resource in IdentityResourcesConfig.GetIdentityResources())
            {
                if (!await context.IdentityResources.AnyAsync(ir => ir.Name == resource.Name))
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
            }
            await context.SaveChangesAsync();


            // ApiScopes
            foreach (var scopeItem in ApiResources.GetApiScopes())
            {
                if (!await context.ApiScopes.AnyAsync(s => s.Name == scopeItem.Name))
                {
                    context.ApiScopes.Add(scopeItem.ToEntity());
                }
            }
            await context.SaveChangesAsync();

            // ApiResources
            foreach (var resource in ApiResources.GetApiResources())
            {
                if (!await context.ApiResources.AnyAsync(ar => ar.Name == resource.Name))
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
            }
            await context.SaveChangesAsync();
        }
    }
}