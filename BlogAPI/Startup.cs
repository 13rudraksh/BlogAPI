using BlogAPI.Persistence.Services;
using MongoDB.Driver;
using MongoDB.Bson;


namespace BlogAPI;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddScoped<IBlogService, BlogService>();

        var connectionString = Configuration.GetSection("MongoDB:ConnectionString").Value;
        var client = new MongoClient(connectionString);
        services.AddSingleton(client.GetDatabase("BlogAPIs"));
        //services.AddScoped(sp => BlogDbContext.Create(sp.GetRequiredService<IMongoDatabase>()));
        services.AddScoped<IBlogDbContext>(sp => BlogDbContext.Create(sp.GetRequiredService<IMongoDatabase>()));

        services.AddLogging();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> _logger)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        var database = app.ApplicationServices.GetRequiredService<IMongoDatabase>();
        try
        {
            database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait();
            _logger.LogInformation("Connected to MongoDB successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to MongoDB.");
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}