using Catalog.Application.DTOs;
using Catalog.Application.Queries.GetAllCourses;
using Catalog.Application.Queries.GetCourseById;
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        CourseDetailDto? response = await _mediator.Send(new GetCourseByIdQuery(id), cancellationToken);

        if (response is null)
            return NotFound();

        return Ok(response);
    }
}