using InnovationLab.Shared.Enums;
using Mapster;

namespace InnovationLab.Landing.Dtos.EventGallery;

[AdaptFrom(typeof(Models.EventGallery))]
public record EventGalleryResponseDto
(
    Guid Id,
    Guid? EventId,
    string Title,
    string SubTitle,
    string Caption,
    string ImageUrl,
    MediaType Type,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);
