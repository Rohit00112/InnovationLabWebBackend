using InnovationLab.Landing.Enums;
using InnovationLab.Landing.Models;
using Mapster;

namespace InnovationLab.Landing.Dtos.EventRegistrations;

[AdaptFrom(typeof(TeamMember))]
public record TeamMemberResponseDto
(
    Guid Id,
    string Name,
    string Faculty,
    string? Email,
    string? Phone,
    string PhotoUrl,
    Gender Gender,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    Guid RegistrationId
);