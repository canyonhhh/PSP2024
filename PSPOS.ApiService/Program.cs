using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PSPOS.ApiService.Data;
using PSPOS.ApiService.Repositories;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services;
using PSPOS.ApiService.Services.Interfaces;
using System.Text;
using AuthenticationService = PSPOS.ApiService.Services.AuthenticationService;
using IAuthenticationService = PSPOS.ApiService.Services.Interfaces.IAuthenticationService;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<AppDbContext>(connectionName: "postgresdb");

builder.AddServiceDefaults();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IBusinessService, BusinessService>();
builder.Services.AddScoped<ITaxRepository, TaxRepository>();
builder.Services.AddScoped<ITaxService  , TaxService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService , ReservationService>();

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IDiscountService, DiscountService>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddSingleton<ISmsService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var accountSid = builder.Configuration["Twilio:SID"] ?? throw new Exception("Twilio:SID is missing");
    var authToken = builder.Configuration["Twilio:Token"] ?? throw new Exception("Twilio:Token is missing");
    var fromPhoneNumber = builder.Configuration["Twilio:PhoneNumber"] ?? throw new Exception("Twilio:PhoneNumber is missing");

    return new TwilioSmsService(accountSid, authToken, fromPhoneNumber);
});



builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var jwtKey = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key is missing"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("AuthToken"))
            {
                context.Token = context.Request.Cookies["AuthToken"];
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();