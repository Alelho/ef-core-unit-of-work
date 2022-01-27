using EFCoreDataAccess.Data;
using EFCoreUnitOfWork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Diagnostics;

namespace EFCoreDataAccess.API
{
	public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = @"Server=localhost;Database=EFCoreUnitOfWork;Uid=root;Pwd=123456;";

            services.AddDbContext<EmployeeDbContext>(options =>
                options.UseMySql(connectionString, serverVersion: ServerVersion.AutoDetect(connectionString))
                .LogTo(msg => Debug.WriteLine(msg), LogLevel.Error)
                .EnableSensitiveDataLogging());

            services.AddUnitOfWork<EmployeeDbContext>();

            services.AddControllers()
                .AddNewtonsoftJson(o => o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EFCoreDataAccess.API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EFCoreDataAccess.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            RunMigrate(app.ApplicationServices);
        }

        private void RunMigrate(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.CreateScope().ServiceProvider.GetService<EmployeeDbContext>();
            dbContext.Database.Migrate();
        }
    }
}
