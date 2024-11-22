using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Регистрация сервисов
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    // Загружаем параметры из конфигурации
    options.ClientId = builder.Configuration["Authentication:Oidc:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Oidc:ClientSecret"];
    options.Authority = builder.Configuration["Authentication:Oidc:Authority"];
    options.CallbackPath = builder.Configuration["Authentication:Oidc:CallbackPath"];
    options.ResponseType = builder.Configuration["Authentication:Oidc:ResponseType"] ?? "code";
    
    // Указываем несколько значений scope, если нужно
    var scopes = builder.Configuration["Authentication:Oidc:Scope"]?.Split(' ') ?? new[] { "openid", "profile", "email" };
    foreach (var scope in scopes)
    {
        options.Scope.Add(scope);
    }

    options.RequireHttpsMetadata = bool.Parse(builder.Configuration["Authentication:Oidc:RequireHttpsMetadata"] ?? "true"); // Если будем хостить исправить на true

    
    options.SaveTokens = true; // сохранение токенов
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Конфигурация пайплайна запросов
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

app.UseAuthentication();  // Включение аутентификации
app.UseAuthorization();   // Включение авторизации

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
