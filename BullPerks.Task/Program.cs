using BullPerks.Data.Dto;
using BullPerks.Api.ServiceExtension;
using Microsoft.OpenApi.Models;
using BullPerks.Data.Context;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

JwtSettings settings = new JwtSettings();
settings.Key = builder.Configuration["JwtSettings:key"]!;
settings.Audience = builder.Configuration["JwtSettings:audience"]!;
settings.Issuer = builder.Configuration["JwtSettings:issuer"]!;
settings.MinutesToExpiration = Convert.ToInt32(builder.Configuration["JwtSettings:minutesToExpiration"]);


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddJWTAuthentication(settings);
builder.Services.AddDependencyInjection();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ExposeResponseHeaders",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200","https://localhost:4200")
                   .AllowAnyHeader()
                   .AllowCredentials()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed(host => true);
        });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Bull Perks APIs"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.UseSwagger(c =>
{
    c.SerializeAsV2 = true;

});
app.UseSwaggerUI(c =>
{
    c.DefaultModelsExpandDepth(-1);
    c.SwaggerEndpoint($"v1/swagger.json", "Bull Perks Website");
    c.RoutePrefix = "swagger";
    c.EnablePersistAuthorization();
    c.DisplayRequestDuration();
});
using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    using (var context = scope.ServiceProvider.GetService<AppDbContext>())
    {
        await context.Database.MigrateAsync();
    }
}
app.UseCors("ExposeResponseHeaders");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
