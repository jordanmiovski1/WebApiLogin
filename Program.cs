using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);
var request_origin = "_request_origin";

{
    var services = builder.Services;
    var env = builder.Environment;

    services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebApiDatabase") ?? throw new InvalidOperationException("Connection string 'WebApiApplication2Context' not found.")));

    services.AddCors(options =>
    {
        options.AddPolicy(name: request_origin,
            policy =>
            {
                policy.WithOrigins("dummy.com");
            });
    });

    services.AddControllers();

    services.AddAutoMapper(typeof(Program));

    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    services.AddScoped<IJwtUtils, JwtUtils>();

    services.AddScoped<IUserService, WorkerService>();
   
    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Bearer Authentication with JWT Token",
            Type = SecuritySchemeType.Http
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
          {
             new OpenApiSecurityScheme
             {
                 Reference = new OpenApiReference
                 {
                     Id = "Bearer",
                     Type = ReferenceType.SecurityScheme
                 }
             },
             new List<string>()
          }
        });
    });

    services.AddFluentValidation(conf =>
    {
        conf.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
        conf.AutomaticValidationEnabled = false;
    });
}

var app = builder.Build();

{    
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    app.UseMiddleware<ErrorHandlerMiddleware>();
    
    app.UseMiddleware<JwtMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(request_origin);

    app.MapControllers();
}

app.Run();