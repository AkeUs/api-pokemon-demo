﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokeApiCustom.Repositories;
using PokeApiCustom.Services;

namespace PokeApiCustom {
    
    public class Startup {
        
        private IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddCors();
            services.AddScoped<IPokemonRepository, PokemonRepository>();
            services.AddScoped<IPokeApiService, PokeApiService>();
            
            // Cliente HTTP
            services.AddHttpClient("PokeApiClient", client => {
                client.BaseAddress = new Uri(Configuration["UrlPokeApi"]);
            });
            
            // Cache Redis
            services.AddDistributedRedisCache(options => {
                options.Configuration = Configuration.GetConnectionString("RedisConnection");
                //options.InstanceName = "PokeApi:";
            });
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseCors(builder => builder.WithOrigins(Configuration["UrlClients"]));
            app.UseMvc();
        }
    }
}
