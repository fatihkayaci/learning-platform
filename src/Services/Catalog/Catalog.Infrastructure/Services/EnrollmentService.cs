using Catalog.Application.Common.Interfaces;
namespace Catalog.Infrastructure.Services;
public class EnrollmentService : IEnrollmentService
{
    private readonly HttpClient _httpClient;
    public EnrollmentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<bool> IsEnrolledAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"enrollments/check?studentId={studentId}&courseId={courseId}", cancellationToken);
        if (!response.IsSuccessStatusCode) return false;
        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        return bool.Parse(json);
    }
}