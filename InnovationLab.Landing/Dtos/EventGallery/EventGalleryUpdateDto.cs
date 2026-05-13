using System.ComponentModel.DataAnnotations;
using InnovationLab.Shared.Enums;
using Mapster;

namespace InnovationLab.Landing.Dtos.EventGallery;

[AdaptTo(typeof(Models.EventGallery))]
public record EventGalleryUpdateDto
(
    [Required] string Title,
    [Required] string SubTitle,
    [Required] string Caption,
    IFormFile? Image,
    [Required] MediaType Type
);
