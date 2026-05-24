using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoreApp.Models;
using CoreApp.Repositories;
using Infrastucture.EntityFramework.Context;

namespace Infrastucture.EntityFramework.Repositories;

public class EfAcademicYearRepository : EfGenericRepository<AcademicYear>, IAcademicYearRepository
{
    public EfAcademicYearRepository(AppDbContext context) : base(context, context.Set<AcademicYear>()) { }
}

