using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoreApp.Models;
using CoreApp.Repositories;
using Infrastucture.EntityFramework.Context;

namespace Infrastucture.EntityFramework.Repositories;

public class EfLecturerRepository : EfGenericRepository<Lecturer>, ILecturerRepository
{
    public EfLecturerRepository(AppDbContext context) : base(context, context.Set<Lecturer>()) { }

    public async Task<IEnumerable<Lecturer>> FindByCourseAsync(Guid courseId)
    {
        return await _set.Where(l => l.TaughtCorses != null && l.TaughtCorses.Any(c => c.Id == courseId)).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Lecturer>> FindByTitleAsync(string title)
    {
        return await _set.Where(l => l.Title != null && l.Title.Equals(title, StringComparison.OrdinalIgnoreCase)).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Lecturer>> FindByFacultyAsync(string faculty)
    {
        return await _set.Where(l => l.Faculty != null && l.Faculty.Equals(faculty, StringComparison.OrdinalIgnoreCase)).AsNoTracking().ToListAsync();
    }
}

