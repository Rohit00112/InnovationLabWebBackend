using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InnovationLab.Landing.Enums;
using InnovationLab.Shared.Models;

namespace InnovationLab.Landing.Models;

public class EventRegistration : BaseModel
{
    [Required] public required Guid EventId { get; set; }
    [ForeignKey(nameof(EventId))] public Event? Event { get; set; }
    [Required] public required EventRegistrationType Type { get; set; }
    [Required][MinLength(5)][MaxLength(30)] public required string TeamName { get; set; }
    [Required][MinLength(5)][MaxLength(30)] public required string Name { get; set; }
    [Required][EmailAddress] public required string Email { get; set; }
    [Phone] public string? Phone { get; set; }
    public EventRegistrationStatus Status { get; set; } = EventRegistrationStatus.Pending;
    public required IList<string?> DocumentUrls { get; set; } = [];

    public IList<TeamMember>? Members { get; set; }
    public IList<RegistrationCollege>? RegistrationColleges { get; set; }
}