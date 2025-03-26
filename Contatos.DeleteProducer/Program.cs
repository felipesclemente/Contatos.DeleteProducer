using Contatos.DataContracts.Commands;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MassTransit;
using Serilog;
using Serilog.Events;

namespace Contatos.DeleteProducer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(path: "Logs/DeleteProducerLogs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Services.AddSerilog();

                builder.Services.AddControllers();
                builder.Services.AddOpenApi();

                builder.Services.AddMassTransit(x =>
                {
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("rabbitmq");

                        //cfg.Host("localhost", "/", h =>
                        //{
                        //    h.Username("guest");
                        //    h.Password("guest");
                        //});

                        cfg.ConfigureEndpoints(context);
                    });
                });

                EndpointConvention.Map<ApagarContato>(new Uri($"queue:ApagarContato"));

                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                }

                app.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                });
                app.MapHealthChecks("/health/live", new HealthCheckOptions());

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal($"O serviço Contatos.DeleteProducer encerrou inesperadamente. Exception: {ex.GetType()}. Message: {ex.Message}.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
