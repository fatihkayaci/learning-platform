using System.Text.Json;
using Enrollment.Application.Common.Interfaces;

namespace Enrollment.Infrastructure.Services;

public class CourseService : ICourseService
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CourseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ExistsAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/courses/{courseId}", cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<int> GetLessonCountAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/courses/{courseId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
            return 0;

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        JsonDocument document = JsonDocument.Parse(json);

        if (document.RootElement.TryGetProperty("lessons", out JsonElement lessonsElement))
            return lessonsElement.GetArrayLength();

        return 0;
    }
}
