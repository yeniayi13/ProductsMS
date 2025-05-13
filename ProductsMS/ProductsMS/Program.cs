
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using ProductosMs;
using ProductosMs.Application;
using ProductsMs.Infrastructure;
using ProductsMs.Infrastructure.Settings;
using ProductsMS.Common.AutoMapper;
using UserMs.Core.Database;
using ProductsMS.Infrastructure.Database.Context.Mongo;
using ProductsMS.Core.RabbitMQ;
using ProductsMS.Infrastructure.RabbitMQ.Connection;
using ProductsMS.Infrastructure.RabbitMQ.Consumer;
using ProductsMS.Infrastructure.RabbitMQ;
using ProductsMS.Common.Dtos.Category.Request;
using ProductsMS.Common.Dtos.Category.Response;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Core.Service.Firebase;
using ProductsMS.Infrastructure.Services.Firebase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation(builder.Configuration)
        .AddInfrastructure(builder.Configuration)
        .AddApplication();

// Registrar el serializador de GUID
BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));
// Registro de los perfiles de AutoMapper
var profileTypes = new[]
{
    typeof(CategoryProfile),
     typeof(ProductProfile)
};

foreach (var profileType in profileTypes)
{
    builder.Services.AddAutoMapper(profileType);
}


builder.Services.AddSingleton<IApplicationDbContextMongo>(sp =>
{
    const string connectionString = "mongodb+srv://yadefreitas19:08092001@cluster0.owy2d.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
    var databaseName = "ProductMs";
    return new ApplicationDbContextMongo(connectionString, databaseName);
});

builder.Services.AddScoped(sp =>
{
    var dbContext = sp.GetRequiredService<IApplicationDbContextMongo>();
    return dbContext.Database.GetCollection<CategoryEntity>("Categories"); // Nombre de la colección en MongoDB
});

builder.Services.AddScoped(sp =>
{
    var dbContext = sp.GetRequiredService<IApplicationDbContextMongo>();
    return dbContext.Database.GetCollection<ProductEntity>("Products"); // Nombre de la colección en MongoDB
});


builder.Services.AddSingleton<RabbitMQConnection>(provider =>
{
    var rabbitMQConnection = new RabbitMQConnection();
    rabbitMQConnection.InitializeAsync().Wait(); //  Inicialización segura
    return rabbitMQConnection;
});

// Usa la misma instancia de `RabbitMQConnection` para el Producer
builder.Services.AddSingleton<IEventBus<CreateCategoryDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<RabbitMQConnection>();
    return new RabbitMQProducer<CreateCategoryDto>(rabbitMQConnection);
});

builder.Services.AddSingleton<IEventBus<UpdateCategoryDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<RabbitMQConnection>();
    return new RabbitMQProducer<UpdateCategoryDto>(rabbitMQConnection);
});


builder.Services.AddSingleton<IEventBus<GetCategoryDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<RabbitMQConnection>();
    return new RabbitMQProducer<GetCategoryDto>(rabbitMQConnection);
});


builder.Services.AddSingleton<IEventBus<CreateProductDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<RabbitMQConnection>();
    return new RabbitMQProducer<CreateProductDto>(rabbitMQConnection);
});

builder.Services.AddSingleton<IEventBus<UpdateProductDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<RabbitMQConnection>();
    return new RabbitMQProducer<UpdateProductDto>(rabbitMQConnection);
});


builder.Services.AddSingleton<IEventBus<GetProductDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<RabbitMQConnection>();
    return new RabbitMQProducer<GetProductDto>(rabbitMQConnection);
});




//  Usa la misma instancia de `RabbitMQConnection` para el Consumer
builder.Services.AddSingleton<RabbitMQConsumer>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<RabbitMQConnection>();
    return new RabbitMQConsumer(rabbitMQConnection);
});

// Iniciar el consumidor automáticamente con `RabbitMQBackgroundService`
builder.Services.AddHostedService<RabbitMQBackgroundService>();


//* Para que funcione el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddSwaggerGen();


var _appSettings = new AppSettings();
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
_appSettings = appSettingsSection.Get<AppSettings>();
builder.Services.Configure<AppSettings>(appSettingsSection);

builder.Services.Configure<HttpClientUrl>(
    builder.Configuration.GetSection("HttpClientAddress"));

builder.Services.AddHttpContextAccessor();
//builder.Services.AddHttpClient<IUserService, UserService>();

//Configurar Firebase Storage Settings desde appsettings.json
builder.Services.Configure<FirebaseStorageSettings>(builder.Configuration.GetSection("Firebase"));

// Agregar el cliente de Firebase Storage
builder.Services.AddSingleton<IFirebaseStorageService, FirebaseStorageService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.MapGet("/", () => "Connected!");

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();