using System.ComponentModel.DataAnnotations;
using InnovationLab.Landing.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace InnovationLab.Landing.Dtos.EventRegistrations;

/// <summary>
/// DTO for creating a registration college entry
/// </summary>
[AdaptTo(typeof(RegistrationCollege))]
public record RegistrationCollegeCreateDto
(
    [Required] string Name,
    [Required][EmailAddress] string ContactEmail,
    [Required] string Address,
    [Required] string RepresentativeName,
    [Required][Phone] string RepresentativePhone,
    [Required][EmailAddress] string RepresentativeEmail,
    [Required] string RepresentativeDesignation
);
