using Catalog.Application.DTOs;
using MediatR;

namespace Catalog.Application.Commands.AddLesson;
public record AddLessonCommand(string Title, string VideoUrl, int Order, Guid CourseId) : IRequest<LessonDto>;