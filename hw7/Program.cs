using System.Diagnostics;
using ikoLite;
using ikoLite.Messaging;
using ikoLite.Models;
using ikoLite.Services.Booking;
using MassTransit;
using MassTransit.Audit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

//var rest = new BookingService();

////Thread thread1 = new(new ThreadStart(rest.AutoCleanBooks));
////thread1.Start();

//while (true)
//{
//    Console.WriteLine("Welcome! Would you like to book a table? \n" +
//        "1 - add book sms(async)\n" +
//        "2 - add book phone call(sync)\n" +
//        "3 - remove book sms(async)\n" +
//        "4 - remove book phone call(sync)");

//    if (!int.TryParse(Console.ReadLine(), out var choice) && choice is not (1 or 2 or 3 or 4))
//    {
//        Console.WriteLine("Please, enter 1,2,3 or 4");
//        continue;
//    }

//    Stopwatch stopwatch = new();
//    stopwatch.Start();

//    //todo add user enter for all methods
//    if (choice == 1)
//        rest.BookFreeTableAsync(1);

//    if (choice == 2)
//        rest.BookFreeTable(1);

//    if (choice == 3)
//        rest.RemoveBookedTable(1);

//    if (choice == 4)
//        rest.RemoveBookedTableAsync(1);

//    Console.WriteLine("Thank's for booking");
//    stopwatch.Stop();
//    TimeSpan timeSpan = stopwatch.Elapsed;
//    Console.WriteLine($"{timeSpan.Seconds:00}:{timeSpan.Milliseconds}:{timeSpan.Nanoseconds}");
//}





ConnectionBuilder(args).Build().Run();



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
                    //config.UseScheduledRedelivery(b =>
                    //{
                    //    b.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(20));
                    //});

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
                    //config.UseScheduledRedelivery(b =>
                    //{
                    //    b.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(20));
                    //});

                    config.UseMessageRetry(a =>
                    {
                        a.Incremental(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));
                    });
                })
                .Endpoint(x =>
                {
                    x.Temporary = false;
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