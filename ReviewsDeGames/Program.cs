

using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using ReviewsDeGames;
using ReviewsDeGames.Database;
using ReviewsDeGames.Helpers;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;


#region Constants
const bool TrySeed = true;
const bool RestartDb = false;
const bool UseTestDb = false;
#endregion



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.ResolveConflictingActions(x => x.First());

    //Ativando leitura de coment�rios por parte do Swashbuckle 
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);
});
builder.Services.AddControllers()
    .AddOData(options => options.AddRouteComponents(Values.ODataPrefixRoute, OData.GetEdmModel())
        .Select()
        .Filter()
        .OrderBy()
        .SetMaxTop(Values.ODataMaxTop)
        .Count()
        .Expand())
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

    }); ;


builder.Services.AddFormattersForOData();

//Configura o Serilog
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));
builder.Services
    .AddDbContext<ReviewGamesContext>(options =>
    {
        var jsonKey = UseTestDb ? Values.SqlConnectionForTests : Values.SqlConnection;
        var connectionString = builder.Configuration.GetConnectionString(jsonKey);
        options.UseSqlServer(connectionString);
    });


//Meus servi�os
builder.Services.AddRepositories();
builder.Services.AddValidations();
builder.Services.AddGeneralServices();
builder.Services.AddMyIdentity();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "API Reviews de Games");
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.AwakeDB(TrySeed, RestartDb);

app.Run();

public partial class Program { } // Para testes