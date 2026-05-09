using InnovationLab.Landing.Models;
using Mapster;

namespace InnovationLab.Landing.Dtos.Events;

[AdaptFrom(typeof(Event))]
public record EventResponseDto
(
    Guid Id,
    Guid? ParentEventId,
    string Title,
    string Description,
    IList<string> Highlights,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string Location,
    string CoverImageUrl,
    string? SeriesName,
    bool IsTeamEvent,
    int MaxTeamMembers,
    int MaxNumberOfTeams,
    bool IsRegistrationOpen,
    DateTimeOffset? RegistrationStart,
    DateTimeOffset? RegistrationEnd,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);