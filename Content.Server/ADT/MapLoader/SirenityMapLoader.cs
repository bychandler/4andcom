using Content.Server.GameTicking.Events;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.ADT.MapLoader;

public sealed class SirenityMapLoader :EntitySystem
{
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly MapLoaderSystem _map = default!;
    [Dependency] private readonly ShuttleConsoleSystem _console = default!;

    public MapId? MarsMapId { get; private set; }
    public EntityUid? MarsMap { get; private set; }

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundStartingEvent>(OnRoundStart);
    }

    private void OnRoundStart(RoundStartingEvent ev)
    {
        MarsMapId = _mapManager.CreateMap();
        //_mapManager.SetMapPaused(MarsMapId.Value, true);

        var marsMap = _map.LoadGrid(MarsMapId.Value, "/Maps/Mars/ChandlerMars.yml");
        MarsMap = marsMap;

        if (MarsMap != null)
            AddFTLDestination(MarsMap.Value, true);
    }

    private FTLDestinationComponent AddFTLDestination(EntityUid uid, bool enabled)
    {
        if (TryComp<FTLDestinationComponent>(uid, out var destination) && destination.Enabled == enabled) return destination;
        destination = EnsureComp<FTLDestinationComponent>(uid);
        destination.Enabled = enabled;
        _console.RefreshShuttleConsoles();
        return destination;
    }
}
