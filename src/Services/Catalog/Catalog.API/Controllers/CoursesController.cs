using Catalog.Application.DTOs;
using Catalog.Application.Queries.GetAllCourses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[Route("api/courses")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        IReadOnlyList<CourseDto> response = await _mediator.Send(new GetAllCoursesQuery(), cancellationToken);
        return Ok(response);
    }
}