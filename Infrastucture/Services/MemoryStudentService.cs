using System;
using System.Linq;
using System.Threading.Tasks;
using CoreApp.Dto;
using CoreApp.Models;
using CoreApp.Repositories;

namespace Infrastucture.Services;

public class MemoryStudentService : CoreApp.Services.IStudentService
{
    private readonly IUniversityUnitOfWork _unitOfWork;

    public MemoryStudentService(IUniversityUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<StudentSummaryDto>> FindAllStudentsPagedAsync(int page, int pageSize)
    {
        var people = await _unitOfWork.Students.FindPagedAsync(page, pageSize);
        var items = people.Items.Select(p => StudentSummaryDto.FromEntity((Student)p)).ToList();
        return new PagedResult<StudentSummaryDto>(items, people.TotalCount, people.Page, people.PageSize);
    }

    public async Task<StudentDetailDto?> GetStudentByIdAsync(Guid id)
    {
        var s = await _unitOfWork.Students.FindByIdAsync(id);
        if (s == null) return null;
        return StudentDetailDto.FromEntity((Student)s);
    }

    public async Task<StudentDetailDto> CreateStudentAsync(StudentCreateDto dto)
    {
        var entity = StudentCreateDto.ToEntity(dto);
        var added = await _unitOfWork.Students.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return StudentDetailDto.FromEntity(added);
    }

    public async Task<StudentDetailDto> UpdateStudentAsync(Guid id, StudentUpdateDto dto)
    {
        var existing = await _unitOfWork.Students.FindByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException($"Student with id {id} not found");
        var student = (Student)existing;
        student.FirstName = dto.FirstName;
        student.LastName = dto.LastName;
        student.Email = dto.Email;
        student.YearOfStudy = dto.YearOfStudy;
        student.Status = dto.Status;
        student.ProgramName = dto.ProgramCode;
        var updated = await _unitOfWork.Students.UpdateAsync(student);
        await _unitOfWork.SaveChangesAsync();
        return StudentDetailDto.FromEntity(updated);
    }

    public async Task<StudentDetailDto> UpdateStudentStatusAsync(Guid id, StudentStatus newStatus)
    {
        var updated = await _unitOfWork.Students.UpdateStatusAsync(id, newStatus);
        await _unitOfWork.SaveChangesAsync();
        return StudentDetailDto.FromEntity(updated);
    }
}


