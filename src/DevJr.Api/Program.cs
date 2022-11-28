using DevJr.Api.Configuration;
using DevJr.Api.Extensions;
using DevJr.Data.Context;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MeuDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle]
builder.Services.AddIdentityConfig(builder.Configuration);
builder.Services.AddApiConfig();
builder.Services.AddSwaggerConfig();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ResolveDependencies();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLoggingConfig(builder.Configuration);

var app = builder.Build();
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerConfig(apiVersionDescriptionProvider);

}
else
{
    app.UseCors("Production");
}

app.UseApiConfig(app.Environment);
app.UseAuthentication();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseLoggingConfiguration();

app.Run();
