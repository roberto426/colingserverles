using Coling.API.Afiliados;
using Coling.API.Afiliados.Contratos;
using Coling.API.Afiliados.Implementacion;
using Coling.Utilitarios.Attributes;
using Coling.Utilitarios.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

var ensamblado = Assembly.GetExecutingAssembly();
var tipoAtributo = typeof(ColingAuthorizeAttribute);
IList<string> DefaultFunctions = new List<string>();
var host = new HostBuilder()

    .ConfigureServices(services =>
    {
        var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables()
           .Build();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddDbContext<Contexto>(options => options.UseSqlServer(
                     configuration.GetConnectionString("cadenaConexion")), ServiceLifetime.Scoped);

        services.AddScoped<IPersonaLogic, PersonaLogic>();
        services.AddScoped<IDireccionLogic, DireccionLogic>();
        services.AddScoped<ITelefonoLogic, TelefonoLogic>();
        services.AddScoped<ITipoSocialLogic, TipoSocialLogic>();
        services.AddScoped<IPersonaTipoSocialLogic, PersonaTipoSocialLogic>();
        services.AddScoped<IAfiliadoLogic, AfiliadoLogic>();
        services.AddScoped<IIdiomaLogic, IdiomaLogic>();
        services.AddScoped<IAfiliadoIdiomaLogic, AfiliadoIdiomaLogic>();
        services.AddSingleton<JwtMiddleware>();
        services.AddSingleton<AutorizacionRolMiddleware>();
        services.Configure<KestrelServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });
    }).ConfigureFunctionsWebApplication(x =>
    {

    })
    .Build();
var lista = GetAuthorizedFunctions(ensamblado, tipoAtributo);
host.Services.GetService<AutorizacionRolMiddleware>()?.SetFunctionAutorizadas(lista);
host.Run();


Dictionary<string, string> GetAuthorizedFunctions2(Type containerType, Type? attributeType)
{
    var authorizedFunctions = new Dictionary<string, string>();

    if (attributeType == null) { return authorizedFunctions; }

    Type functionType = typeof(FunctionAttribute);
    var methods = ObtenerMetdosConAtributoPersonalizado(containerType, functionType);
    foreach (var method in methods)
    {
        var function = method.GetCustomAttribute(attributeType, true) as ColingAuthorizeAttribute;
        if (function is null)
        {
            continue;
        }

        var functionA = (FunctionAttribute?)method.GetCustomAttribute(functionType, true);
        if (functionA is null)
        {
            continue;
        }
        authorizedFunctions.Add(functionA.Name, function.Rols);
    }

    return authorizedFunctions;
}

MethodInfo[] ObtenerMetdosConAtributoPersonalizado(Type type, Type attributeType)
{
    return type.GetMethods()
        .Where(m => m.GetCustomAttributes(attributeType, inherit: true).Any())
        .ToArray();
}

Dictionary<string, string> GetAuthorizedFunctions(Assembly containerAssembly, Type? attributeType)
{
    if (containerAssembly == null) throw new ArgumentNullException(nameof(containerAssembly));
    if (attributeType == null) throw new ArgumentNullException(nameof(attributeType));

    var result = new Dictionary<string, string>();
    foreach (var item in containerAssembly.ExportedTypes)
    {
        var resulItem = GetAuthorizedFunctions2(item, attributeType);
        result.AddRange(resulItem);
    }

    return result;
}