
using System.Net.Http.Headers;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
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
using ProductsMS.Common.Dtos.Category.Response;
using ProductsMS.Common.Dtos.Product.Request;
using ProductsMS.Common.Dtos.Product.Response;
using ProductsMS.Core.Service.Auction;
using ProductsMs.Domain.Entities.Category;
using ProductsMs.Domain.Entities.Products;
using ProductsMS.Core.Service.User;
using ProductsMS.Infrastructure.Services.Auction;
using ProductsMS.Infrastructure.Services.User;
using RabbitMQ.Client;
using ProductsMS.Infrastructure.Services.History;
using ProductsMS.Core.Service.History;


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

builder.Services.AddSingleton<IRabbitMQConsumer, RabbitMQConsumer>();
builder.Services.AddSingleton<IConnectionRabbbitMQ, RabbitMQConnection>();

builder.Services.AddHostedService<RabbitMQBackgroundService>();

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


builder.Services.AddSingleton<IConnectionFactory>(provider =>
{
    return new ConnectionFactory
    {
        HostName = "localhost",
        Port = 5672,
        UserName = "guest",
        Password = "guest",
    };
});

// ?? Registrar `RabbitMQConnection` pasando `IConnectionFactory` en el constructor
builder.Services.AddSingleton<IConnectionRabbbitMQ>(provider =>
{
    var connectionFactory = provider.GetRequiredService<IConnectionFactory>();
    var rabbitMQConnection = new RabbitMQConnection(connectionFactory);
    rabbitMQConnection.InitializeAsync().Wait(); // ?? Ejecutar inicialización antes de inyectarlo
    return rabbitMQConnection;
}); builder.Services.AddSingleton<IRabbitMQConsumer, RabbitMQConsumer>();

// ?? Ahora los Producers pueden usar `RabbitMQConnection`

builder.Services.AddSingleton<IEventBus<CreateProductDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<IConnectionRabbbitMQ>();
    return new RabbitMQProducer<CreateProductDto>(rabbitMQConnection);
});

builder.Services.AddSingleton<IEventBus<UpdateProductDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<IConnectionRabbbitMQ>();
    return new RabbitMQProducer<UpdateProductDto>(rabbitMQConnection);
});

builder.Services.AddSingleton<IEventBus<GetProductDto>>(provider =>
{
    var rabbitMQConnection = provider.GetRequiredService<IConnectionRabbbitMQ>();
    return new RabbitMQProducer<GetProductDto>(rabbitMQConnection);
});

builder.Services.AddSingleton<IMongoCollection<GetProductDto>>(provider =>
{
    var mongoClient = new MongoClient("mongodb+srv://yadefreitas19:08092001@cluster0.owy2d.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
    var database = mongoClient.GetDatabase("ProductMs");
    return database.GetCollection<GetProductDto>("Products");
});

// ?? Registrar `RabbitMQConsumer` solo una vez
builder.Services.AddSingleton<RabbitMQConsumer>(provider =>
{
   
    var rabbitMQConnection = provider.GetRequiredService<IConnectionRabbbitMQ>();
    var productCollection = provider.GetRequiredService<IMongoCollection<GetProductDto>>();
    return new RabbitMQConsumer(rabbitMQConnection, productCollection);
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
builder.Services.AddHttpClient<IUserService, UserService>();
builder.Services.AddHttpClient<IAuctionService, AuctionService>();
builder.Services.AddHttpClient<IHistoryService, HistoryService>();


//Configurar Firebase Storage Settings desde appsettings.json


// Agregar el cliente de Firebase Storage
//builder.Services.AddSingleton<IFirebaseStorageService, FirebaseStorageService>();


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