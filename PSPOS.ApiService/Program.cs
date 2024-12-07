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

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var jwtKey = Encoding.UTF8.GetBytes("e56ccfdac2091df4a0377cd7db900c23b716f8830cc178f6401cea7a12b42a6f3389edb104a6703036f5a56093c422e289b6292176659c45786b7493cca4371e0b20ea931768aac892f7f83084074c51c63f975d46020810a93943ed1d48454b56ad4d265a2dde18fea260ad117ad78fa8cc48088265cdcbf240b9f5e20a358646be1c022699e81147c3be18115a8d335904d00513c232c7dca6a32dfc1e3bcdee59c6ecbcbf3a57b7c2622cfca7266eb17e012079223855911a25d474a9c93a50ea17ae9f5bb2143eac7fef1cc49ef5044e59ad8901f4fee0382a286fc9587090f55fe72d428fbe43d541f24d07c23c5353a5929482b92d00e2f17b4c452238");

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
