using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InnovationLab.Shared.Models;

namespace InnovationLab.Landing.Models;

public class RegistrationCollege : BaseModel
{
    [Required] public required Guid RegistrationId { get; set; }
    [ForeignKey(nameof(RegistrationId))] public EventRegistration? Registration { get; set; }
    [Required] public required string Name { get; set; }
    [Required][EmailAddress] public required string ContactEmail { get; set; }
    [Required] public required string Address { get; set; }
    [Required] public required string RepresentativeName { get; set; }
    [Required][Phone] public required string RepresentativePhone { get; set; }
    [Required][EmailAddress] public required string RepresentativeEmail { get; set; }
    [Required] public required string RepresentativeDesignation { get; set; }
}