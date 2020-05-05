using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WorkerServiceM2M.DataLayer;
using WorkerServiceM2M.Models;

namespace WorkerServiceM2M
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<Settings>(
               hostContext.Configuration.GetSection(nameof(Settings)));

                    services.AddSingleton<Settings>(sp =>
                        sp.GetRequiredService<IOptions<Settings>>().Value);

                    services.AddTransient<IBookService, BookService>();

                    services.AddHostedService<Worker>();
                    //services.Configure<Settings>(Options =>
                    //{
                    //    Options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                    //    Options.Database = Configuration.GetSection("MongoConnection:Database").Value;
                    //});
                    //services.AddTransient<BookService>();
                });
    }
}
