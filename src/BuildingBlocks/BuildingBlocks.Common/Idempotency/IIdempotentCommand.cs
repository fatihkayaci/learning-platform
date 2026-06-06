using MediatR;

namespace BuildingBlocks.Common.Idempotency;

public interface IIdempotentCommand<TResponse> : IRequest<TResponse>;
