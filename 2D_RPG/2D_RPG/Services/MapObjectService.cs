using _2D_RPG.Models;

namespace _2D_RPG.Services;

public sealed class MapObjectService
{
    public MapObjectDefinition? GetObjectAt(MapDefinition map, int x, int y) => map.Objects.LastOrDefault(obj => obj.X == x && obj.Y == y);

    public MapEventDefinition? GetEventAt(MapDefinition map, int x, int y) => map.Events.LastOrDefault(evt => evt.X == x && evt.Y == y);

    public MapObjectDefinition? PlaceObject(MapDefinition map, MapObjectKind kind, int x, int y)
    {
        if (!TileMapService.IsInBounds(map, x, y)) return null;
        var obj = new MapObjectDefinition
        {
            Id = NextId(map.Objects.Select(candidate => candidate.Id), kind.ToString().ToLowerInvariant()),
            Name = NewObjectName(kind, map.Objects.Count(candidate => candidate.Kind == kind) + 1),
            Kind = kind,
            X = x,
            Y = y,
            IsBlocking = kind is MapObjectKind.Prop or MapObjectKind.Door or MapObjectKind.Chest or MapObjectKind.Npc,
            SpriteKey = kind.ToString().ToLowerInvariant()
        };
        map.Objects.Add(obj);
        return obj;
    }

    public MapEventDefinition? PlaceEvent(MapDefinition map, EventTriggerKind trigger, int x, int y)
    {
        if (!TileMapService.IsInBounds(map, x, y)) return null;
        var evt = new MapEventDefinition
        {
            Id = NextId(map.Events.Select(candidate => candidate.Id), "event"),
            Name = NewEventName(trigger, map.Events.Count(candidate => candidate.Trigger == trigger) + 1),
            Trigger = trigger,
            X = trigger == EventTriggerKind.EnterMap ? null : x,
            Y = trigger == EventTriggerKind.EnterMap ? null : y,
            Commands = [CreateDefaultCommand(trigger)]
        };
        map.Events.Add(evt);
        return evt;
    }

    public bool MoveObject(MapDefinition map, string objectId, int x, int y)
    {
        var obj = map.Objects.FirstOrDefault(candidate => candidate.Id == objectId);
        if (obj is null || !TileMapService.IsInBounds(map, x, y)) return false;
        obj.X = x;
        obj.Y = y;
        return true;
    }

    public bool MoveEvent(MapDefinition map, string eventId, int x, int y)
    {
        var evt = map.Events.FirstOrDefault(candidate => candidate.Id == eventId);
        if (evt is null || evt.Trigger == EventTriggerKind.EnterMap || !TileMapService.IsInBounds(map, x, y)) return false;
        evt.X = x;
        evt.Y = y;
        return true;
    }

    public bool DeleteObject(MapDefinition map, string objectId)
    {
        var obj = map.Objects.FirstOrDefault(candidate => candidate.Id == objectId);
        return obj is not null && map.Objects.Remove(obj);
    }

    public bool DeleteEvent(MapDefinition map, string eventId)
    {
        var evt = map.Events.FirstOrDefault(candidate => candidate.Id == eventId);
        return evt is not null && map.Events.Remove(evt);
    }

    public static EventCommandDefinition CreateDefaultCommand(EventTriggerKind trigger) => trigger switch
    {
        EventTriggerKind.EnterMap => new() { Kind = EventCommandKind.Dialogue, Text = "The party enters the map." },
        EventTriggerKind.Touch => new() { Kind = EventCommandKind.Dialogue, Text = "A hidden trigger activates." },
        _ => new() { Kind = EventCommandKind.Dialogue, Text = "Hello, traveler!" }
    };

    private static string NewObjectName(MapObjectKind kind, int number) => kind switch
    {
        MapObjectKind.Npc => $"NPC {number}",
        _ => $"{kind} {number}"
    };

    private static string NewEventName(EventTriggerKind trigger, int number) => $"{trigger} Event {number}";

    private static string NextId(IEnumerable<string> existingIds, string prefix)
    {
        var existing = existingIds.ToHashSet(StringComparer.OrdinalIgnoreCase);
        for (var index = 1; ; index++)
        {
            var id = $"{prefix}-{index}";
            if (!existing.Contains(id)) return id;
        }
    }
}
