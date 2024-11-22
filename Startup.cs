using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyMvcApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Этот метод добавляет сервисы в контейнер
        public void ConfigureServices(IServiceCollection services)
        {
            // Настройка аутентификации с использованием Cookie и OpenID Connect
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie() // Добавляем поддержку Cookie
            .AddOpenIdConnect(options => // Настройка OpenID Connect
            {
                options.Authority = Configuration["Authentication:Keycloak:Authority"];
                options.ClientId = Configuration["Authentication:Keycloak:ClientId"];
                options.ClientSecret = Configuration["Authentication:Keycloak:ClientSecret"];
                options.CallbackPath = Configuration["Authentication:Keycloak:CallbackPath"];
                options.ResponseType = Configuration["Authentication:Keycloak:ResponseType"];
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                // Сохранение токенов после успешной авторизации
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
            });

            services.AddControllersWithViews(); // Добавляем поддержку контроллеров и представлений
        }

        // Этот метод настраивает middleware, обрабатывающее запросы
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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

            app.UseAuthentication(); // Middleware для аутентификации
            app.UseAuthorization();  // Middleware для авторизации

            // Настройка маршрутов
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
