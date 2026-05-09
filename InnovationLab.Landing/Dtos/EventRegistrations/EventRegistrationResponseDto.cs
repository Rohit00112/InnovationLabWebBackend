using InnovationLab.Landing.Dtos.Events;
using InnovationLab.Landing.Models;
using InnovationLab.Landing.Enums;

namespace InnovationLab.Landing.Dtos.EventRegistrations;

public record EventRegistrationResponseDto
(
    Guid Id,
    Guid EventId,
    EventResponseDto? Event,
    EventRegistrationType Type,
    string TeamName,
    string Name,
    string Email,
    string? Phone,
    IList<string> DocumentUrls,
    IList<TeamMemberResponseDto>? Members,
    IList<RegistrationCollegeResponseDto>? RegistrationColleges,
    EventRegistrationStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
)
{
    public static EventRegistrationResponseDto FromModel(EventRegistration registration)
    {
        return new EventRegistrationResponseDto(
            registration.Id,
            registration.EventId,
            registration.Event is null ? null : new EventResponseDto(
                registration.Event.Id,
                registration.Event.ParentEventId,
                registration.Event.Title,
                registration.Event.Description,
                registration.Event.Highlights,
                registration.Event.StartTime,
                registration.Event.EndTime,
                registration.Event.Location,
                registration.Event.CoverImageUrl,
                registration.Event.SeriesName,
                registration.Event.IsTeamEvent,
                registration.Event.MaxTeamMembers,
                registration.Event.MaxNumberOfTeams,
                registration.Event.IsRegistrationOpen,
                registration.Event.RegistrationStart,
                registration.Event.RegistrationEnd,
                registration.Event.CreatedAt,
                registration.Event.UpdatedAt
            ),
            registration.Type,
            registration.TeamName,
            registration.Name,
            registration.Email,
            registration.Phone,
            registration.DocumentUrls.Where(url => !string.IsNullOrWhiteSpace(url)).Select(url => url!).ToList(),
            registration.Members?.Select(MapTeamMember).ToList(),
            registration.RegistrationColleges?.Select(MapRegistrationCollege).ToList(),
            registration.Status,
            registration.CreatedAt,
            registration.UpdatedAt
        );
    }

    public static IList<EventRegistrationResponseDto> FromModels(IEnumerable<EventRegistration> registrations)
    {
        return registrations.Select(FromModel).ToList();
    }

    private static TeamMemberResponseDto MapTeamMember(TeamMember member)
    {
        return new TeamMemberResponseDto(
            member.Id,
            member.Name,
            member.Faculty,
            member.Email,
            member.Phone,
            member.PhotoUrl,
            member.Gender,
            member.CreatedAt,
            member.UpdatedAt,
            member.RegistrationId
        );
    }

    private static RegistrationCollegeResponseDto MapRegistrationCollege(RegistrationCollege college)
    {
        return new RegistrationCollegeResponseDto(
            college.Id,
            college.Name,
            college.ContactEmail,
            college.Address,
            college.RepresentativeName,
            college.RepresentativePhone,
            college.RepresentativeEmail,
            college.RepresentativeDesignation,
            college.CreatedAt,
            college.UpdatedAt,
            college.RegistrationId
        );
    }
}