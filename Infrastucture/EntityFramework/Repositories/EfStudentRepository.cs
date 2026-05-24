using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoreApp.Models;
using CoreApp.Repositories;
using Infrastucture.EntityFramework.Context;

namespace Infrastucture.EntityFramework.Repositories;

public class EfStudentRepository : EfGenericRepository<Student>, IStudentRepository
{
    private readonly AppDbContext _appContext;

    public EfStudentRepository(AppDbContext context) : base(context, context.Set<Student>())
    {
        _appContext = context;
    }

    public async Task<IEnumerable<Student>> FindByAcademicYearAsync(Guid academicYearId)
    {
        return await _set.Where(s => s.AcademicYear != null && s.AcademicYear.Id == academicYearId).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Student>> FindByDegreeProgramAsync(Guid degreeProgramId)
    {
        return await _set.Where(s => s.DegreeProgram != null && s.DegreeProgram.Id == degreeProgramId).AsNoTracking().ToListAsync();
    }

    public async Task<Student> UpdateStatusAsync(Guid studentId, StudentStatus newStatus)
    {
        var student = await _set.FindAsync(studentId);
        if (student == null) throw new KeyNotFoundException($"Student with id {studentId} not found");
        student.Status = newStatus;
        _set.Update(student);
        return student;
    }
}

