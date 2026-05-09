using InnovationLab.Landing.Controllers;
using InnovationLab.Landing.DbContexts;
using InnovationLab.Landing.Dtos.Events;
using InnovationLab.Landing.Dtos.EventRegistrations;
using InnovationLab.Landing.Enums;
using InnovationLab.Landing.Models;
using InnovationLab.Landing.Services;
using InnovationLab.Shared.Enums;
using InnovationLab.Shared.Interfaces;
using InnovationLab.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Xunit;

namespace InnovationLab.Landing.Tests.Controllers;

public sealed class EventsControllerTests
{
    [Fact]
    public async Task RegisterForEvent_ReturnsBadRequest_WhenTeamMemberPhotoIsMissing()
    {
        var eventId = Guid.NewGuid();
        var controller = CreateController(
            SeedEvent(eventId, isTeamEvent: true),
            eventRegistrations: new FakeRepository<EventRegistration>()
        );

        var registration = new EventRegistrationCreateDto(
            EventId: eventId,
            Type: EventRegistrationType.Team,
            TeamName: "Team Alpha",
            Name: "Leader Name",
            Email: "leader@example.com",
            Phone: "1234567890",
            Documents: null,
            Members: [new TeamMemberCreateDto(
                Name: "Member One",
                Faculty: "Engineering",
                Email: "member@example.com",
                Phone: "1234567890",
                Photo: null!,
                Gender: Gender.Male
            )],
            RegistrationColleges: null
        );

        var result = await controller.RegisterForEvent(eventId, registration);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.Contains("photo", badRequest.Value?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RegisterForEvent_ReturnsBadRequest_WhenDocumentIsMissing()
    {
        var eventId = Guid.NewGuid();
        var controller = CreateController(
            SeedEvent(eventId, isTeamEvent: false),
            eventRegistrations: new FakeRepository<EventRegistration>()
        );

        var registration = new EventRegistrationCreateDto(
            EventId: eventId,
            Type: EventRegistrationType.Solo,
            TeamName: "Team Alpha",
            Name: "Leader Name",
            Email: "leader@example.com",
            Phone: "1234567890",
            Documents: [null!],
            Members: null,
            RegistrationColleges: null
        );

        var result = await controller.RegisterForEvent(eventId, registration);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.Contains("document", badRequest.Value?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RegisterForEvent_CreatesRegistration_WhenPayloadIsValid()
    {
        var eventId = Guid.NewGuid();
        var repository = new FakeRepository<EventRegistration>();
        var mediaService = new FakeMediaService();
        var notificationService = new FakeNotificationService();
        var controller = CreateController(
            SeedEvent(eventId, isTeamEvent: true),
            eventRegistrations: repository,
            mediaService: mediaService,
            notificationService: notificationService
        );

        var photo = CreateFile("member.png", "image/png");
        mediaService.AddUploadResult(photo, "https://cdn.example.com/member.png");

        var registration = new EventRegistrationCreateDto(
            EventId: eventId,
            Type: EventRegistrationType.Team,
            TeamName: "Team Alpha",
            Name: "Leader Name",
            Email: "leader@example.com",
            Phone: "1234567890",
            Documents: null,
            Members: [new TeamMemberCreateDto(
                Name: "Member One",
                Faculty: "Engineering",
                Email: "member@example.com",
                Phone: "1234567890",
                Photo: photo,
                Gender: Gender.Male
            )],
            RegistrationColleges: null
        );

        var result = await controller.RegisterForEvent(eventId, registration);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<EventRegistrationResponseDto>(created.Value);
        Assert.Equal(eventId, dto.EventId);
        Assert.Equal(EventRegistrationType.Team, dto.Type);
        Assert.Equal(EventRegistrationStatus.Pending, dto.Status);
        Assert.Single(repository.Entities);
        Assert.Single(repository.Entities[0].Members!);
        Assert.Equal("https://cdn.example.com/member.png", repository.Entities[0].Members![0].PhotoUrl);
        Assert.Empty(notificationService.Notifications);
    }

    [Fact]
    public void EventRegistrationResponseDto_Adapt_DoesNotRecurseThroughEventRegistrations()
    {
        var eventId = Guid.NewGuid();
        var registration = new EventRegistration
        {
            EventId = eventId,
            Type = EventRegistrationType.Team,
            TeamName = "Team Alpha",
            Name = "Leader Name",
            Email = "leader@example.com",
            DocumentUrls = [],
            Event = new Event
            {
                Id = eventId,
                Title = "Innovation Summit",
                Description = "Annual innovation summit",
                Highlights = ["Track 1"],
                StartTime = DateTimeOffset.UtcNow.AddDays(1),
                EndTime = DateTimeOffset.UtcNow.AddDays(2),
                Location = "Main Hall",
                CoverImageUrl = "https://cdn.example.com/event.png",
                IsTeamEvent = true,
                MaxTeamMembers = 5,
                MaxNumberOfTeams = 3,
                IsRegistrationOpen = true,
                RegistrationEnd = DateTimeOffset.UtcNow.AddDays(1),
                Registrations = []
            }
        };

        registration.Event.Registrations.Add(registration);

        var dto = registration.Adapt<EventRegistrationResponseDto>();
        var eventDto = Assert.IsType<EventResponseDto>(dto.Event);

        Assert.Equal(eventId, eventDto.Id);
        Assert.True(eventDto.Title == "Innovation Summit");
    }

    private static EventsController CreateController(
        Event @event,
        FakeRepository<EventRegistration> eventRegistrations,
        IMediaService? mediaService = null,
        IEventRegistrationNotificationService? notificationService = null
    )
    {
        return new EventsController(
            new FakeRepository<Event>([@event]),
            new FakeRepository<EventAgenda>(),
            eventRegistrations,
            new FakeRepository<TeamMember>(),
            new FakeRepository<RegistrationCollege>(),
            mediaService ?? new FakeMediaService(),
            notificationService ?? new FakeNotificationService()
        );
    }

    private static Event SeedEvent(Guid eventId, bool isTeamEvent)
    {
        return new Event
        {
            Id = eventId,
            Title = "Innovation Summit",
            Description = "Annual innovation summit",
            Highlights = ["Track 1"],
            StartTime = DateTimeOffset.UtcNow.AddDays(1),
            EndTime = DateTimeOffset.UtcNow.AddDays(2),
            Location = "Main Hall",
            CoverImageUrl = "https://cdn.example.com/event.png",
            IsTeamEvent = isTeamEvent,
            MaxTeamMembers = 5,
            MaxNumberOfTeams = 3,
            IsRegistrationOpen = true,
            RegistrationEnd = DateTimeOffset.UtcNow.AddDays(1)
        };
    }

    private static IFormFile CreateFile(string fileName, string contentType)
    {
        var bytes = new byte[] { 1, 2, 3 };
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private sealed class FakeMediaService : IMediaService
    {
        private readonly Dictionary<IFormFile, string> _uploads = new(ReferenceEqualityComparer.Instance);

        public void AddUploadResult(IFormFile file, string url)
        {
            _uploads[file] = url;
        }

        public Task<string?> UploadAsync(IFormFile file, MediaType mediaType, string? folder = null, CloudinaryDotNet.Transformation? transformation = null)
        {
            return Task.FromResult(_uploads.TryGetValue(file, out var url) ? url : null);
        }
    }

    private sealed class FakeNotificationService : IEventRegistrationNotificationService
    {
        public List<Guid> Notifications { get; } = [];

        public void Subscribe(Guid eventId, System.Threading.Channels.Channel<EventRegistrationNotificationDto> channel)
        {
        }

        public void Unsubscribe(Guid eventId, System.Threading.Channels.Channel<EventRegistrationNotificationDto> channel)
        {
        }

        public Task NotifyRegistrationClosedAsync(Guid eventId)
        {
            Notifications.Add(eventId);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeRepository<TModel>(IEnumerable<TModel>? seed = null) : IRepository<LandingDbContext, TModel>
        where TModel : BaseModel
    {
        public List<TModel> Entities { get; } = seed?.ToList() ?? [];

        public Task<bool> SaveChangesAsync() => Task.FromResult(true);

        public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<TModel, bool>>? predicate = null)
        {
            var query = Entities.AsQueryable();
            return Task.FromResult(predicate is null ? query.Count() : query.Count(predicate));
        }

        public Task<TModel?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Entities.FirstOrDefault(entity => entity.Id == id));
        }

        public Task<IEnumerable<TModel>> GetByIdsAsync(IEnumerable<Guid> ids, int skip, int take)
        {
            return Task.FromResult(Entities.Where(entity => ids.Contains(entity.Id)).Skip(skip).Take(take).AsEnumerable());
        }

        public Task<IEnumerable<TModel>> GetAsync(int skip, int take)
        {
            return Task.FromResult(Entities.AsEnumerable().Skip(skip).Take(take));
        }

        public Task<IEnumerable<TModel>> FindAsync(System.Linq.Expressions.Expression<Func<TModel, bool>> predicate, int skip, int take)
        {
            return Task.FromResult(Entities.AsQueryable().Where(predicate).Skip(skip).Take(take).AsEnumerable());
        }

        public Task<IEnumerable<TModel>> QueryAsync(Func<IQueryable<TModel>, IQueryable<TModel>> query, int skip, int take)
        {
            return Task.FromResult(query(Entities.AsQueryable()).Skip(skip).Take(take).AsEnumerable());
        }

        public Task BatchAddAsync(IEnumerable<TModel> entities)
        {
            Entities.AddRange(entities);
            return Task.CompletedTask;
        }

        public Task AddAsync(TModel entity)
        {
            Entities.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(TModel entity)
        {
            entity.UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void SoftDelete(TModel entity)
        {
            entity.DeletedAt = DateTimeOffset.UtcNow;
        }

        public void HardDelete(TModel entity)
        {
            Entities.Remove(entity);
        }
    }
}
