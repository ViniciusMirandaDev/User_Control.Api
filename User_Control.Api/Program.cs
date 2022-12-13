using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using User_Control.Api.Application.Repositories;
using User_Control.Api.Application.Repositories.Interfaces;
using User_Control.Api.Application.Services;
using User_Control.Api.Application.Services.Interfaces;
using User_Control.Api.Infrastucture.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swaggerGen =>
{
    swaggerGen.SwaggerDoc("v1", new OpenApiInfo { Title = "User_Control API", Version = "v1" });

    var security = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        }, new List<string>()
                    }
                };
    swaggerGen.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "User_Control Auth",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    swaggerGen.AddSecurityRequirement(security);

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    swaggerGen.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins(builder.Configuration.GetValue<string>("AllowedHosts"))
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod();
                      });
});

builder.Services.AddAuthorization(options =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
       JwtBearerDefaults.AuthenticationScheme);

    defaultAuthorizationPolicyBuilder =
        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();

    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

builder.Services.AddAuthentication(authentication =>
{
    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(jwtBearer =>
{

    var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("Authentication:SecretKey"));
    jwtBearer.RequireHttpsMetadata = false;
    jwtBearer.SaveToken = true;
    jwtBearer.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

//Repositories Dependecy Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();

//Services Dependecy Injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();


builder.Services.AddDbContext<CoreContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error is ArgumentException || exceptionHandlerPathFeature?.Error is ArgumentNullException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }

        if (exceptionHandlerPathFeature?.Error is NullReferenceException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        var json = new
        {
            StatusCode = context.Response.StatusCode,
            Message = exceptionHandlerPathFeature?.Error.Message
        };

        await context.Response.WriteAsync(JsonConvert.SerializeObject(json));
    });
});

app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
