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
}

