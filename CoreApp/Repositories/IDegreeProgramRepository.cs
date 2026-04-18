using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.Models;

namespace CoreApp.Repositories;

public interface IDegreeProgramRepository : IGenericRepositoryAsync<DegreeProgram>
{
    /// <summary>
    /// Zwraca programy kształcenia dostępne na danym wydziale.
    /// </summary>
    Task<IEnumerable<DegreeProgram>> FindByFacultyAsync(string faculty);

    /// <summary>
    /// Zwraca programy po typie stopnia (Bachelor, Master, itp.).
    /// </summary>
    Task<IEnumerable<DegreeProgram>> FindByDegreeTypeAsync(DegreeType degreeType);

    /// <summary>
    /// Zwraca programy zawierające dany kurs.
    /// </summary>
    Task<IEnumerable<DegreeProgram>> FindByCourseAsync(Guid courseId);
}

