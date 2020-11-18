using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using muratulugBlogCore.API.Models;

namespace muratulugBlogCore.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Api cross-orgin resource sharing added (uzak sunucular ile düzenleme ve policy işlemleri )
            services.AddCors(opts =>
            {
            opts.AddDefaultPolicy(x => 
               {
                  x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials(); // Tüm orjin tüm header tüm methodlara ve tüm kimlik doğrulamalara şimdilik izin ver . 
               });
            });

            //servis entity framework database ile bağlantı  .
            services.AddDbContext<muratulugBlogDbContext>(opts => 
            {
                opts.UseSqlServer(Configuration["ConnectionStrings:DefaultSqlConnectionString"]);
            });

            //default
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            //Swagger için önce nugget (swashbuckleAspNetCore) eklendi sonrasında addSwaggerGen 
            services.AddSwaggerGen( c => 
            {
                c.SwaggerDoc(name: "v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "muratulugBlogCore.API", Version = "v1.000" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); //This line
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseDeveloperExceptionPage();
            //Swagger başla
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            //Swagger bitir

            app.UseCors(); // Cors kullanmak için
            app.UseStaticFiles(); // Resimler veya herhangi bir statik dosya ile çalışabilmek için 
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
