using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.Models;

namespace CoreApp.Repositories;

public interface IGradeRepository : IGenericRepositoryAsync<Grade>
{
    Task<IEnumerable<Grade>> FindByCourseAsync(Guid courseId);
    Task<IEnumerable<Grade>> FindByStudentAsync(Guid studentId);

    /// <summary>
    /// Zwraca oceny z danego roku akademickiego.
    /// </summary>
    Task<IEnumerable<Grade>> FindByAcademicYearAsync(Guid academicYearId);

    /// <summary>
    /// Oblicza średnią ocen dla studenta (null jeśli brak ocen).
    /// </summary>
    Task<double?> GetAverageForStudentAsync(Guid studentId);
}