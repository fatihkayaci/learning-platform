using Enrollment.Application.Common.Interfaces;

namespace Enrollment.Infrastructure.Services;

public class CourseService : ICourseService
{
    private readonly HttpClient _httpClient;

    public CourseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ExistsAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/courses/{courseId}", cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
