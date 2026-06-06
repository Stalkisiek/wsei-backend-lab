using CoreApp.Models;
using CoreApp.Repositories;
using Infrastucture.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.EntityFramework.Repositories;

public class EfDegreeProgramRepository : EfGenericRepository<DegreeProgram>, IDegreeProgramRepository
{
    public EfDegreeProgramRepository(AppDbContext context) : base(context, context.Set<DegreeProgram>())
    {
    }

    public async Task<IEnumerable<DegreeProgram>> FindByFacultyAsync(string faculty)
    {
        return await _set
            .Where(p => p.Faculty.Equals(faculty, StringComparison.OrdinalIgnoreCase))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<DegreeProgram>> FindByDegreeTypeAsync(DegreeType degreeType)
    {
        return await _set
            .Where(p => p.DegreeType == degreeType)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<DegreeProgram>> FindByCourseAsync(Guid courseId)
    {
        return await _set
            .Where(p => p.Courses.Any(c => c.Id == courseId))
            .AsNoTracking()
            .ToListAsync();
    }
}

