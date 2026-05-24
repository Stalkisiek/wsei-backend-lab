using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace CoreApp.Module;

public static class StudentModule
{
    public static IServiceCollection AddStudentsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssemblyContaining<CoreApp.Validators.StudentCreateDtoValidator>();
        services.AddFluentValidationAutoValidation();
        return services;
    }
}



