# SCP-500-X Rework

SCP-500-X Rework is a plugin for SCP: Secret Laboratory that introduces a variety of custom SCP-500 pills, each with unique effects. These pills provide players with diverse gameplay experiences, ranging from teleportation to team-switching and model distortions.

## 📥 Installation
1. Download the latest release from the GitHub repository.
2. Place the `SCP500XRework.dll` file into your Exiled Plugins folder.
3. Restart your SCP:SL server.

## 📌 Available SCP-500 Pills
Each SCP-500 variant has a unique effect, impacting gameplay in different ways.

### 🔵 SCP-500-A (Revival Pill)
- **Effect:** Summons a dead player back into the game.
- **Mechanics:**
  - The revived player spawns at the pill user's location.
  - The revived player retains their previous team/faction.
  - Spawn protection is removed immediately after revival.
- **Restrictions:** Cannot be used in elevators or Pocket Dimension.

### 🟠 SCP-500-B (Betrayal Pill)
- **Effect:** Swaps the player's team to the opposite faction.
- **Mechanics:**
  - Class-D ↔ Scientist
  - MTF ↔ Chaos Insurgency
  - Facility Guard transforms randomly into SCP-049-2, Scientist, or Class-D.
  - Drops all inventory items before switching teams.
  - Spawn protection is removed upon transformation.
- **Restrictions:** Cannot be used in elevators or Pocket Dimension.

### 🍀 SCP-500-L (Lucky Boost)
- **Effect:** Grants rapid healing and movement speed boost for 30 seconds.
- **Mechanics:**
  - Vitality effect (fast regeneration)
  - Movement speed boost
  - After 30 seconds, effects fade away.

### 💊 SCP-500-O (Overdose Pill)
- **Effect:** Grants all SCP-500 effects at once, but weakened.
- **Mechanics:**
  - Provides healing, movement boost, damage resistance, and other effects.
  - Effects last for 15 seconds.
  - Weakened intensity compared to individual pills.

### 🎲 SCP-500-U (Unstable Pill)
- **Effect:** Grants a random SCP-500 effect after a 5-second delay.
- **Mechanics:**
  - Randomly selects an effect such as Movement Boost, Healing, or Damage Resistance.
  - The effect lasts for 15 seconds.
- **Restrictions:** Cannot be used in elevators or Pocket Dimension.

### ⚡ SCP-500-Y (Yo-Yo Pill)
- **Effect:** Oscillates between positive and negative effects.
- **Mechanics:**
  - Alternates between beneficial and harmful effects every 5 seconds.
  - Lasts for 5 cycles.

### 🔮 SCP-500-M (Distortion Pill)
- **Effect:** Distorts the player's model in various ways.
- **Mechanics:**
  - Changes model scaling randomly (e.g., wider, taller, upside-down, paper-thin).
  - Previous distortions are reset before applying a new one.
  - **Fix:** The max width is now 1.3x (previously 1.5x) to prevent getting stuck in doors.
- **Restrictions:** Cannot be used in elevators or Pocket Dimension.

### 🚀 SCP-500-T (Teleportation Pill)
- **Effect:** Teleports the user to a random location in the facility.
- **Mechanics:**
  - Randomly selects a valid room from Light Containment, Heavy Containment, Entrance, or Surface.
  - **Fix:** Prevents teleportation to `EzCollapsedTunnel` and `EzShelter`.
  - Has a 3-second delay before activation.
  - Applies a dizziness effect before teleportation.
- **Restrictions:** Cannot be used in elevators or Pocket Dimension.

## ⚙ Configuration
The plugin includes a `config.yml` file where you can enable/disable specific pills, adjust effect durations, and tweak intensity levels.

## 📢 Changelog
### Latest Updates:
- **SCP-500-A:** Now correctly revives players into their correct team instead of defaulting to Class-D.
- **SCP-500-M:** Adjusted the maximum width scale from `1.5f` to `1.3f` to allow passing through doors.
- **SCP-500-T:** Prevents teleportation to `EzCollapsedTunnel` and `EzShelter`.

## 🔗 Links
- [GitHub Repository](https://github.com/LaFesta1749/SCP500XRework)
- [Support Discord](https://github.com/LaFesta1749/SCP500XRework/#)

## 📜 License
This plugin is open-source and available under the MIT license. Contributions are welcome!

