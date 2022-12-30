using ikoLite.Kitchen;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

ConnectionBuilder(args).Build().Run();



static IHostBuilder ConnectionBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<KitchenTableBookedConsumer>();

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

                    cfg.ConfigureEndpoints(context);
                });

                services.AddTransient<Manager>();
            });
        });
