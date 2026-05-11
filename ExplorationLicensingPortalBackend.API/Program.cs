using Azure.Storage.Blobs;
using ExplorationLicensingPortalBackend.Application.Interfaces;
using ExplorationLicensingPortalBackend.Application.Services;
using ExplorationLicensingPortalBackend.Domain.Interfaces;
using ExplorationLicensingPortalBackend.Infrastructure.Persistence;
using ExplorationLicensingPortalBackend.Infrastructure.Repositories;
using ExplorationLicensingPortalBackend.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mining Exploration Licensing Portal API",
        Version = "v1",
        Description = """
            API for the Ministry of Mines and Steel Development — Mines Inspectorate Department.

            Supports the following licence/permit applications:
            - **Reg 133** — Licence to Purchase and Possess Minerals
            - **Reg 131** — Permit to Export Minerals for Commercial Purposes
            - **Reg 132** — Permit to Export Mineral Samples (Analysis / Exhibition)

            ### Application Workflow
            1. `POST /api/minesinspectorate` — Create application
            2. `POST /api/minesinspectorate/{id}/generate-rrr` — Generate Remita RRR & compute fee
            3. `POST /api/minesinspectorate/{id}/documents` — Upload required documents
            4. `POST /api/minesinspectorate/{id}/submit` — Submit for processing
            """,
        Contact = new OpenApiContact
        {
            Name = "Mines Inspectorate Department",
            Email = "info@minesportal.gov.ng"
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    // Show enums as strings
    c.UseInlineDefinitionsForEnums();
});

// EF Core - MS SQL
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Azure Blob Storage
builder.Services.AddSingleton(_ =>
    new BlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]));

// Remita HTTP client
builder.Services.AddHttpClient<IRemitaService, RemitaService>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Remita:BaseUrl"]!));

// Repositories & Services
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mining Portal API v1");
    c.RoutePrefix = string.Empty; // Swagger UI at root "/"
    c.DocumentTitle = "Mining Licensing Portal API";
    c.DefaultModelsExpandDepth(2);
    c.DisplayRequestDuration();
    c.EnableFilter();
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
