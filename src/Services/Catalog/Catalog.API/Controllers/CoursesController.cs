using Catalog.Application.Commands.CreateCourse;
using Catalog.Application.Commands.DeleteCourse;
using Catalog.Application.Commands.UpdateCourse;
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
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseCommand command, CancellationToken cancellationToken)
    {
        CourseDto response = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCourseCommand command, CancellationToken cancellationToken)
    {
        CourseDto response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteCourseCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
}