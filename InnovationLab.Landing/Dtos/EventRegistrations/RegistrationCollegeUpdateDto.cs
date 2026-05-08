using System.ComponentModel.DataAnnotations;
using InnovationLab.Landing.Models;
using Mapster;

namespace InnovationLab.Landing.Dtos.EventRegistrations;

[AdaptTo(typeof(RegistrationCollege))]
public record RegistrationCollegeUpdateDto
(
    string? Name,
    [EmailAddress] string? ContactEmail,
    string? Address,
    string? RepresentativeName,
    [Phone] string? RepresentativePhone,
    [EmailAddress] string? RepresentativeEmail,
    string? RepresentativeDesignation
);
