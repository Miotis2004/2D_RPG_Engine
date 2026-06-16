using System.Text.Json;
using _2D_RPG.Models;

namespace _2D_RPG.Services;

public sealed class RuntimeEngineService
{
    private static readonly JsonSerializerOptions CloneOptions = new(JsonSerializerDefaults.Web);

    private List<MapDefinition> runtimeMaps = [];

    public RuntimeSessionState State { get; private set; } = new();

    public RuntimeSessionState StartSession(ProjectDefinition project, string mapId, int playerX = 1, int playerY = 1)
    {
        runtimeMaps = project.Maps.Select(CloneMap).ToList();
        var sourceMap = runtimeMaps.FirstOrDefault(map => map.Id == mapId) ?? runtimeMaps.FirstOrDefault(map => map.Id == project.ActiveMapId) ?? runtimeMaps.FirstOrDefault();
        if (sourceMap is null)
        {
            State = new RuntimeSessionState { LastInteraction = "No map is available to playtest." };
            return State;
        }

        State = new RuntimeSessionState
        {
            Map = sourceMap,
            Player = new RuntimeActorState { X = playerX, Y = playerY },
            LastInteraction = $"Loaded {sourceMap.Name}."
        };

        ClampPlayerToMap();
        TriggerEvents(EventTriggerKind.EnterMap, State.Player.X, State.Player.Y);
        return State;
    }

    public bool MovePlayer(int deltaX, int deltaY)
    {
        if (State.Map is null || (deltaX == 0 && deltaY == 0))
        {
            return false;
        }

        State.Player.Facing = DirectionFromDelta(deltaX, deltaY, State.Player.Facing);
        var targetX = State.Player.X + Math.Clamp(deltaX, -1, 1);
        var targetY = State.Player.Y + Math.Clamp(deltaY, -1, 1);

        if (TileMapService.IsBlocked(State.Map, targetX, targetY))
        {
            State.LastInteraction = $"Blocked at {targetX}, {targetY}.";
            return false;
        }

        State.Player.X = targetX;
        State.Player.Y = targetY;
        State.LastInteraction = $"Moved to {targetX}, {targetY}.";
        TriggerEvents(EventTriggerKind.Touch, targetX, targetY);
        return true;
    }

    public bool Interact()
    {
        if (State.Map is null)
        {
            return false;
        }

        var (x, y) = FacingTile();
        var objectEvent = State.Map.Objects.FirstOrDefault(obj => obj.X == x && obj.Y == y)?.EventId;
        var triggered = TriggerEvents(EventTriggerKind.Interact, x, y, objectEvent);
        State.LastInteraction = triggered ? State.LastInteraction : $"Nothing to interact with at {x}, {y}.";
        return triggered;
    }

    public bool EvaluateCondition(string conditionExpression)
    {
        if (string.IsNullOrWhiteSpace(conditionExpression))
        {
            return true;
        }

        var parts = conditionExpression.Split("==", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2 || !parts[0].StartsWith("flag.", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var flagName = parts[0]["flag.".Length..];
        var expected = bool.TryParse(parts[1], out var parsed) && parsed;
        return State.Flags.TryGetValue(flagName, out var actual) ? actual == expected : expected is false;
    }

    private bool TriggerEvents(EventTriggerKind trigger, int x, int y, string? requiredEventId = null)
    {
        if (State.Map is null)
        {
            return false;
        }

        var events = State.Map.Events.Where(evt => evt.Trigger == trigger && EvaluateCondition(evt.ConditionExpression));
        if (trigger != EventTriggerKind.EnterMap)
        {
            events = events.Where(evt => evt.X == x && evt.Y == y);
        }

        if (!string.IsNullOrWhiteSpace(requiredEventId))
        {
            events = events.Where(evt => evt.Id == requiredEventId);
        }

        var triggered = false;
        foreach (var evt in events)
        {
            triggered = true;
            ExecuteCommands(evt.Commands);
        }

        return triggered;
    }

    private void ExecuteCommands(IEnumerable<EventCommandDefinition> commands)
    {
        foreach (var command in commands)
        {
            switch (command.Kind)
            {
                case EventCommandKind.Dialogue:
                    State.Messages.Add(new RuntimeMessage { Text = command.Text, SourceCommand = command.Kind });
                    State.LastInteraction = command.Text;
                    break;
                case EventCommandKind.ItemReward:
                    State.Inventory.Add($"{command.ItemId} x{Math.Max(1, command.Quantity)}");
                    State.LastInteraction = $"Received {command.ItemId} x{Math.Max(1, command.Quantity)}.";
                    break;
                case EventCommandKind.TransferMap:
                    var targetMap = runtimeMaps.FirstOrDefault(map => map.Id == command.TargetMapId);
                    if (targetMap is not null)
                    {
                        State.Map = targetMap;
                        State.Player.X = command.TargetX;
                        State.Player.Y = command.TargetY;
                        ClampPlayerToMap();
                        State.LastInteraction = $"Transferred to {State.Map.Name} at {State.Player.X}, {State.Player.Y}.";
                        TriggerEvents(EventTriggerKind.EnterMap, State.Player.X, State.Player.Y);
                    }
                    break;
            }
        }
    }

    private (int X, int Y) FacingTile() => State.Player.Facing switch
    {
        RuntimeDirection.Up => (State.Player.X, State.Player.Y - 1),
        RuntimeDirection.Down => (State.Player.X, State.Player.Y + 1),
        RuntimeDirection.Left => (State.Player.X - 1, State.Player.Y),
        RuntimeDirection.Right => (State.Player.X + 1, State.Player.Y),
        _ => (State.Player.X, State.Player.Y)
    };

    private void ClampPlayerToMap()
    {
        if (State.Map is null) return;
        State.Player.X = Math.Clamp(State.Player.X, 0, Math.Max(0, State.Map.Width - 1));
        State.Player.Y = Math.Clamp(State.Player.Y, 0, Math.Max(0, State.Map.Height - 1));
    }

    private static RuntimeDirection DirectionFromDelta(int deltaX, int deltaY, RuntimeDirection fallback) => (deltaX, deltaY) switch
    {
        (0, < 0) => RuntimeDirection.Up,
        (0, > 0) => RuntimeDirection.Down,
        (< 0, 0) => RuntimeDirection.Left,
        (> 0, 0) => RuntimeDirection.Right,
        _ => fallback
    };

    private static MapDefinition CloneMap(MapDefinition map) => JsonSerializer.Deserialize<MapDefinition>(JsonSerializer.Serialize(map, CloneOptions), CloneOptions) ?? new MapDefinition();
}
