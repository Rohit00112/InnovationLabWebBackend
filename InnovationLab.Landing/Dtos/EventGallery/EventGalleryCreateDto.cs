using System.ComponentModel.DataAnnotations;
using InnovationLab.Shared.Enums;
using Mapster;

namespace InnovationLab.Landing.Dtos.EventGallery;

[AdaptTo(typeof(Models.EventGallery))]
public record EventGalleryCreateDto
(
    [Required] string Title,
    [Required] string SubTitle,
    [Required] string Caption,
    [Required] IFormFile Image,
    [Required] MediaType Type
);
