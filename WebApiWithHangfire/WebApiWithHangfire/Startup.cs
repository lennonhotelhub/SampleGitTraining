using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Hangfire;
using Hangfire.MemoryStorage;
using WebApiWithHangfire.Services;
using Hangfire.Mongo;
using MongoDB.Driver;

namespace WebApiWithHangfire
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //BookService = bookService;
        }

        public IConfiguration Configuration { get; }
        //public BookService BookService { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["BookstoreDatabaseSettings:ConnectionString"];
            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            var storageOptions = new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    Strategy = MongoMigrationStrategy.Migrate,
                    BackupStrategy = MongoBackupStrategy.Collections
                }
            };
            services.AddHangfire(config =>
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseDefaultTypeSerializer()
            .UseMongoStorage(mongoClient, "HangfireDB", storageOptions));
            //.UseMemoryStorage());


            services.AddHangfireServer();
            services.AddControllers();
            services.AddTransient<IBookService,BookService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IBookService bookservice, IRecurringJobManager recurringJobManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseHangfireDashboard();

            app.UseHangfireServer(new BackgroundJobServerOptions { ServerName = "server1", WorkerCount = 2 });
            app.UseHangfireServer(new BackgroundJobServerOptions { ServerName = "server2", WorkerCount = 2 });
            using (var server = new BackgroundJobServer(
                     new BackgroundJobServerOptions { ServerName = "server1" }))
            {
               BackgroundJob.Enqueue(() => bookservice.Get());
            }

            using (var server = new BackgroundJobServer(
                         new BackgroundJobServerOptions { ServerName = "server2" }))
            {
                BackgroundJob.Enqueue(() => bookservice.Get());
            }

            recurringJobManager.AddOrUpdate("every five seconds", () => bookservice.Get(), "* * * * *");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
