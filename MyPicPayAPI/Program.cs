using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Refit;
using SimplePicPay;
using SimplePicPay.Data;
using SimplePicPay.Helpers;
using SimplePicPay.Integration;
using SimplePicPay.Models;
using SimplePicPay.Repository.Transaction;
using SimplePicPay.Repository.User;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var key = Encoding.ASCII.GetBytes(Key.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    //c.OperationFilter<SwaggerDefaultValues>();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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

        },
        new List<string>()
        }
    });
});

builder.Services.AddDbContext<ConnectionContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ISendEmail, SendEmail>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IMockVerifyPayment, MockVerifyPayment>();

builder.Services.AddRefitClient<IMockVerifyPaymentRefit>().ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri("https://run.mocky.io");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Adicionar dados iniciais ao contexto do banco de dados em mem ria
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ConnectionContext>();

    if (!dbContext.Users.Any())
    {
        dbContext.Users.Add(new UserModel { Name = "Edson", Type = UserType.Default, CPF = "123.123.123-22", Email = "edsoneurides0705@gmail.com", Password = "123", Balance = 1000 });
        dbContext.Users.Add(new UserModel { Name = "Andreia", Type = UserType.Default, CPF = "123.123.123-87", Email = "andreia@email.com", Password = "123", Balance = 1500 });
        dbContext.Users.Add(new UserModel { Name = "Loja seu Zé", Type = UserType.Store, CPF = "123.123.543-87", Email = "seuze@email.com", Password = "123", Balance = 20000 });
        dbContext.SaveChanges();
    }
}

app.Run();
