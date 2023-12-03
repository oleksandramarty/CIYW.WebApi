using System.Globalization;
using System.Text.Json;
using CIYW.ClientApi.Middleware;
using CIYW.Domain;
using CIYW.Domain.Initialization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

namespace CIYW.Kernel.Extensions;

    public static class WebAppExtension
    {
        public static async Task RemoveDatabaseAsync(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    await context.Database.EnsureDeletedAsync();
                }
            }
        }

        public static async Task UpdateDatabaseAsync(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    await context.Database.MigrateAsync();
                }
            }
        }

        public static void AddCorsSupport(this IServiceCollection services, string? origins)
        {
          services.AddCors(options =>
          {
            options.AddPolicy("CorsPolicy",
              builder => builder.WithOrigins(origins.Split(","))
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
          });
        }
        
        public static void InitDatabase(this IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices
                       .GetRequiredService<IServiceScopeFactory>()
                       .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    DbInitializer.Initialize(context, isProd);
                }
            }
        }

        public static void ConfigureApplicationLocalization(this IApplicationBuilder app)
        {
            //var english = "en-US";
            const string defCult = "en-US";
            var defaultRequestCulture = new RequestCulture(defCult, defCult);
            var supportedCultures = new List<CultureInfo>
            {
                new("en-US"),
                new("ru-RU")
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = defaultRequestCulture,
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            //RequestCultureProvider requestProvider = options.RequestCultureProviders.OfType<AcceptLanguageHeaderRequestCultureProvider>().First();
            //requestProvider.Options.DefaultRequestCulture = englishRequestCulture;

            RequestCultureProvider requestProvider =
                options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().First();
            options.RequestCultureProviders.Remove(requestProvider);

            app.UseRequestLocalization(options);
        }

        public static IApplicationBuilder UseRequestTimeHeader(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTimeMiddleware>();
        }
    }