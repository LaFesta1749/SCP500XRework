# SCP-500 Pills Plugin for SCP: Secret Laboratory

## Overview
The **SCP-500 Pills** plugin adds **20 unique SCP-500 items** to the game, each with different effects. The plugin is built using the **Exiled API** and **CustomItems API**.

## Features
- **20 different SCP-500 variants**, each with a unique effect.
- Customizable through `config.yml`.
- Integrated with Exiled's **Custom Items API**.
- Balanced gameplay mechanics for SCP-500 usage.

## Installation
1. Download the latest **SCP-500 Pills** release from [GitHub Releases](#).
2. Place the `SCP500Pills.dll` into your server's `Exiled/Plugins/` folder.
3. Restart your SCP:SL server.
4. Configure the plugin in `Exiled/Configs/config.yml`.

## Configuration (`config.yml`)
```yaml
SCP500Pills:
# Дали плъгинът да бъде активиран.
  is_enabled: true
  # Дали debug да бъде активиран.
  debug: false
  # Максимален брой SCP-500 хапчета, които могат да spawn-нат в един рунд.
  max_pills_per_round: 13
  # Процентен шанс за spawn на всяко хапче.
  pills_spawn_chance:
    SCP-500-A: 35
    SCP-500-B: 40
    SCP-500-C: 30
    SCP-500-D: 45
    SCP-500-H: 50
    SCP-500-I: 35
    SCP-500-M: 30
    SCP-500-S: 60
    SCP-500-T: 25
    SCP-500-V: 40
    SCP-500-X: 20
    SCP-500-E: 50
    SCP-500-F: 30
    SCP-500-L: 50
    SCP-500-O: 5
    SCP-500-U: 30
    SCP-500-W: 25
    SCP-500-Y: 40
    SCP-500-Z: 15
    SCP-500-P: 45
  # Дали хапчетата да имат glow ефект при spawn.
  enable_glow_effect: true
  # Колко ярка да бъде светлината около хапчетата.
  glow_intensity: 5
  # Обхватът на светлината около хапчетата.
  glow_range: 1.5
  # Съобщение, което се показва при взимане на хапче.
  picked_up_hint: |-
    You have picked up a {0}
    {1}
  # Съобщение, което се показва при избиране на хапче в инвентара.
  selected_hint: |-
    You have selected a {0}
    {1}
```

## SCP-500 Variants & Effects
| SCP-500 Type | Effect |
|-------------|--------|
| SCP-500-A   | Summons a dead player to help you. |
| SCP-500-B   | Swaps your team to the opposite faction. |
| SCP-500-C   | Causes chaos by confusing all nearby enemies! |
| SCP-500-D   | Temporarily disguises you as the enemy! |
| SCP-500-E   | Gives Class-D and Scientists an item. |
| SCP-500-F   | Simulates death, then revives you. |
| SCP-500-H   | Increases your maximum health. |
| SCP-500-I   | Grants temporary invisibility for 8 seconds, removed if interacting with a door. |
| SCP-500-L   | Grants rapid healing and speed boost for 30 seconds. |
| SCP-500-M   | Distorts your appearance in strange ways! |
| SCP-500-O   | Applies all SCP-500 effects at once, but weakened. |
| SCP-500-P   | Boosts your damage output by 25% for 20 seconds. |
| SCP-500-S   | Gives a random speed boost or slowdown. |
| SCP-500-T   | Teleports you to a random location within the facility! |
| SCP-500-U   | After 5 seconds, grants a random SCP-500 effect for 15 seconds. |
| SCP-500-V   | Steals HP from nearby enemies and converts it into AHP. |
| SCP-500-W   | Causes nearby doors to open and close randomly! |
| SCP-500-X   | Explodes all nearby doors. |
| SCP-500-Y   | An unpredictable pill that oscillates between good and bad effects. |
| SCP-500-Z   | You die and return as an enhanced SCP-049-2. |

## Commands
- `.gp <Player> <SCP-500-X>` - Gives an SCP-500 pill to a player.
- `.pills` - Lists all SCP-500 pills and their effects.
- `.sap` - Forces all SCP-500 pills to spawn.
- `.tpnp` - Teleports the player to the nearest SCP-500 pill.

## Dependencies
- [Exiled API](https://github.com/ExMod-Team/EXILED)
- [Custom Items API](https://github.com/Exiled-Team/CustomItems)

## Credits
- **Developer**: LaFesta1749
- **Special Thanks**: Exiled Community

## License
This project is licensed under the **MIT License**.