using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.Models;

namespace CoreApp.Repositories;

public interface ICourseRepository : IGenericRepositoryAsync<Course>
{
    Task<IEnumerable<Course>> FindByDegreeProgramAsync(Guid degreeProgramId);
    Task<IEnumerable<Course>> FindByAcademicYearAsync(Guid academicYearId);
    Task<IEnumerable<Course>> FindByLecturerAsync(Guid lecturerId);
    Task<Course?> FindByCodeAsync(string code);
}