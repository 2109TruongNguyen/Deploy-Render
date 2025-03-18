using System.Text;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetCore.AutoRegisterDi;
using Serilog;
using NZWalksAPI.Data;
using NZWalksAPI.Mapper;
using NZWalksAPI.Middelwares;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Services.Feature;
using Hangfire;
using Hangfire.SqlServer;
using NZWalksAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Logging configuration
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Warning()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NZ Walks API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new List<string>()
        }
    });
});

// Add DbContext
builder.Services.AddDbContext<NZWalksDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksConnectionString")));

// Auto Register DI
builder.Services.RegisterAssemblyPublicNonGenericClasses(
        Assembly.GetAssembly(typeof(ServiceLayer.ServiceMarker)),
        Assembly.GetAssembly(typeof(RepositoryLayer.RepoMarker)),
        Assembly.GetAssembly(typeof(NZWalksAPI.ControllerMarker))
    )
    .Where(c => c.Name.EndsWith("Service") || c.Name.EndsWith("Repository"))
    .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

builder.Services.AddScoped<TemplateService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

// Add Identity
builder.Services.AddIdentityCore<User>()
    .AddRoles<Role>()
    .AddEntityFrameworkStores<NZWalksDbContext>()
    .AddDefaultTokenProviders();

// Password policy
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Authentication setup
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:AccessSecretKey"]!))
        };
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
        googleOptions.CallbackPath = "/api/auth/google-response";
        googleOptions.Scope.Add("profile"); // Yêu cầu ảnh đại diện
        googleOptions.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
    })
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = builder.Configuration["Facebook:AppId"];
        facebookOptions.AppSecret = builder.Configuration["Facebook:AppSecret"];

        // Thêm quyền truy cập vào các trường dữ liệu
        facebookOptions.Scope.Add("email");
        facebookOptions.Scope.Add("public_profile");
        facebookOptions.Scope.Add("user_birthday"); // Lấy ngày sinh
        facebookOptions.Scope.Add("user_gender"); // Lấy giới tính
        facebookOptions.Scope.Add("user_location"); // Lấy vị trí
        facebookOptions.Scope.Add("user_hometown"); // Lấy quê quán

        // Yêu cầu các trường dữ liệu cần lấy
        facebookOptions.Fields.Add("id");
        facebookOptions.Fields.Add("email");
        facebookOptions.Fields.Add("name");
        facebookOptions.Fields.Add("first_name");
        facebookOptions.Fields.Add("last_name");
        facebookOptions.Fields.Add("picture.width(200).height(200)");
        facebookOptions.Fields.Add("birthday"); // Ngày sinh
        facebookOptions.Fields.Add("gender"); // Giới tính
        facebookOptions.Fields.Add("location"); // Vị trí
        facebookOptions.Fields.Add("hometown"); // Quê quán

        // Map dữ liệu vào Claims
        facebookOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        facebookOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        facebookOptions.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "first_name");
        facebookOptions.ClaimActions.MapJsonKey(ClaimTypes.Surname, "last_name");
        facebookOptions.ClaimActions.MapJsonKey(ClaimTypes.DateOfBirth, "birthday");
        facebookOptions.ClaimActions.MapJsonKey(ClaimTypes.Gender, "gender");
        facebookOptions.ClaimActions.MapJsonKey("urn:facebook:location", "location", "name");
        facebookOptions.ClaimActions.MapJsonKey("urn:facebook:hometown", "hometown", "name");

        // Xử lý ảnh đại diện
        facebookOptions.ClaimActions.MapJsonSubKey("urn:facebook:picture", "picture", "data", "url");
    });


    
// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", corsPolicyBuilder =>
        corsPolicyBuilder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

// Configure Hangfire
builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnectionString"), new SqlServerStorageOptions
          {
              CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
              QueuePollInterval = TimeSpan.Zero,
              UseRecommendedIsolationLevel = true,
              DisableGlobalLocks = true
          });
});

// Đăng ký Hangfire Server
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) // Cho phép cả Production
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Static files configuration
app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
        RequestPath = "/Images"
    }
);

app.UseHangfireServer();

// 💡 Thêm Job Hangfire ở đây (sau khi khởi tạo Server)
var jobManager = app.Services.GetRequiredService<IRecurringJobManager>();

// Job chạy mỗi 1 phút
jobManager.AddOrUpdate(
    "process_orders",
    () => Console.WriteLine($"🕒 Processing orders at {DateTime.Now}"),
    Cron.MinuteInterval(1)
);

// Nếu muốn chạy ngay lập tức:
RecurringJob.Trigger("process_orders");

app.MapControllers();
app.Run();
