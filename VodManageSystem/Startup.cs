using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using VodManageSystem.Models.DataModels;
using VodManageSystem.Models.Dao;
using Newtonsoft.Json.Serialization;

namespace VodManageSystem
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
            services.AddMvc();

            // the follwing is to keep the properties' name as the as they are defined
            // when the model is Serialized
            // services.AddMvc()
            // .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddMvc().AddJsonOptions(
                options =>
                {
                    // the follwing is to keep the properties' name as the as they are defined
                    // when the model is Serialized
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    // for avoiding infinite loop when serializing
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                }
            );

            services.AddDistributedMemoryCache();
            services.AddSession();

            // For pomelo.EntityFrameworkCore.MySql
            services.AddDbContext<KtvSystemDBContext>(options =>
                  options.UseMySql(Configuration.GetConnectionString("MySqlConnection")));

            services.AddScoped<LanguagesManager>();  // languages management service
            services.AddScoped<SingareasManager>();  // singer areas management service
            services.AddScoped<SingersManager>();    // singers management service
            services.AddScoped<SongsManager>();      // songs management service
            services.AddScoped<PlayerscoreManager>();   // Playerscore Manager service
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
