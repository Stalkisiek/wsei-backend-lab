using CoreApp.Models;
using CoreApp.Repositories;

namespace Infrastucture.Memory;

public class MemoryDegreeProgramRepository : MemoryGenericRepository<DegreeProgram>, IDegreeProgramRepository
{
    public MemoryDegreeProgramRepository()
    {
        var program = new DegreeProgram
        {
            Id = Guid.NewGuid(),
            Code = "INF-BSC",
            Name = "Informatyka",
            DegreeType = DegreeType.Engineering,
            Faculty = "Wydzial Informatyki",
            DurationYears = 3,
            MinEctsForDiploma = 180,
            Courses = new List<Course>()
        };

        _data[program.Id] = program;
    }

    public Task<IEnumerable<DegreeProgram>> FindByFacultyAsync(string faculty)
    {
        var result = _data.Values.Where(p => string.Equals(p.Faculty, faculty, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult<IEnumerable<DegreeProgram>>(result.ToList());
    }

    public Task<IEnumerable<DegreeProgram>> FindByDegreeTypeAsync(DegreeType degreeType)
    {
        var result = _data.Values.Where(p => p.DegreeType == degreeType);
        return Task.FromResult<IEnumerable<DegreeProgram>>(result.ToList());
    }

    public Task<IEnumerable<DegreeProgram>> FindByCourseAsync(Guid courseId)
    {
        var result = _data.Values.Where(p => p.Courses.Any(c => c.Id == courseId));
        return Task.FromResult<IEnumerable<DegreeProgram>>(result.ToList());
    }
}

