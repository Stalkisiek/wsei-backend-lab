using System;
using System.Linq;
using System.Threading.Tasks;
using CoreApp.Dto;
using CoreApp.Models;
using CoreApp.Repositories;
using AutoMapper;

namespace Infrastucture.Services;

public class MemoryStudentService : CoreApp.Services.IStudentService
{
    private readonly IUniversityUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MemoryStudentService(IUniversityUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<StudentSummaryDto>> FindAllStudentsPagedAsync(int page, int pageSize)
    {
        var people = await _unitOfWork.Students.FindPagedAsync(page, pageSize);
        var items = people.Items.Select(p => _mapper.Map<StudentSummaryDto>(p)).ToList();
        return new PagedResult<StudentSummaryDto>(items, people.TotalCount, people.Page, people.PageSize);
    }

    public async Task<StudentDetailDto?> GetStudentByIdAsync(Guid id)
    {
        var s = await _unitOfWork.Students.FindByIdAsync(id);
        if (s == null) return null;
        return _mapper.Map<StudentDetailDto>(s);
    }

    public async Task<Student?> GetStudentById(Guid id)
    {
        var s = await _unitOfWork.Students.FindByIdAsync(id);
        return s as Student;
    }

    public async Task<StudentDetailDto> CreateStudentAsync(StudentCreateDto dto)
    {
        var entity = _mapper.Map<Student>(dto);
        var added = await _unitOfWork.Students.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StudentDetailDto>(added);
    }

    public async Task<Student> AddStudent(StudentCreateDto dto)
    {
        var entity = _mapper.Map<Student>(dto);
        var added = await _unitOfWork.Students.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return added;
    }

    public async Task<StudentDetailDto> UpdateStudentAsync(Guid id, StudentUpdateDto dto)
    {
        var existing = await _unitOfWork.Students.FindByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException($"Student with id {id} not found");
        var student = (Student)existing;
        _mapper.Map(dto, student);
        var updated = await _unitOfWork.Students.UpdateAsync(student);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StudentDetailDto>(updated);
    }

    public async Task<Student> UpdateStudent(Guid id, StudentUpdateDto dto)
    {
        var existing = await _unitOfWork.Students.FindByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException($"Student with id {id} not found");
        var student = (Student)existing;
        _mapper.Map(dto, student);
        var updated = await _unitOfWork.Students.UpdateAsync(student);
        await _unitOfWork.SaveChangesAsync();
        return updated;
    }

    public async Task<StudentDetailDto> UpdateStudentStatusAsync(Guid id, StudentStatus newStatus)
    {
        var updated = await _unitOfWork.Students.UpdateStatusAsync(id, newStatus);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StudentDetailDto>(updated);
    }
}


