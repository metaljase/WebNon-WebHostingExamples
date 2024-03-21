namespace Metalhead.Examples.Hosting.CDB.Api;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    // Configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseHttpsRedirection();

        // Add route matching to the middleware pipeline.
        app.UseRouting();
        // Add endpoint execution to the middleware pipeline, and add WeatherForecast endpoints.
        app.UseEndpoints(WeatherForecastEndpoints.Register);
    }
}
