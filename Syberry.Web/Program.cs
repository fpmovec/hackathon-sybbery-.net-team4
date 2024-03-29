using Syberry.Web;
using System.Text.Json.Serialization;
using Syberry.Web.Constraints;
using Syberry.Web.Models.Dto;
using Syberry.Web.Services;
using Syberry.Web.Services.Abstractions;
using Syberry.Web.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("CommonFactory", _ => { })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    });

builder.Services.AddScoped<IBelarusBankService, BelarusBankService>();
builder.Services.AddScoped<IAlpfaBankService, AlpfaBankService>();
builder.Services.AddScoped<INationalBankService, NationalBankService>();
//builder.Services.AddSingleton<ICacheService, CacheService>();
//builder.Services.AddSingleton<IRequestsService, RequestsService>();

builder.Services.AddHttpClient<BelarusBankService>();
builder.Services.AddHttpClient<AlpfaBankService>();
builder.Services.AddHttpClient<NationalBankService>();

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName));
builder.Services.Configure<RouteOptions>(opts =>
{
    opts.ConstraintMap.Add("bank", typeof(BankNameConstraint));
});

//builder.Services.AddStackExchangeRedisCache(config =>
//{
//    config.Configuration = "localhost:6379";
//    config.InstanceName = "local";
//});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
