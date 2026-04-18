using CoreApp.Models;
using CoreApp.Repositories;

namespace Infrastucture.Memory;

public class InMemoryStudentRepository<T> : MemoryGenericRepository<T>, IStudentRepository where T : EntityBase
{
    protected Dictionary<Guid, Student> _data = new();

    public async Task<Student?> FindByIdAsync(Guid id)
    {
        var result = _data.TryGetValue(id, out var value) ? value : null;
        return await Task.FromResult(result as  Student);
    }

    public async Task<IEnumerable<Student>> FindAllAsync()
    {
        return await Task.FromResult(_data.Values.OfType<Student>().ToList());
    }

    public async Task<PagedResult<Student>> FindPagedAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        var total = _data.Count;
        var items = _data.Values.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var result = new PagedResult<Student>(items, total, page, pageSize);
        return await Task.FromResult(result as PagedResult<Student>);
    }

    public async Task<Student> AddAsync(Student entity)
    {
        if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
        _data[entity.Id] = entity;
        return await Task.FromResult(entity);
    }

    public async Task<Student> UpdateAsync(Student entity)
    {
        _data[entity.Id] = entity;
        return await Task.FromResult(entity);
    }

    public async Task<IEnumerable<Student>> FindByAcademicYearAsync(Guid academicYearId)
    {
        var result =  _data.Values.OfType<Student>().Where(s => s.AcademicYear != null && s.AcademicYear.Id == academicYearId).ToList();
        return await Task.FromResult(result);
    }

    public async Task<IEnumerable<Student>> FindByDegreeProgramAsync(Guid degreeProgramId)
    {
        var result = _data.Values.OfType<Student>().Where(s => s.DegreeProgram != null && s.DegreeProgram.Id == degreeProgramId).ToList();
        return await Task.FromResult(result);
    }

    public async Task<Student> UpdateStatusAsync(Guid studentId, StudentStatus newStatus)
    {
        if (_data.TryGetValue(studentId, out var value) && value is Student student)
        {
            student.Status = newStatus;
            _data[studentId] = student;
            return await Task.FromResult(student);
        }
        throw new KeyNotFoundException($"Student with id {studentId} not found.");
    }
}