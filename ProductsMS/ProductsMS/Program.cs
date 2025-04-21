
using ProductosMs;
using ProductosMs.Application;
using ProductsMs.Infrastructure;
using ProductsMs.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation(builder.Configuration)
        .AddInfrastructure(builder.Configuration)
        .AddApplication();

builder.Services.AddSwaggerGen();



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


var _appSettings = new AppSettings();
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
_appSettings = appSettingsSection.Get<AppSettings>();
builder.Services.Configure<AppSettings>(appSettingsSection);

builder.Services.Configure<HttpClientUrl>(
    builder.Configuration.GetSection("HttpClientAddress"));

builder.Services.AddHttpContextAccessor();
//builder.Services.AddHttpClient<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Connected!");

app.UseHttpsRedirection();

//app.UseCors("AllowAll");

//app.UseAuthorization();

app.MapControllers();

app.Run();