using System.Diagnostics;
using ikoLite;
using ikoLite.Messaging;
using ikoLite.Messaging.Memory;
using ikoLite.Models;
using ikoLite.Services;
using ikoLite.Services.Booking;
using ikoLite.Services.Consumers;
using ikoLite.Services.Saga;
using MassTransit;
using MassTransit.Audit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



try
{
    ConnectionBuilder(args).Build().Run();
}
catch (Exception ex)
{
    Console.WriteLine($"[Exeption: {ex.Data}] {ex.Message}");
    Console.WriteLine($"[Help link] {ex.HelpLink}");
}


static IHostBuilder ConnectionBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMassTransit(x =>
            {
                services.AddSingleton<IMessageAuditStore, AuditStore>();

                var serviceProvider = services.BuildServiceProvider();
                var audit = serviceProvider.GetService<IMessageAuditStore>();

                x.AddConsumer<RestaurantBookingRequestConsumer>(config =>
                {
                    config.UseMessageRetry(a =>
                    {
                        a.Incremental(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
                    });
                })
                .Endpoint(x =>
                {
                    x.Temporary = true;
                });

                x.AddConsumer<BookingRequestFaultConsumer>(config =>
                {
                    config.UseMessageRetry(a =>
                    {
                        a.Incremental(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
                    });
                });

                x.AddSagaStateMachine<RestaurantBookingSaga, RestaurantBooking>()
                .Endpoint(x => x.Temporary = true)
                .InMemoryRepository();

                x.AddDelayedMessageScheduler();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("mustang.rmq.cloudamqp.com", 5671, "nvyrywte", conf =>
                    {
                        conf.Username("nvyrywte");
                        conf.Password("LfN6kkcHwTenNa5ZG7YSMhx3ymMT2FS4");

                        //ForSSl Port 5671
                        conf.UseSsl(s =>
                        {
                            s.Protocol = System.Security.Authentication.SslProtocols.Tls12;
                        });

                    });

                    cfg.UsePrometheusMetrics(serviceName: "Booking_service");

                    cfg.UseDelayedMessageScheduler();
                    cfg.UseInMemoryOutbox();

                    cfg.ConfigureEndpoints(context);

                    cfg.ConnectSendAuditObservers(audit);
                    cfg.ConnectConsumeAuditObserver(audit);
                });

                services.Configure<MassTransitHostOptions>(opt =>
                {
                    opt.WaitUntilStarted = true;
                    opt.StartTimeout = TimeSpan.FromSeconds(20);
                    opt.StopTimeout = TimeSpan.FromMinutes(2);
                });

                services.AddTransient<RestaurantBooking>();
                services.AddTransient<RestaurantBookingSaga>();

                services.AddSingleton<IMemoryRepository<BookingRequestModel>, MemoryRepository<BookingRequestModel>>();
                services.AddSingleton<BookingService>();

                services.AddHostedService<Worker>();
            });
        });