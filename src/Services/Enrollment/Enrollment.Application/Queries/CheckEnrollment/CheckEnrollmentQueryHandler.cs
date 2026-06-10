using Enrollment.Application.Common.Interfaces;
using MediatR;

namespace Enrollment.Application.Queries.CheckEnrollment;

public class CheckEnrollmentQueryHandler : IRequestHandler<CheckEnrollmentQuery, bool>
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public CheckEnrollmentQueryHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<bool> Handle(CheckEnrollmentQuery request, CancellationToken cancellationToken)
    {
        return await _enrollmentRepository.ExistsAsync(request.StudentId, request.CourseId, cancellationToken);
    }
}
