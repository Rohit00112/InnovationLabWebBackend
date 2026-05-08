using System.Threading.Channels;
using InnovationLab.Landing.Dtos.EventRegistrations;

namespace InnovationLab.Landing.Services;

public interface IEventRegistrationNotificationService
{
    void Subscribe(Guid eventId, Channel<EventRegistrationNotificationDto> channel);
    void Unsubscribe(Guid eventId, Channel<EventRegistrationNotificationDto> channel);
    Task NotifyRegistrationClosedAsync(Guid eventId);
}