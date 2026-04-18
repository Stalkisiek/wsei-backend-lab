using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using CoreApp.Models;
using CoreApp.Repositories;
using Infrastucture.Memory;

namespace UnitTest;

public class MemoryGenericRepositoryTest
{
    private IGenericRepositoryAsync<Student> _repo = new MemoryGenericRepository<Student>();

    [Fact]
    public async Task AddStudentToRepositoryTestAsync()
    {
        var expected = new Student()
        {
            StudentId = Guid.NewGuid(),
            YearOfStudy = 2,
            ProgramName = "Informatyka",
            Status = StudentStatus.Active,
            Grades = new List<Grade>(),
            FirstName = "Ivan",
            LastName = "Ivanov",
            NationalId = "Ivanov",
            Email = "test@test.test"
        };
        await _repo.AddAsync(expected);
        var actual = await _repo.FindByIdAsync(expected.Id);
        Assert.Equal(expected, actual);
        Assert.Equal(expected.Id, actual?.Id);
    }

    [Fact]
    public async Task GenericRepository_OtherMethods_TestAsync()
    {
        _repo = new MemoryGenericRepository<Student>();
        var students = Enumerable.Range(1, 7).Select(i => new Student
        {
            StudentId = Guid.NewGuid(),
            ProgramName = $"Program{i}",
            YearOfStudy = i % 4 + 1,
            Status = StudentStatus.Active,
            Grades = new List<Grade>(),
            FirstName = $"First{i}",
            LastName = $"Last{i}",
            NationalId = $"NID{i}",
            Email = $"student{i}@example.com"
        }).ToList();
        foreach (var s in students) await _repo.AddAsync(s);
        var all = (await _repo.FindAllAsync()).ToList();
        Assert.Equal(7, all.Count);
        var paged = await _repo.FindPagedAsync(2, 3);
        Assert.Equal(7, paged.TotalCount);
        Assert.Equal(3, paged.Items.Count);
        var first = students[0];
        first.ProgramName = "UpdatedProgram";
        await _repo.UpdateAsync(first);
        var updated = await _repo.FindByIdAsync(first.Id);
        Assert.Equal("UpdatedProgram", updated?.ProgramName);
        var toRemove = students[1];
        await _repo.RemoveByIdAsync(toRemove.Id);
        var removed = await _repo.FindByIdAsync(toRemove.Id);
        Assert.Null(removed);
    }
}