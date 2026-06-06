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
        var id1 = Guid.NewGuid();
        var l1 = new Lecturer
        {
            Id = id1,
            FirstName = "Jan",
            LastName = "Kowal",
            Email = "jan.kowal@wsei.edu.pl",
            Title = "Dr",
            Faculty = "Wydział Informatyki",
            TaughtCorses = new List<Course>()
        };
        _data.Add(l1.Id, l1);

        var id2 = Guid.NewGuid();
        var l2 = new Lecturer
        {
            Id = id2,
            FirstName = "Anna",
            LastName = "Nowak",
            Email = "anna.nowak@wsei.edu.pl",
            Title = "Prof",
            Faculty = "Wydział Matematyki",
            TaughtCorses = new List<Course>()
        };
        _data.Add(l2.Id, l2);
    }

    public Task<IEnumerable<Lecturer>> FindByCourseAsync(Guid courseId)
    {
        var result = _data.Values.Where(l => l.TaughtCorses != null && l.TaughtCorses.Any(c => c.Id == courseId)).ToList();
        return Task.FromResult<IEnumerable<Lecturer>>(result);
    }

    public Task<IEnumerable<Lecturer>> FindByTitleAsync(string title)
    {
        var result = _data.Values.Where(l => string.Equals(l.Title, title, StringComparison.OrdinalIgnoreCase)).ToList();
        return Task.FromResult<IEnumerable<Lecturer>>(result);
    }

    public Task<IEnumerable<Lecturer>> FindByFacultyAsync(string faculty)
    {
        var result = _data.Values.Where(l => string.Equals(l.Faculty, faculty, StringComparison.OrdinalIgnoreCase)).ToList();
        return Task.FromResult<IEnumerable<Lecturer>>(result);
    }

    public Task<IEnumerable<Course>> GetCoursesByLecturerAsync(Guid lecturerId)
    {
        var lecturer = _data.Values.FirstOrDefault(l => l.Id == lecturerId);
        var courses = lecturer?.TaughtCorses ?? new List<Course>();
        return Task.FromResult<IEnumerable<Course>>(courses);
    }

    public Task<IEnumerable<Student>> GetStudentsByCourseAsync(Guid lecturerId, Guid courseId)
    {
        var lecturer = _data.Values.FirstOrDefault(l => l.Id == lecturerId);
        var course = lecturer?.TaughtCorses?.FirstOrDefault(c => c.Id == courseId);
        var students = course?.Enrollments ?? new List<Student>();
        return Task.FromResult<IEnumerable<Student>>(students);
    }

    public Task<bool> TeachesCourseAsync(Guid lecturerId, Guid courseId)
    {
        var lecturer = _data.Values.FirstOrDefault(l => l.Id == lecturerId);
        var teaches = lecturer?.TaughtCorses?.Any(c => c.Id == courseId) ?? false;
        return Task.FromResult(teaches);
    }
}
