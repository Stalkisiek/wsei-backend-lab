using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.Models;
using CoreApp.Repositories;

namespace Infrastucture.Memory;

public class MemoryLecturerRepository : MemoryGenericRepository<Lecturer>, ILecturerRepository
{
    public MemoryLecturerRepository() : base()
    {
    }

    public Task<IEnumerable<Lecturer>> FindByCourseAsync(Guid courseId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Lecturer>> FindByTitleAsync(string title)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Lecturer>> FindByFacultyAsync(string faculty)
    {
        throw new NotImplementedException();
    }
}

