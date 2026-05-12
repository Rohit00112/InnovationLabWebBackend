using System.ComponentModel.DataAnnotations;
using InnovationLab.Shared.Enums;
using InnovationLab.Shared.Models;

namespace InnovationLab.Landing.Models;

public class EventGallery : BaseModel
{
    public Guid? EventId { get; set; }
    public Event? ParentEvent { get; set; }
    [Required] public required string Title { get; set; }
    [Required] public required string SubTitle { get; set; }
    [Required] public required string Caption { get; set; }
    [Required] public required string ImageUrl { get; set; }
    [Required] public MediaType Type { get; set; }
}
