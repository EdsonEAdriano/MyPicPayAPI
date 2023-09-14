var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/Authorize", () =>
{
    Random rnd = new Random();

    if (rnd.Next(100) <= 10)
    {
        var resultado = new { message = "N�o Autorizado" };
        return resultado;

    }
    else
    {
        var resultado = new { message = "Autorizado" };
        return resultado;
    }
    
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();