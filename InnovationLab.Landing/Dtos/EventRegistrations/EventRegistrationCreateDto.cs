using System.ComponentModel.DataAnnotations;
using InnovationLab.Landing.Enums;
using InnovationLab.Landing.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace InnovationLab.Landing.Dtos.EventRegistrations;

/// <summary>
/// DTO for creating a new event registration with documents and team members
/// </summary>
[AdaptTo(typeof(EventRegistration))]
public record EventRegistrationCreateDto
(
    [Required] Guid EventId,
    [Required] EventRegistrationType Type,
    [Required][MinLength(5)][MaxLength(30)] string TeamName,
    [Required][MinLength(5)][MaxLength(30)] string Name,
    [Required][EmailAddress] string Email,
    [Phone] string? Phone,
    [MinLength(1)] IList<IFormFile>? Documents,
    [MinLength(1)] List<TeamMemberCreateDto>? Members,
    [MinLength(1)] List<RegistrationCollegeCreateDto>? RegistrationColleges
);