# SCP-500X Rework 🔬💊
A **custom item expansion** for SCP: Secret Laboratory, adding **unique SCP-500-based pills** with special effects.

---

### 📌 **What's Included in This README?**
✅ **Formatted config.yml** example  
✅ **List of all SCP-500X pills** with detailed effects  
✅ **Installation guide**  
✅ **Commands & permissions**  
✅ **Credits & contact links**  

---

## ⚙️ **Configuration (config.yml)**

```yaml
SCP500XRework:
  EnablePlugin: true  # Enables or disables the plugin
  EnableLogging: true  # Logs pill usage in the console
  SCP500-Chance: 0.1   # Chance for SCP-500X pills to spawn instead of normal SCP-500
  SCP500-SpawnLocations:
    - LCZ
    - HCZ
    - Entrance
    - Surface
  SCP500-Pills:
    SCP-500-M: true  # Model distortion pills
    SCP-500-H: true  # Health boost pills
    SCP-500-I: true  # Invisibility pills
    SCP-500-S: true  # Speed boost/slowing pills
    SCP-500-T: true  # Random teleport pills
    SCP-500-V: true  # Vampire pills (steal HP)
    SCP-500-X: true  # Explosive doors pills
    SCP-500-Y: true  # Alternating positive/negative effects
    SCP-500-Z: true  # Zombie transformation pills
    SCP-500-F: true  # Fake death pills
    SCP-500-P: true  # Power boost (damage increase)
    SCP-500-L: true  # Luck pills (healing & speed)
    SCP-500-O: true  # Overdose pills (all effects but weaker)
    SCP-500-U: true  # Unstable pills (random SCP-500 effect)
```

🧪 SCP-500X Pills List
Each pill has unique effects when consumed.

🔹 SCP-500-M (Model Pills)
📌 Distorts your character model randomly.
Possible distortions:
- Upside-down character
- Backward-facing character
- Paper-thin model
- Short, floating appearance
- Wide or tall body
- Giant head, hands, or legs

❤️ SCP-500-H (Health Pills)
📌 Increases your max HP permanently for the round.
- Each use adds +20 HP, stackable.
- Medkits, adrenaline, and SCP-500 won't reset your HP to 100.

🕶 SCP-500-I (Invisibility Pills)
📌 Turns you invisible for 8 seconds.
- Ends early if you interact with a door.

⚡ SCP-500-S (Speed Pills)
📌 Grants a speed boost (or sometimes slows you down).
- Speed boost: Intensity 70-200
- Slow effect: Randomized duration
- Lasts 10 seconds.

🎲 SCP-500-T (Teleport Pills)
📌 Teleports you to a random zone.
- Possible locations: LCZ, HCZ, Entrance, Surface
- Safe spawn logic: Ensures you do not fall out of bounds.

🦇 SCP-500-V (Vampire Pills)
📌 Steals 10 HP from nearby enemies and converts it into AHP.
- Affects all enemies within 8 meters.
- Stolen HP is added as AHP to the user.

💥 SCP-500-X (Explosive Pills)
📌 Breaks or opens all nearby doors.
- If breakable, the door explodes.
- If unbreakable, the door opens instead.
- Exclusion: 079_FIRST and 079_SECOND doors cannot be affected.

🔄 SCP-500-Y (Yoyo Pills)
📌 Swaps between positive and negative effects every 5 seconds.
- Duration: 25 seconds total.
- Alternates between: Speed boost, slowdown, HP regen, damage weakness, etc.

🧟 SCP-500-Z (Zombie Pills)
📌 Kills you and revives you as an enhanced SCP-049-2.
- Respawns with 400 HP and increased movement speed.

🎭 SCP-500-F (Fake Death Pills)
📌 Fakes your death for 10 seconds, then revives you.
- Spawns a ragdoll body.
- You become invisible & invincible for the duration.
- After 10 sec, you "revive" at the same spot.

🏋 SCP-500-P (Power Pills)
📌 Increases your damage output by +25% for 20 seconds.
- All weapons & melee (jailbird) attacks deal more damage.

🍀 SCP-500-L (Lucky Pills)
📌 Gives you fast healing and a speed boost.
- Duration: 30 seconds.
- Effects: Faster movement & increased HP regen.

☠ SCP-500-O (Overdose Pills)
📌 Applies all SCP-500 effects at once, but at 50% potency.
- Rare pill, only a small chance to spawn.

🎲 SCP-500-U (Unstable Pills)
📌 After 5 seconds, you receive a random SCP-500 effect.
- Effect lasts 15 seconds.
- Random selection from all existing SCP-500 pills.

🛠 Installation Guide
🔹 Prerequisites
Requires EXILED framework installed on your SCP:SL server.
Custom Items plugin must be enabled.

🔹 Installation Steps
Download the latest release from GitHub Releases.
Place the .dll file into:
`/plugins/EXILED/`
Modify the `config.yml` settings as needed.
Restart your SCP:SL server.

📝 Commands & Permissions

Command	| Description	                            | Permission
.givepill [player] [pill]	Gives a pill to a player	scp500x.give
.pillinfo [pill]	Shows info about a specific pill	scp500x.info

🛠 Developer Notes
- Built using Exiled API.
- Compatible with SCP:SL 13.x and newer.
- More effects & balance changes planned in future updates.

📜 Credits
👨‍💻 Developer: **LaFesta1749**
📢 Special Thanks: Exiled Team for the API & documentation.
