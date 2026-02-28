using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApp.Repositories;
using CoreApp.Models;

namespace Infrastucture.Repository;

public class MemoryStudentRepository : IGenericRepository<Student>
{
    private static readonly List<Student> _students = new()
    {
        new Student { Id = 1, Name = "John", Surname = "Doe", BirthDate = new DateOnly(2000, 1, 1), Email = "" }
    };

    public async Task<Student?> AddAsync(Student entity)
    {
        if (entity == null) return null;
        entity.Id = _students.Any() ? _students.Max(s => s.Id) + 1 : 1;
        _students.Add(entity);
        return await Task.FromResult(entity);
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        try
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            return await Task.FromResult(student);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while retrieving the student: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await Task.FromResult(_students.AsEnumerable());
    }

    public async Task DeleteAsync(int id)
    {
        var student = _students.FirstOrDefault(s => s.Id == id);
        if (student != null)
        {
            _students.Remove(student);
        }
        await Task.CompletedTask;
    }

    public async Task<Student?> UpdateAsync(Student entity)
    {
        if (entity == null) return null;
        var existing = _students.FirstOrDefault(s => s.Id == entity.Id);
        if (existing == null) return null;

        // update fields
        existing.Name = entity.Name;
        existing.Surname = entity.Surname;
        existing.BirthDate = entity.BirthDate;
        existing.Email = entity.Email;

        return await Task.FromResult(existing);
    }
}
