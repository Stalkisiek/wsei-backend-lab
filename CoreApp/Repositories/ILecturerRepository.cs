using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.Models;

namespace CoreApp.Repositories;

public interface ILecturerRepository : IGenericRepositoryAsync<Lecturer>
{
    Task<IEnumerable<Lecturer>> FindByCourseAsync(Guid courseId);
    Task<IEnumerable<Lecturer>> FindByTitleAsync(string title);
    Task<IEnumerable<Lecturer>> FindByFacultyAsync(string faculty);
}

