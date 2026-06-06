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
    

    Task<IEnumerable<Course>> GetCoursesByLecturerAsync(Guid lecturerId);
    

    Task<IEnumerable<Student>> GetStudentsByCourseAsync(Guid lecturerId, Guid courseId);
    

    Task<bool> TeachesCourseAsync(Guid lecturerId, Guid courseId);
}
