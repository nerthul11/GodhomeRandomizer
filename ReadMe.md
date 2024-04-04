# Godhome Randomizer

A Randomizer add-on for Godhome checks.

## Description

### Hall of Gods

This mod will randomize up to four locations for each Hall of Gods Boss based on their tier:
- Unlock --> An item is granted when interacting with a statue for the first time.
- Attuned --> An item is granted when defeating the Attuned version.
- Ascended --> An item is granted when defeating the Ascended version.
- Radiant --> An item is granted when defeating the Radiant version.

The Statue_Mark items will improve the statue's state for each boss by 1, meaning you'll unlock the statue and then get the Attuned, Ascended and Radiant marks in progressive order. The amount of copies of this item will depend on the mod settings. Randomizing the statue access will add one copy, while randomizing the tier limits will add up to three extra copies, matching the four location types.

### Pantheons

Ever got the feeling that doing all 5 pantheons with every single binding was just too painful for them to just grant you one single journal entry? Well, they will still be as painful, but now we can have checks added to that. Bonus points if for some reason you decide you want to play with All Bindings and manage to do it Hitless.

This allows you to randomize the obtention status of the Pantheon Bindings (Nail/Shell/Charms/Soul) both individually and all at once, and also the status of a hitless Pantheon run.

__NOTE:__ To obtain Godhome Lifeblood you need to get the individual binding item (or location if vanilla). Getting the "All Bindings" check doesn't add to the lifeblood counter.

All this can add up to a whopping total of __206__ new items and locations to the Randomization pool!

## Settings:
- Enabled --> Boolean to define if the connection should be active or not.

### Hall of Gods
- Randomize access --> Defines if statue access is vanilla, randomized or if all of them are unlocked by default. This last option will likely force you to fight Godhome bosses earlier in the progression.
- Limit --> Options: IncludeAll, ExcludeRadiant, ExcludeAscended, Vanilla. Defines which locations should be randomized and contain items and which should remain vanilla. Excluding Ascended also Excludes Radiant entries.

### Pantheons
- Apply access to Pantheon --> Defines if the access rules provided to Hall of Gods should also apply to Pantheons or leave it vanilla.
- Pantheons included --> Defines the top Pantheon to randomize. IE, if "Sage" is selected, Pantheons 1, 2 and 3 will be added to the pool.
- Bindings:
    - Nail --> Checks if the nail binding is randomized or not.
    - Shell --> Checks if the shell binding is randomized or not.
    - Charms --> Checks if the charms binding is randomized or not.
    - Soul --> Checks if the soul binding is randomized or not.
    - AllAtOnce --> Randomizes all bindings at once. NOTE: Acquiring this doesn't provide the individual bindings.
    - Hitless --> Randomizes the wondrous talent of doing a hitless run.

## Dependencies:
- ItemChanger
- KorzUtils (for the moment, might remove in future patches if I get a better understanding of sprite rendering)
- MenuChanger
- Randomizer 4
- RandomizerCore

## Integrations:
- ExtraRando: When using the "Randomize Pantheon Access" option, it takes precedence over the settings defined by this mod.
- Lost Artifacts: There is one element there that has its logic altered by the randomizing of Godhome assets.
- RandoSettingsManager
- TheRealJournalRando: This mod has quite a few interactions with TRJR:
    - Since access to boss fights can be altered, the logic now contemplates the option of beating them through Hall of Gods to obtain their journal entries.
    - Access to the Weathered Mask Journal Entry may be affected if bindings are randomized, so the logic gets adjusted.
    - Access to the Void Idol Journal Entry now checks for this mod's statue marks for each tier.

## Known issues

- The "All unlocked" setting fails when paired with "Vanilla" Hall of Gods tier limit and the access mode is set to vanilla, so, don't use them together as it will not behave as intended and can make a seed impossible.
- If every single other Randomizer settings are vanilla AND statue access is randomized, there could be unreachable location issues.