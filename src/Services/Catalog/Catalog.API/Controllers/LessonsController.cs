using Catalog.Application.Commands.AddLesson;
using Catalog.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[Route("api/lessons")]
[ApiController]
[Authorize]
public class LessonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LessonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> Create([FromBody] AddLessonCommand command, CancellationToken cancellationToken)
    {
        LessonDto response = await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, response);
    }
}