using CoreApp.Dto;

namespace CoreApp.Services;

public interface IDegreeProgramService
{
    Task<DegreeProgramDto> CreateAsync(DegreeProgramCreateDto dto);
    Task<IEnumerable<DegreeProgramDto>> GetAllAsync();
    Task<DegreeProgramReportDto?> GetReportAsync(Guid id);
}

