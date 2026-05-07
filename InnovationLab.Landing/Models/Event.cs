using System.ComponentModel.DataAnnotations;
using InnovationLab.Shared.Models;

namespace InnovationLab.Landing.Models;

public class Event : BaseModel
{
    public Guid? ParentEventId { get; set; }
    public Event? ParentEvent { get; set; }
    [Required] public required string Title { get; set; }
    [Required] public required string Description { get; set; }
    [Required][MinLength(1)][MaxLength(6)] public required IList<string> Highlights { get; set; }
    [Required] public required DateTimeOffset StartTime { get; set; }
    [Required] public required DateTimeOffset EndTime { get; set; }
    [Required] public required string Location { get; set; }
    [Required] public required string CoverImageUrl { get; set; }
    public string? SeriesName { get; set; }
    [Required] public required bool IsTeamEvent { get; set; }
    [Required] public required int MaxTeamMembers { get; set; }
    [Required] public required int MaxNumberOfTeams { get; set; }
    [Required] public required int IsRegistrationOpen { get; set; }
    public DateTimeOffset? RegistrationStart { get; set; }
    public DateTimeOffset? RegistrationEnd { get; set; }
}