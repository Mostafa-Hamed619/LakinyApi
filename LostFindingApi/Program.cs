using LostFindingApi.Hubs;
using LostFindingApi.Middlewares;
using LostFindingApi.Models;
using LostFindingApi.Models.Data;
using LostFindingApi.Models.Real_Time;
using LostFindingApi.Services;
using LostFindingApi.Services.IRepository;
using LostFindingApi.Services.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
    options.InvalidModelStateResponseFactory = actioncontext =>
    {
        var modelState = actioncontext.ModelState.Values;

        return new OkObjectResult(new ApiResponse
        {
            HttpStatusCode = (int)HttpStatusCode.OK,
            Descriptions = ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.OK),
            Errors = modelState.SelectMany(x => x.Errors,(x,y)=>y.ErrorMessage).ToList()
        });
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/AppLogs-.txt",rollingInterval : RollingInterval.Day)
    .CreateLogger();

//builder.Host.UseSerilog();
builder.Services.AddDbContextPool<DataContext>(opt =>
{
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DbConnection")
        );
});

builder.Services.AddIdentityCore<User>(opt =>
{
    opt.Password.RequiredLength = 6;
    opt.Password.RequireDigit = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireNonAlphanumeric = true;

   
    opt.SignIn.RequireConfirmedEmail = true;
}).
AddRoles<IdentityRole>().
AddRoleManager<RoleManager<IdentityRole>>().
AddUserManager<UserManager<User>>().
AddSignInManager<SignInManager<User>>().
AddEntityFrameworkStores<DataContext>().
AddDefaultTokenProviders();



builder.Services.AddScoped<JwtServices>();
builder.Services.AddScoped<EmailServices>();
builder.Services.AddScoped<ItemRepository, ItemMockRepository>();
builder.Services.AddTransient<IFileRepository, FileMockRepository>();
builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
builder.Services.AddSingleton<IhttpContextAccessor, httpContextMockRepository>();
builder.Services.AddScoped<ChatServices, ChatRepository>();
builder.Services.AddScoped<ContextSeedService>();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the bearer scheme. \r\n\r\n" +
        "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
        "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type =ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "outh2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWTKey:Issuer"],
            ValidateIssuer = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTKey:Key"])),
            ValidateAudience = false
        }; 
    });
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = ActionContext =>
    {
        var errors = ActionContext.ModelState
        .Where(x => x.Value.Errors.Count() > 0)
        .SelectMany(x => x.Value.Errors)
        .Select(x => x.ErrorMessage).ToList();

        var toReturn = new
        {
            errors = errors
        };

        return new BadRequestObjectResult(toReturn);
    };
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



#region Enable Using the Files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/Resources"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Search")),
    RequestPath = "/Search"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Accounts")),
    RequestPath = "/Accounts"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "chatFiles")),
    RequestPath = "/chatFiles"
});

#endregion

#region Enable DirectoryBrowser
//Enable directory browsing
app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/Resources"
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Search")),
    RequestPath = "/Search"
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Accounts")),
    RequestPath = "/Accounts"
});
app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "chatFiles")),
    RequestPath = "/chatFiles"
});
#endregion

app.UseMiddleware<HttpStatusErrorMiddleware>();
app.UseAuthentication();

app.UseAuthorization();


app.UseMiddleware<userInfoMiddlewar>();


app.MapControllers();

//app.UseSerilogRequestLogging();
using var scope = app.Services.CreateScope();
try
{
    var contextSeedServices = scope.ServiceProvider.GetService<ContextSeedService>();
    await contextSeedServices.InitializeContextAsync();
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
    logger.LogError(ex.Message, "Failed to initialize the seed context");
}

app.MapHub<ChatHub>("/hubs/Chat");

app.Run();