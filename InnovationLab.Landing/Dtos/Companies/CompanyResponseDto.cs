using InnovationLab.Landing.Models;
using Mapster;

namespace InnovationLab.Landing.Dtos.Companies;

[AdaptFrom(typeof(Company))]
public record CompanyResponseDto
(
    Guid Id,
    string Name,
    string About,
    string Address,
    string? ContactEmail,
    string? WebsiteUrl,
    string LogoUrl,
    int Priority,
    bool IsMouSigned,
    bool IsJobFair,
    int? NumberOfInterns,
    int? NumberOfVacancies,
    IList<string?>? OpeningUrls,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? DeletedAt
);

