// Purpose   : Ponto de entrada da aplicação VGC College.
//             Fase 1 — registo de serviços no DI container (ordem não importa).
//             Fase 2 — pipeline HTTP (ordem é crítica, cada middleware passa para o próximo).
// Consumed by: ASP.NET Core runtime.
// Layer     : Web — Entry Point

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VgcCollege.Data;
using VgcCollege.Data.Models;

// Configura o Serilog antes de qualquer outro serviço para capturar
// erros que ocorram durante o arranque da aplicação.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting VGC College application.");

    var builder = WebApplication.CreateBuilder(args);

    // Substitui o sistema de logging padrão do ASP.NET Core pelo Serilog.
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    
    // FASE 1 — Registo de serviços
    // Tudo aqui é configurado antes da aplicação arrancar.
    // A ordem entre os blocos desta fase não importa.
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
    
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });
    
    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    // DADOS INICIAIS
    // Executados após o build do app mas antes do pipeline HTTP.
    // O scope garante que os serviços são resolvidos e descartados correctamente.
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await DatabaseInitialiser.SeedAsync(context, userManager, roleManager);
    }


    
    // FASE 2 Pipeline HTTP
    // Cada middleware processa o pedido em sequência, de cima para baixo.
    // A ordem aqui é crítica — inverter dois middlewares pode
    // causar erros de autenticação ou segurança.
    
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception exception)
{
    
    Log.Fatal(exception, "VGC College application terminated unexpectedly.");
}
finally
{
    
    Log.CloseAndFlush();
}