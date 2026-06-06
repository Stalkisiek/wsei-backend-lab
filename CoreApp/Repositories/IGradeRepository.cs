using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.Models;

namespace CoreApp.Repositories;

public interface IGradeRepository : IGenericRepositoryAsync<Grade>
{
    Task<IEnumerable<Grade>> FindByCourseAsync(Guid courseId);
    Task<IEnumerable<Grade>> FindByStudentAsync(Guid studentId);


    Task<IEnumerable<Grade>> FindByAcademicYearAsync(Guid academicYearId);
    
    Task<double?> GetAverageForStudentAsync(Guid studentId);
    
    Task<IEnumerable<Grade>> FindByStudentAndCourseAsync(Guid studentId, Guid courseId);
    
    Task AddGradeChangeAsync(Guid gradeId, GradeValue? previousValue, GradeValue newValue, string changedBy);
    
    Task<IEnumerable<GradeChangeHistory>> GetChangeHistoryAsync(Guid gradeId);
}