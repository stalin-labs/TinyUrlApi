using TinyUrl.Api.Core;
using TinyUrl.Api.Handlers;
using TinyUrl.Api.Options;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Configuration
// ---------------------------------------------------------------------------
builder.Services.Configure<TinyUrlOptions>(builder.Configuration.GetSection(TinyUrlOptions.SectionName));

// ---------------------------------------------------------------------------
// Services
// ---------------------------------------------------------------------------
builder.Services.AddScoped<IRepository>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string DefaultConnection is not configured.");
    return new PostgresRepository(connectionString);
});

builder.Services.AddScoped<IUrlService>(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<TinyUrlOptions>>().Value;
    var repository = sp.GetRequiredService<IRepository>();
    return new UrlService(repository, new Uri(options.TinyUrlBaseAddress));
});

builder.Services.AddScoped<IHttpRequestHandler, TinyUrlHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TinyUrl API", Version = "v1" });
});

builder.Services.AddHealthChecks()
    .AddNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")!, tags: new[] { "ready" });

// ---------------------------------------------------------------------------
// Middleware pipeline
// ---------------------------------------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/startup", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

app.Run();

// Expose for integration testing
public partial class Program { }
