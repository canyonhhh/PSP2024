using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PSPOS.ApiService.Data;
using PSPOS.ApiService.Repositories;
using PSPOS.ApiService.Repositories.Interfaces;
using PSPOS.ApiService.Services;
using PSPOS.ApiService.Services.Interfaces;
using Stripe;
using System.Text;
using DiscountService = PSPOS.ApiService.Services.DiscountService;
using TaxService = PSPOS.ApiService.Services.TaxService;

var builder = WebApplication.CreateBuilder(args);

builder.AddNpgsqlDbContext<AppDbContext>(connectionName: "postgresdb");

builder.AddServiceDefaults();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IBusinessService, BusinessService>();

builder.Services.AddScoped<ITaxRepository, TaxRepository>();
builder.Services.AddScoped<ITaxService, TaxService>();

builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IDiscountService, DiscountService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IProdAndServRepository, ProdAndServRepository>();
builder.Services.AddScoped<IProdAndServService, ProdAndServService>();

builder.Services.AddScoped<IGiftcardRepository, GiftcardRepository>();
builder.Services.AddScoped<IGiftcardService, GiftcardService>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

var jwtKey = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key is missing"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
})
.AddCookie("Cookies", options =>
{
    options.Cookie.Name = "AuthToken";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"]
    ?? throw new Exception("Stripe:SecretKey is missing");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header...",
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
             Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
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


app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Path: {context.Request.Path}");
    await next();
});

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();