using System.Threading.Channels;
using InnovationLab.Landing.DbContexts;
using InnovationLab.Landing.Dtos.EventAgendas;
using InnovationLab.Landing.Dtos.EventRegistrations;
using InnovationLab.Landing.Dtos.Events;
using InnovationLab.Landing.Enums;
using InnovationLab.Landing.Models;
using InnovationLab.Landing.Services;
using InnovationLab.Shared.Enums;
using InnovationLab.Shared.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InnovationLab.Shared.Extensions;

namespace InnovationLab.Landing.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class EventsController(
    IRepository<LandingDbContext, Event> eventRepo,
    IRepository<LandingDbContext, EventAgenda> eventAgendaRepo,
    IRepository<LandingDbContext, EventRegistration> eventRegistrationRepo,
    IRepository<LandingDbContext, TeamMember> teamMemberRepo,
    IRepository<LandingDbContext, RegistrationCollege> registrationCollegeRepo,
    IMediaService mediaService,
    IEventRegistrationNotificationService notificationService
) : ControllerBase
{
    private const string EventRegistrationsDocumentsFolder = "events/registrations";
    private const string EventTeamMembersPhotosFolder = "events/team-members";

    private readonly IRepository<LandingDbContext, Event> _eventRepo = eventRepo;
    private readonly IRepository<LandingDbContext, EventAgenda> _eventAgendaRepo = eventAgendaRepo;
    private readonly IRepository<LandingDbContext, EventRegistration> _eventRegistrationRepo = eventRegistrationRepo;
    private readonly IRepository<LandingDbContext, TeamMember> _teamMemberRepo = teamMemberRepo;
    private readonly IRepository<LandingDbContext, RegistrationCollege> _registrationCollegeRepo = registrationCollegeRepo;
    private readonly IMediaService _mediaService = mediaService;
    private readonly IEventRegistrationNotificationService _notificationService = notificationService;

    [AllowAnonymous]
    [HttpGet(Name = nameof(GetEvents))]
    public async Task<ActionResult<IList<EventResponseDto>>> GetEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var skip = (page - 1) * pageSize;
        var events = await _eventRepo.GetAsync(skip, pageSize);
        var eventsDto = events.Adapt<IList<EventResponseDto>>();
        return Ok(eventsDto);
    }

    [AllowAnonymous]
    [HttpGet("{id}", Name = nameof(GetEventById))]
    public async Task<ActionResult<EventResponseDto>> GetEventById(Guid id)
    {
        var @event = await _eventRepo.GetByIdAsync(id);
        if (@event is null)
        {
            return NotFound();
        }

        var eventDto = @event.Adapt<EventResponseDto>();
        return Ok(eventDto);
    }

    [Authorize]
    [HttpPost(Name = nameof(CreateEvent))]
    public async Task<ActionResult<EventResponseDto>> CreateEvent([FromForm] EventCreateDto eventCreateDto)
    {
        var @event = eventCreateDto.Adapt<Event>();
        await _eventRepo.AddAsync(@event);
        await _eventRepo.SaveChangesAsync();

        var eventDto = @event.Adapt<EventResponseDto>();
        return CreatedAtAction(nameof(GetEventById), new { id = eventDto.Id }, eventDto);
    }

    [Authorize]
    [HttpPut("{id}", Name = nameof(UpdateEvent))]
    public async Task<ActionResult> UpdateEvent(Guid id, [FromForm] EventUpdateDto eventUpdateDto)
    {
        var @event = await _eventRepo.GetByIdAsync(id);
        if (@event is null)
        {
            return NotFound();
        }

        eventUpdateDto.Adapt(@event);
        _eventRepo.Update(@event);
        await _eventRepo.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}", Name = nameof(DeleteEvent))]
    public async Task<ActionResult> DeleteEvent(Guid id)
    {
        var @event = await _eventRepo.GetByIdAsync(id);
        if (@event is null)
        {
            return NotFound();
        }

        _eventRepo.SoftDelete(@event);
        await _eventRepo.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/agenda", Name = nameof(GetEventAgenda))]
    public async Task<ActionResult<IList<EventAgendaResponseDto>>> GetEventAgenda(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        var skip = (page - 1) * pageSize;

        var agendas = await _eventAgendaRepo.QueryAsync(
            ea => ea.Include(a => a.Items).Where(a => a.EventId == id),
            skip,
            pageSize
        );

        var agendasDto = agendas.Adapt<IList<EventAgendaResponseDto>>();

        return Ok(agendasDto);
    }

    [Authorize]
    [HttpPost("{id}/agenda", Name = nameof(CreateEventAgenda))]
    public async Task<ActionResult<EventAgendaResponseDto>> CreateEventAgenda(Guid id, [FromBody] EventAgendaCreateDto agendaCreateDto)
    {
        var @event = await _eventRepo.GetByIdAsync(id);

        if (@event is null)
        {
            return NotFound();
        }

        var newAgenda = agendaCreateDto.Adapt<EventAgenda>();
        newAgenda.EventId = id;

        await _eventAgendaRepo.AddAsync(newAgenda);
        await _eventAgendaRepo.SaveChangesAsync();

        var agendaDto = newAgenda.Adapt<EventAgendaResponseDto>();

        return CreatedAtAction(nameof(GetEventAgenda), new { id = agendaDto.Id }, agendaDto);
    }

    [Authorize]
    [HttpPut("agenda/{agendaId}", Name = nameof(UpdateEventAgenda))]
    public async Task<ActionResult> UpdateEventAgenda(Guid agendaId, [FromBody] EventAgendaUpdateDto agendaUpdateDto)
    {
        var agenda = await _eventAgendaRepo.GetByIdAsync(agendaId);

        if (agenda is null)
        {
            return NotFound();
        }

        agendaUpdateDto.Adapt(agenda);
        _eventAgendaRepo.Update(agenda);
        await _eventAgendaRepo.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("agenda/{agendaId}", Name = nameof(DeleteEventAgenda))]
    public async Task<ActionResult> DeleteEventAgenda(Guid agendaId)
    {
        var agenda = await _eventAgendaRepo.GetByIdAsync(agendaId);

        if (agenda is null)
        {
            return NotFound();
        }

        _eventAgendaRepo.HardDelete(agenda);
        await _eventAgendaRepo.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/register", Name = nameof(RegisterForEvent))]
    public async Task<ActionResult<EventRegistrationResponseDto>> RegisterForEvent(Guid id, [FromForm] EventRegistrationCreateDto registrationCreateDto)
    {
        var @event = await _eventRepo.GetByIdAsync(id);
        if (@event is null)
        {
            return NotFound();
        }

        // Validate registration is open
        if (!@event.IsRegistrationOpen || @event.RegistrationEnd <= DateTimeOffset.Now)
        {
            return StatusCode(StatusCodes.Status410Gone, "Registration for this event is no longer open");
        }

        // Check if registration would exceed max teams
        var currentRegistrationCount = await _eventRegistrationRepo.QueryAsync(
            er => er.Where(r => r.EventId == id && r.DeletedAt == null),
            0,
            1
        );

        if (currentRegistrationCount.Count() >= @event.MaxNumberOfTeams)
        {
            return StatusCode(StatusCodes.Status410Gone, "Maximum registrations reached for this event");
        }

        // Validate team event requirements
        if (@event.IsTeamEvent)
        {
            if (registrationCreateDto.Members is null || registrationCreateDto.Members.Count is 0)
            {
                return BadRequest("Team Members are required for team event");
            }

            if (registrationCreateDto.Members.Count > @event.MaxTeamMembers)
            {
                return BadRequest($"Maximum of {@event.MaxTeamMembers} team members are only allowed");
            }
        }
        else
        {
            if (registrationCreateDto.Members is not null && registrationCreateDto.Members.Count is not 0)
            {
                return BadRequest("Team Members are not allowed for solo event");
            }
        }

        // Upload documents if provided
        var documentUrls = new List<string?>();
        if (registrationCreateDto.Documents is not null && registrationCreateDto.Documents.Count > 0)
        {
            foreach (var document in registrationCreateDto.Documents)
            {
                var mediaType = document.ContentType.ToMediaType();
                if (mediaType is not (MediaType.Pdf or MediaType.Image))
                {
                    return StatusCode(StatusCodes.Status415UnsupportedMediaType, "Only PDF and Image documents are allowed");
                }

                var documentUrl = await _mediaService.UploadAsync(document, mediaType, EventRegistrationsDocumentsFolder);
                if (string.IsNullOrWhiteSpace(documentUrl))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to upload document");
                }

                documentUrls.Add(documentUrl);
            }
        }

        var newRegistration = registrationCreateDto.Adapt<EventRegistration>();
        newRegistration.EventId = id;
        newRegistration.Type = @event.IsTeamEvent ? EventRegistrationType.Team : EventRegistrationType.Solo;
        newRegistration.Status = EventRegistrationStatus.Pending;
        newRegistration.DocumentUrls = documentUrls;

        // Handle team members with photos
        if (registrationCreateDto.Members is not null && registrationCreateDto.Members.Count > 0)
        {
            var teamMembers = new List<TeamMember>();
            foreach (var memberDto in registrationCreateDto.Members)
            {
                var photoUrl = await _mediaService.UploadAsync(memberDto.Photo, MediaType.Image, EventTeamMembersPhotosFolder);
                if (string.IsNullOrWhiteSpace(photoUrl))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to upload team member photo");
                }

                var teamMember = memberDto.Adapt<TeamMember>();
                teamMember.PhotoUrl = photoUrl;
                teamMembers.Add(teamMember);
            }

            newRegistration.Members = teamMembers;
        }

        // Handle registration colleges if provided
        if (registrationCreateDto.RegistrationColleges is not null && registrationCreateDto.RegistrationColleges.Count > 0)
        {
            newRegistration.RegistrationColleges = registrationCreateDto.RegistrationColleges.Adapt<List<RegistrationCollege>>();
        }

        await _eventRegistrationRepo.AddAsync(newRegistration);
        await _eventRegistrationRepo.SaveChangesAsync();

        // Reload event to check if registration was auto-closed by trigger
        var updatedEvent = await _eventRepo.GetByIdAsync(id);
        if (updatedEvent != null && !updatedEvent.IsRegistrationOpen)
        {
            // Notify all subscribers that registration has been closed
            await _notificationService.NotifyRegistrationClosedAsync(id);
        }

        var registrationDto = newRegistration.Adapt<EventRegistrationResponseDto>();
        return CreatedAtAction(nameof(GetEventRegistration), new { registrationId = registrationDto.Id }, registrationDto);
    }

    [AllowAnonymous]
    [HttpGet("{id}/registrations-closed-stream", Name = nameof(GetRegistrationClosedStream))]
    public async Task GetRegistrationClosedStream(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var @event = await _eventRepo.GetByIdAsync(id);
        if (@event is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        Response.ContentType = "text/event-stream";
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        var channel = Channel.CreateUnbounded<EventRegistrationNotificationDto>();
        _notificationService.Subscribe(id, channel);

        try
        {
            await foreach (var notification in channel.Reader.ReadAllAsync(cancellationToken))
            {
                var json = System.Text.Json.JsonSerializer.Serialize(notification);
                await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        finally
        {
            _notificationService.Unsubscribe(id, channel);
        }
    }

    [AllowAnonymous]
    [HttpGet("{id}/registrations", Name = nameof(GetEventRegistrations))]
    public async Task<ActionResult<IList<EventRegistrationResponseDto>>> GetEventRegistrations(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        var skip = (page - 1) * pageSize;

        var registrations = await _eventRegistrationRepo.QueryAsync(
            er => er.Include(r => r.Members).Include(r => r.RegistrationColleges).Where(r => r.EventId == id),
            skip,
            pageSize
        );

        var registrationsDto = registrations.Adapt<IList<EventRegistrationResponseDto>>();
        return Ok(registrationsDto);
    }

    [AllowAnonymous]
    [HttpGet("registrations/{registrationId}", Name = nameof(GetEventRegistration))]
    public async Task<ActionResult<EventRegistrationResponseDto>> GetEventRegistration(Guid registrationId)
    {
        var registrations = await _eventRegistrationRepo.QueryAsync(
            er => er.Include(r => r.Members).Include(r => r.RegistrationColleges).Where(r => r.Id == registrationId),
            0,
            1
        );

        var registration = registrations.FirstOrDefault();
        if (registration is null)
        {
            return NotFound();
        }

        var registrationDto = registration.Adapt<EventRegistrationResponseDto>();
        return Ok(registrationDto);
    }

    [Authorize]
    [HttpPut("registrations/{registrationId}", Name = nameof(UpdateEventRegistration))]
    public async Task<ActionResult> UpdateEventRegistration(Guid registrationId, [FromForm] EventRegistrationUpdateDto registrationUpdateDto)
    {
        var registration = await _eventRegistrationRepo.GetByIdAsync(registrationId);
        if (registration is null)
        {
            return NotFound();
        }

        // Upload new documents if provided
        if (registrationUpdateDto.Documents is not null && registrationUpdateDto.Documents.Count > 0)
        {
            foreach (var document in registrationUpdateDto.Documents)
            {
                var mediaType = document.ContentType.ToMediaType();
                if (mediaType is not (MediaType.Pdf or MediaType.Image))
                {
                    return StatusCode(StatusCodes.Status415UnsupportedMediaType, "Only PDF and Image documents are allowed");
                }

                var documentUrl = await _mediaService.UploadAsync(document, mediaType, EventRegistrationsDocumentsFolder);
                if (string.IsNullOrWhiteSpace(documentUrl))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to upload document");
                }

                registration.DocumentUrls.Add(documentUrl);
            }
        }

        // Update basic fields
        if (!string.IsNullOrWhiteSpace(registrationUpdateDto.TeamName))
            registration.TeamName = registrationUpdateDto.TeamName;

        if (!string.IsNullOrWhiteSpace(registrationUpdateDto.Name))
            registration.Name = registrationUpdateDto.Name;

        if (!string.IsNullOrWhiteSpace(registrationUpdateDto.Email))
            registration.Email = registrationUpdateDto.Email;

        if (!string.IsNullOrWhiteSpace(registrationUpdateDto.Phone))
            registration.Phone = registrationUpdateDto.Phone;

        if (registrationUpdateDto.Status.HasValue)
            registration.Status = registrationUpdateDto.Status.Value;

        _eventRegistrationRepo.Update(registration);
        await _eventRegistrationRepo.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("registrations/{registrationId}", Name = nameof(DeleteEventRegistration))]
    public async Task<ActionResult> DeleteEventRegistration(Guid registrationId)
    {
        var registration = await _eventRegistrationRepo.GetByIdAsync(registrationId);
        if (registration is null)
        {
            return NotFound();
        }

        _eventRegistrationRepo.SoftDelete(registration);
        await _eventRegistrationRepo.SaveChangesAsync();

        return NoContent();
    }

    // Team Members Endpoints
    [AllowAnonymous]
    [HttpGet("registrations/{registrationId}/members", Name = nameof(GetTeamMembers))]
    public async Task<ActionResult<IList<TeamMemberResponseDto>>> GetTeamMembers(
        Guid registrationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        var skip = (page - 1) * pageSize;

        var members = await _teamMemberRepo.QueryAsync(
            m => m.Where(mem => mem.RegistrationId == registrationId),
            skip,
            pageSize
        );

        var membersDto = members.Adapt<IList<TeamMemberResponseDto>>();
        return Ok(membersDto);
    }

    [Authorize]
    [HttpPost("registrations/{registrationId}/members", Name = nameof(AddTeamMember))]
    public async Task<ActionResult<TeamMemberResponseDto>> AddTeamMember(Guid registrationId, [FromForm] TeamMemberCreateDto memberCreateDto)
    {
        var registration = await _eventRegistrationRepo.GetByIdAsync(registrationId);
        if (registration is null)
        {
            return NotFound();
        }

        var mediaType = memberCreateDto.Photo.ContentType.ToMediaType();
        if (mediaType is not MediaType.Image)
        {
            return StatusCode(StatusCodes.Status415UnsupportedMediaType, "Only image files are allowed for photos");
        }

        var photoUrl = await _mediaService.UploadAsync(memberCreateDto.Photo, mediaType, EventTeamMembersPhotosFolder);
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to upload photo");
        }

        var teamMember = memberCreateDto.Adapt<TeamMember>();
        teamMember.RegistrationId = registrationId;
        teamMember.PhotoUrl = photoUrl;

        await _teamMemberRepo.AddAsync(teamMember);
        await _teamMemberRepo.SaveChangesAsync();

        var memberDto = teamMember.Adapt<TeamMemberResponseDto>();
        return CreatedAtAction(nameof(GetTeamMember), new { memberId = memberDto.Id }, memberDto);
    }

    [AllowAnonymous]
    [HttpGet("members/{memberId}", Name = nameof(GetTeamMember))]
    public async Task<ActionResult<TeamMemberResponseDto>> GetTeamMember(Guid memberId)
    {
        var member = await _teamMemberRepo.GetByIdAsync(memberId);
        if (member is null)
        {
            return NotFound();
        }

        var memberDto = member.Adapt<TeamMemberResponseDto>();
        return Ok(memberDto);
    }

    [Authorize]
    [HttpPut("members/{memberId}", Name = nameof(UpdateTeamMember))]
    public async Task<ActionResult> UpdateTeamMember(Guid memberId, [FromForm] TeamMemberUpdateDto memberUpdateDto)
    {
        var member = await _teamMemberRepo.GetByIdAsync(memberId);
        if (member is null)
        {
            return NotFound();
        }

        // Handle photo update if provided
        if (memberUpdateDto.Photo is not null && memberUpdateDto.Photo.Length > 0)
        {
            var mediaType = memberUpdateDto.Photo.ContentType.ToMediaType();
            if (mediaType is not MediaType.Image)
            {
                return StatusCode(StatusCodes.Status415UnsupportedMediaType, "Only image files are allowed");
            }

            var photoUrl = await _mediaService.UploadAsync(memberUpdateDto.Photo, mediaType, EventTeamMembersPhotosFolder);
            if (string.IsNullOrWhiteSpace(photoUrl))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to upload photo");
            }

            member.PhotoUrl = photoUrl;
        }

        // Update other fields
        if (!string.IsNullOrWhiteSpace(memberUpdateDto.Name))
            member.Name = memberUpdateDto.Name;

        if (!string.IsNullOrWhiteSpace(memberUpdateDto.Faculty))
            member.Faculty = memberUpdateDto.Faculty;

        if (!string.IsNullOrWhiteSpace(memberUpdateDto.Email))
            member.Email = memberUpdateDto.Email;

        if (!string.IsNullOrWhiteSpace(memberUpdateDto.Phone))
            member.Phone = memberUpdateDto.Phone;

        if (memberUpdateDto.Gender.HasValue)
            member.Gender = memberUpdateDto.Gender.Value;

        _teamMemberRepo.Update(member);
        await _teamMemberRepo.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("members/{memberId}", Name = nameof(DeleteTeamMember))]
    public async Task<ActionResult> DeleteTeamMember(Guid memberId)
    {
        var member = await _teamMemberRepo.GetByIdAsync(memberId);
        if (member is null)
        {
            return NotFound();
        }

        _teamMemberRepo.SoftDelete(member);
        await _teamMemberRepo.SaveChangesAsync();

        return NoContent();
    }

    // Registration Colleges Endpoints
    [AllowAnonymous]
    [HttpGet("registrations/{registrationId}/colleges", Name = nameof(GetRegistrationColleges))]
    public async Task<ActionResult<IList<RegistrationCollegeResponseDto>>> GetRegistrationColleges(
        Guid registrationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        var skip = (page - 1) * pageSize;

        var colleges = await _registrationCollegeRepo.QueryAsync(
            c => c.Where(col => col.RegistrationId == registrationId),
            skip,
            pageSize
        );

        var collegesDto = colleges.Adapt<IList<RegistrationCollegeResponseDto>>();
        return Ok(collegesDto);
    }

    [Authorize]
    [HttpPost("registrations/{registrationId}/colleges", Name = nameof(AddRegistrationCollege))]
    public async Task<ActionResult<RegistrationCollegeResponseDto>> AddRegistrationCollege(Guid registrationId, [FromBody] RegistrationCollegeCreateDto collegeCreateDto)
    {
        var registration = await _eventRegistrationRepo.GetByIdAsync(registrationId);
        if (registration is null)
        {
            return NotFound();
        }

        var college = collegeCreateDto.Adapt<RegistrationCollege>();
        college.RegistrationId = registrationId;

        await _registrationCollegeRepo.AddAsync(college);
        await _registrationCollegeRepo.SaveChangesAsync();

        var collegeDto = college.Adapt<RegistrationCollegeResponseDto>();
        return CreatedAtAction(nameof(GetRegistrationCollege), new { collegeId = collegeDto.Id }, collegeDto);
    }

    [AllowAnonymous]
    [HttpGet("colleges/{collegeId}", Name = nameof(GetRegistrationCollege))]
    public async Task<ActionResult<RegistrationCollegeResponseDto>> GetRegistrationCollege(Guid collegeId)
    {
        var college = await _registrationCollegeRepo.GetByIdAsync(collegeId);
        if (college is null)
        {
            return NotFound();
        }

        var collegeDto = college.Adapt<RegistrationCollegeResponseDto>();
        return Ok(collegeDto);
    }

    [Authorize]
    [HttpPut("colleges/{collegeId}", Name = nameof(UpdateRegistrationCollege))]
    public async Task<ActionResult> UpdateRegistrationCollege(Guid collegeId, [FromBody] RegistrationCollegeUpdateDto collegeUpdateDto)
    {
        var college = await _registrationCollegeRepo.GetByIdAsync(collegeId);
        if (college is null)
        {
            return NotFound();
        }

        collegeUpdateDto.Adapt(college);

        _registrationCollegeRepo.Update(college);
        await _registrationCollegeRepo.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("colleges/{collegeId}", Name = nameof(DeleteRegistrationCollege))]
    public async Task<ActionResult> DeleteRegistrationCollege(Guid collegeId)
    {
        var college = await _registrationCollegeRepo.GetByIdAsync(collegeId);
        if (college is null)
        {
            return NotFound();
        }

        _registrationCollegeRepo.SoftDelete(college);
        await _registrationCollegeRepo.SaveChangesAsync();

        return NoContent();
    }
}