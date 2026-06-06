using CoreApp.Dto;
using CoreApp.Models;
using CoreApp.Repositories;

namespace CoreApp.Services;

public class DegreeProgramService : IDegreeProgramService
{
    private readonly IUniversityUnitOfWork _unitOfWork;

    public DegreeProgramService(IUniversityUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DegreeProgramDto> CreateAsync(DegreeProgramCreateDto dto)
    {
        var code = dto.Code.Trim();
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Program code is required.");

        var existing = (await _unitOfWork.DegreePrograms.FindAllAsync())
            .FirstOrDefault(p => string.Equals(p.Code, code, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
            throw new ArgumentException($"Degree program with code {code} already exists.");

        var entity = new DegreeProgram
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = dto.Name.Trim(),
            Faculty = dto.Faculty.Trim(),
            DegreeType = dto.DegreeType,
            DurationYears = dto.DurationYears,
            MinEctsForDiploma = dto.MinEctsForDiploma,
            Courses = new List<Course>()
        };

        var added = await _unitOfWork.DegreePrograms.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return Map(added);
    }

    public async Task<IEnumerable<DegreeProgramDto>> GetAllAsync()
    {
        var items = await _unitOfWork.DegreePrograms.FindAllAsync();
        return items.Select(Map).OrderBy(x => x.Code);
    }

    public async Task<DegreeProgramReportDto?> GetReportAsync(Guid id)
    {
        var program = await _unitOfWork.DegreePrograms.FindByIdAsync(id);
        if (program == null)
            return null;

        var studentsByRelation = await _unitOfWork.Students.FindByDegreeProgramAsync(id);
        var fallbackByProgramName = (await _unitOfWork.Students.FindAllAsync())
            .Where(s => string.Equals(s.ProgramName, program.Code, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(s.ProgramName, program.Name, StringComparison.OrdinalIgnoreCase));

        var students = studentsByRelation
            .Concat(fallbackByProgramName)
            .GroupBy(s => s.Id)
            .Select(g => g.First())
            .ToList();

        return new DegreeProgramReportDto
        {
            DegreeProgramId = program.Id,
            Code = program.Code,
            Name = program.Name,
            ActiveStudentsCount = students.Count(s => s.Status == StudentStatus.Active),
            GraduatesCount = students.Count(s => s.Status == StudentStatus.Graduate)
        };
    }

    private static DegreeProgramDto Map(DegreeProgram entity)
    {
        return new DegreeProgramDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            DegreeType = entity.DegreeType,
            Faculty = entity.Faculty,
            DurationYears = entity.DurationYears,
            MinEctsForDiploma = entity.MinEctsForDiploma
        };
    }
}

