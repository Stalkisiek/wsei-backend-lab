using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApp.Models;
using CoreApp.Repositories;

namespace Infrastucture.Memory;

public class MemoryAcademicYearRepository : MemoryGenericRepository<AcademicYear>, IAcademicYearRepository
{
    public MemoryAcademicYearRepository() : base()
    {
        var id1 = Guid.NewGuid();
        var y1 = new AcademicYear
        {
            Id = id1,
            YearFrom = 2022,
            YearTo = 2023,
            IsActive = false,
            Name = "2022/2023"
        };
        _data.Add(y1.Id, y1);

        var id2 = Guid.NewGuid();
        var y2 = new AcademicYear
        {
            Id = id2,
            YearFrom = 2023,
            YearTo = 2024,
            IsActive = true,
            Name = "2023/2024"
        };
        _data.Add(y2.Id, y2);
    }
}

