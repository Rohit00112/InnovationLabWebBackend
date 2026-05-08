using InnovationLab.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace InnovationLab.Landing.Models;

[Index(nameof(Priority), IsUnique = true)]
public class Company : BaseModel
{
    public required string Name { get; set; }
    public required string About { get; set; }
    public required string Address { get; set; }
    public required string LogoUrl { get; set; }
    public int Priority { get; set; }
    public string? ContactEmail { get; set; }
    public string? WebsiteUrl { get; set; }
    public required bool IsMouSigned { get; set; }
    public required bool IsJobFair { get; set; }
    public int? NumberOfInterns { get; set; }
    public int? NumberOfVacancies { get; set; }
    public IList<string?>? OpeningUrls { get; set; }
}