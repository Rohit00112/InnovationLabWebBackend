using InnovationLab.Landing.Models;
using Mapster;
using InnovationLab.Landing.Enums;

namespace InnovationLab.Landing.Dtos.EventRegistrations;

[AdaptFrom(typeof(EventRegistration))]
public record EventRegistrationResponseDto
(
    Guid Id,
    Guid EventId,
    Event? Event,
    EventRegistrationType Type,
    string TeamName,
    string Name,
    string Email,
    string? Phone,
    IList<string> DocumentUrls,
    IList<TeamMemberResponseDto>? Members,
    IList<RegistrationCollegeResponseDto>? RegistrationColleges,
    EventRegistrationStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);