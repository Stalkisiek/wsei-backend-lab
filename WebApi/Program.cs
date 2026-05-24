using Infrastucture.Repository;
using AutoMapper;

namespace WebApi;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        builder.Services.AddSingleton<CoreApp.Repositories.IStudentRepository, Infrastucture.Memory.MemoryStudentRepository>();
        builder.Services.AddSingleton<CoreApp.Repositories.ILecturerRepository, Infrastucture.Memory.MemoryLecturerRepository>();
        builder.Services.AddSingleton<CoreApp.Repositories.IGradeRepository, Infrastucture.Memory.MemoryGradeRepository>();
        builder.Services.AddSingleton<CoreApp.Repositories.IUniversityUnitOfWork, Infrastucture.Repository.MemoryUniversityUnitOfWork>(sp =>
        {
            var students = sp.GetRequiredService<CoreApp.Repositories.IStudentRepository>();
            var lecturers = sp.GetRequiredService<CoreApp.Repositories.ILecturerRepository>();
            var grades = sp.GetRequiredService<CoreApp.Repositories.IGradeRepository>();
            return new Infrastucture.Repository.MemoryUniversityUnitOfWork(students, lecturers, grades);
        });
        builder.Services.AddSingleton<CoreApp.Services.IStudentService, Infrastucture.Services.MemoryStudentService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        {
                            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            TemperatureC = Random.Shared.Next(-20, 55),
                            Summary = summaries[Random.Shared.Next(summaries.Length)]
                        })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast");

        app.Run();
    }
}