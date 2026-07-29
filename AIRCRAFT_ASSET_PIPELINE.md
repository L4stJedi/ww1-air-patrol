# WONML Aircraft Asset Pipeline

## Source of truth

Game-ready aircraft PNG files are classified by nation in:

`/Users/karelkadlec/Desktop/My Team/Team Inbox/WONML/assets/planes/<nation>/`

The nation folder is authoritative. For example, every `AircoDH*.png` currently
under `britain/` is British and must never appear in the German service tree.

The Team Inbox folder is source material and is not modified by the Unity
project. Files used by the game are copied into:

`Assets/Textures/Planes/<Nation>/`

Runtime code must only reference project assets. It must not contain an
absolute path to the Team Inbox, so the project remains usable on other Macs
and on mobile build machines.

## Naming

- `_std` or an unsuffixed aircraft name: standard service aircraft.
- Other suffixes: cosmetic, ace, supporter, or event liveries.
- Cosmetic variants belong to the same nation as their base aircraft.
- Adding a livery must not add a stronger aircraft to the progression tree.

## Currently connected prototype aircraft

### Germany

- Albatros D.V

### Britain

- Airco D.H.1
- Airco D.H.2
- Airco D.H.9

The Airco D.H.2 is the current British starting aircraft. The Albatros D.V is
the current German starting aircraft.

## Integration checklist

When a new standard aircraft is promoted from Team Inbox into Unity:

1. Copy its PNG to `Assets/Textures/Planes/<Nation>/`.
2. Add a stable enum value without changing existing numeric values.
3. Register its nation and display name in `ListOfPlanes`.
4. Add its playable prefab or runtime definition to `PlayerSpawner`.
5. Add its Hangar card beneath the matching nation container.
6. Test equipping it and starting a mission.

