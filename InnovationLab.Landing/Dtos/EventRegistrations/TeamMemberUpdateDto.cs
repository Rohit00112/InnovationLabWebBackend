using System.ComponentModel.DataAnnotations;
using InnovationLab.Landing.Enums;
using InnovationLab.Landing.Models;
using Mapster;

namespace InnovationLab.Landing.Dtos.EventRegistrations;

[AdaptTo(typeof(TeamMember))]
public record TeamMemberUpdateDto
(
    string? Name,
    string? Faculty,
    [EmailAddress] string? Email,
    [Phone] string? Phone,
    IFormFile? Photo,
    Gender? Gender
);
