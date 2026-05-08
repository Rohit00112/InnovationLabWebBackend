using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InnovationLab.Landing.Enums;
using InnovationLab.Shared.Models;

namespace InnovationLab.Landing.Models;

public class TeamMember : BaseModel
{
    [Required] public required Guid RegistrationId { get; set; }
    [ForeignKey(nameof(RegistrationId))] public EventRegistration? Registration { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public required string Faculty { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    [Required] public required string PhotoUrl { get; set; }
    [Required] public required Gender Gender { get; set; }
}
