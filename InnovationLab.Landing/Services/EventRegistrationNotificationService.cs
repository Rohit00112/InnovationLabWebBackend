using System.Collections.Concurrent;
using System.Threading.Channels;
using InnovationLab.Landing.Constants;
using InnovationLab.Landing.Dtos.EventRegistrations;

namespace InnovationLab.Landing.Services;

public class EventRegistrationNotificationService : IEventRegistrationNotificationService
{
    private readonly ConcurrentDictionary<Guid, List<Channel<EventRegistrationNotificationDto>>> _subscribers = new();

    public void Subscribe(Guid eventId, Channel<EventRegistrationNotificationDto> channel)
    {
        _subscribers.AddOrUpdate(eventId,
            [channel],
            (key, list) => { list.Add(channel); return list; });
    }

    public void Unsubscribe(Guid eventId, Channel<EventRegistrationNotificationDto> channel)
    {
        if (_subscribers.TryGetValue(eventId, out var channels))
        {
            channels.Remove(channel);
        }
    }

    public async Task NotifyRegistrationClosedAsync(Guid eventId)
    {
        if (_subscribers.TryGetValue(eventId, out var channels))
        {
            var notification = new EventRegistrationNotificationDto
            {
                Event = EventRegistrationNotificationEvents.RegistrationClosed
            };

            foreach (var channel in channels.ToList())
            {
                try
                {
                    await channel.Writer.WriteAsync(notification);
                }
                catch
                {
                    Unsubscribe(eventId, channel);
                }
            }
        }
    }
}
