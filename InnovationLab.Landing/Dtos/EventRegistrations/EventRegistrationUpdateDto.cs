using System.ComponentModel.DataAnnotations;
using InnovationLab.Landing.Models;
using Mapster;
using InnovationLab.Landing.Enums;

namespace InnovationLab.Landing.Dtos.EventRegistrations;

[AdaptTo(typeof(EventRegistration))]
public record EventRegistrationUpdateDto
(
    [MinLength(5)][MaxLength(30)] string? TeamName,
    [MinLength(5)][MaxLength(30)] string? Name,
    [EmailAddress] string? Email,
    [Phone] string? Phone,
    [MinLength(1)] IList<IFormFile>? Documents,
    EventRegistrationStatus? Status
);