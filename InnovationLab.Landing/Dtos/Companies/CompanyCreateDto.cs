using System.ComponentModel.DataAnnotations;
using InnovationLab.Landing.Models;
using Mapster;

namespace InnovationLab.Landing.Dtos.Companies;

[AdaptTo(typeof(Company))]
public record CompanyCreateDto
(
    [Required] string Name,
    [Required] string About,
    [Required] string Address,
    [EmailAddress] string? ContactEmail,
    string? WebsiteUrl,
    [Required] bool IsMouSigned,
    [Required] bool IsJobFair,
    int? NumberOfInterns,
    int? NumberOfVacancies,
    IList<string?>? OpeningUrls,
    [Required] IFormFile Logo,
    [Range(0, int.MaxValue)] int Priority
);
