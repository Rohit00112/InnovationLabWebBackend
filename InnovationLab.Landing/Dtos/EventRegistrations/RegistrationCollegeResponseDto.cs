using InnovationLab.Landing.Models;
using Mapster;

namespace InnovationLab.Landing.Dtos.EventRegistrations;

[AdaptFrom(typeof(RegistrationCollege))]
public record RegistrationCollegeResponseDto
(
    Guid Id,
    string Name,
    string ContactEmail,
    string Address,
    string RepresentativeName,
    string RepresentativePhone,
    string RepresentativeEmail,
    string RepresentativeDesignation,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    Guid RegistrationId
);
