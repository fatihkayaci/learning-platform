using BuildingBlocks.Common.Idempotency;

namespace Catalog.Application.Commands.AddReview;

public record AddReviewCommand(Guid CourseId, int Rating, string Comment):IIdempotentCommand<Guid>;