
using FluentValidation;
using FluentValidation.AspNetCore;
using Google.Api;
using System.Reflection;
using System.Text.Json.Serialization;
using TestRichard.Constants;
using TestRichard.Extensions;

namespace TestRichard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
        

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
                .AddJsonOptions(option =>
                {
                    option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                }); 
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCORS();
            builder.Services.RegiserAppsetting(builder.Configuration);
            builder.Services.RegiserService();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors(AppConstants.CORS);


            app.MapControllers();

            app.Run();
        }
    }
}