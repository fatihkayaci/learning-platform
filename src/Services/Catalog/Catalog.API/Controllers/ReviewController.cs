using Catalog.Application.Commands.AddReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[Route("api/reviews")]
[ApiController]
[Authorize]
public class ReviewController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> AddReview([FromBody] AddReviewCommand command, CancellationToken cancellationToken)
    {
        Guid reviewId = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(AddReview), new { id = reviewId }, new { id = reviewId });
    }
}
