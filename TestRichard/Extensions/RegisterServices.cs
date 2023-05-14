using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;
using System.Runtime.CompilerServices;
using TestRichard.Constants;
using TestRichard.Filters;
using TestRichard.Models;
using TestRichard.Services;

namespace TestRichard.Extensions
{
    public static class RegisterServices
    {

        public static IServiceCollection AddCORS(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(AppConstants.CORS, policy =>
                {
                    policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
                });
            });
            return services;
        }

        public static IServiceCollection RegiserService(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<ValidateModelStateFilter>();
            services.AddTransient<IDownloadService, DownloadService>();

            //register fluent validation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            return services;
        }
        public static IServiceCollection RegiserAppsetting(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FirebaseSetting>(configuration.GetSection(nameof(FirebaseSetting)));
            return services;
        }
    }
}
    /*Note:
     * when modelstate is invalid, fluent validation automatically returns its response.
     * if you want to return customize response instead, create ActionFilter like above 'ValidateModelStateFilter'
     * implement it in your controller [ValidateModelStateFilter()] or [ServiceFilter(typeof(ValidateModelStateFilter))]      
     * and it will not work unless you supress APIBehaviour like below
     *   builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)

     * */

