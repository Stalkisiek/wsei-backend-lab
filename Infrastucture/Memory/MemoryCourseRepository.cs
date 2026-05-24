using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.Models;
using CoreApp.Repositories;

namespace Infrastucture.Memory;

public class MemoryCourseRepository : MemoryGenericRepository<Course>, ICourseRepository
{
    public MemoryCourseRepository() : base()
    {
    }

    public Task<IEnumerable<Course>> FindByDegreeProgramAsync(Guid degreeProgramId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Course>> FindByAcademicYearAsync(Guid academicYearId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Course>> FindByLecturerAsync(Guid lecturerId)
    {
        throw new NotImplementedException();
    }

    public Task<Course?> FindByCodeAsync(string code)
    {
        throw new NotImplementedException();
    }
}

