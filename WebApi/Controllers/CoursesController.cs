using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreApp.Repositories;
using CoreApp.Dto;
using CoreApp.Authorization;
using AutoMapper;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseRepository _repo;
    private readonly IMapper _mapper;

    public CoursesController(ICourseRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Policy = nameof(AppPolicies.Administrator))]
    public async Task<IActionResult> GetAll()
    {
        var items = await _repo.FindAllAsync();
        var dtos = items.Select(c => new CourseDto { Id = c.Id, Code = c.Code, Name = c.Name }).ToList();
        return Ok(dtos);
    }
}

