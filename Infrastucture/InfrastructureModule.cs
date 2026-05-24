using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Infrastucture.EntityFramework.Context;
using Infrastucture.EntityFramework.Entities;
using Infrastucture.EntityFramework.Repositories;
using Infrastucture.EntityFramework.UnitOfWork;
using CoreApp.Repositories;
using CoreApp.Services;

namespace Infrastucture;

public static class InfrastructureModule
{
    public static IServiceCollection AddUniversityEfModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("AppDb") ?? "Data Source=app.db"));

        services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IStudentRepository, EfStudentRepository>();
        services.AddScoped<ILecturerRepository, EfLecturerRepository>();
        services.AddScoped<IGradeRepository, EfGradeRepository>();
        services.AddScoped<ICourseRepository, EfCourseRepository>();
        services.AddScoped<IAcademicYearRepository, EfAcademicYearRepository>();

        services.AddScoped<EfStudentRepository>();
        services.AddScoped<EfLecturerRepository>();
        services.AddScoped<EfGradeRepository>();
        services.AddScoped<EfCourseRepository>();
        services.AddScoped<EfAcademicYearRepository>();

        services.AddScoped<IUniversityUnitOfWork, EfUniversityUnitOfWork>();

        services.AddScoped<CoreApp.Services.IStudentService, CoreApp.Services.StudentService>();

        return services;
    }

    public static IServiceCollection AddUniversityMemoryModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<CoreApp.Repositories.IStudentRepository, Infrastucture.Memory.MemoryStudentRepository>();
        services.AddScoped<CoreApp.Repositories.ILecturerRepository, Infrastucture.Memory.MemoryLecturerRepository>();
        services.AddScoped<CoreApp.Repositories.IGradeRepository, Infrastucture.Memory.MemoryGradeRepository>();
        services.AddScoped<CoreApp.Repositories.ICourseRepository, Infrastucture.Memory.MemoryCourseRepository>();
        services.AddScoped<CoreApp.Repositories.IAcademicYearRepository, Infrastucture.Memory.MemoryAcademicYearRepository>();

        services.AddScoped<CoreApp.Repositories.IUniversityUnitOfWork, Infrastucture.Repository.MemoryUniversityUnitOfWork>(sp =>
        {
            var students = sp.GetRequiredService<CoreApp.Repositories.IStudentRepository>();
            var lecturers = sp.GetRequiredService<CoreApp.Repositories.ILecturerRepository>();
            var grades = sp.GetRequiredService<CoreApp.Repositories.IGradeRepository>();
            var courses = sp.GetRequiredService<CoreApp.Repositories.ICourseRepository>();
            var years = sp.GetRequiredService<CoreApp.Repositories.IAcademicYearRepository>();
            return new Infrastucture.Repository.MemoryUniversityUnitOfWork(students, lecturers, grades, courses, years);
        });

        services.AddScoped<CoreApp.Services.IStudentService, CoreApp.Services.StudentService>();

        return services;
    }
}


