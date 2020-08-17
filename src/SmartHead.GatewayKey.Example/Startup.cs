using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartHead.GatewayKey.Example.Dal;

namespace SmartHead.GatewayKey.Example
{
    public class Startup
    {
        private SqliteConnection _connection;
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            
            services
                .AddDbContext<ExampleDbContext>(opt => 
                    opt.UseSqlite(_connection)
                );
            
            services.AddScoped<DbContext, ExampleDbContext>();
            
            services.AddControllers();

            services.AddAuthentication().AddGatewayKey<Models.GatewayKey>(opt =>
            {
                opt.GatewayKeyHeaderName = "Api-Key";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ExampleDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
            
            dbContext.GatewayKeys.AddRange(new[] {
                new Models.GatewayKey( "forbidden", "forbidden", GatewayKeyType.Device),
                new Models.GatewayKey( "authorized", "authorized", GatewayKeyType.Device, true)
            });
            
            dbContext.SaveChanges();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}