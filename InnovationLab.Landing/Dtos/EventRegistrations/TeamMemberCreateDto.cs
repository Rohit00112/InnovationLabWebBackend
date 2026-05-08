using System.ComponentModel.DataAnnotations;
using InnovationLab.Landing.Enums;
using InnovationLab.Landing.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace InnovationLab.Landing.Dtos.EventRegistrations;

/// <summary>
/// DTO for creating a team member with photo upload
/// </summary>
[AdaptTo(typeof(TeamMember))]
public record TeamMemberCreateDto
(
    [Required] string Name,
    [Required] string Faculty,
    [EmailAddress] string? Email,
    [Phone] string? Phone,
    [Required] IFormFile Photo,
    [Required] Gender Gender
);