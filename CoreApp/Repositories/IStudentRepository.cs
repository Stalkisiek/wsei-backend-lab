using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.Models;

namespace CoreApp.Repositories;

public interface IStudentRepository : IGenericRepositoryAsync<Student>
{
    Task<IEnumerable<Student>> FindByAcademicYearAsync(Guid academicYearId);
    Task<IEnumerable<Student>> FindByDegreeProgramAsync(Guid degreeProgramId);
    Task<Student> UpdateStatusAsync(Guid studentId, StudentStatus newStatus);
}

