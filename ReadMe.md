# Godhome Randomizer

A Randomizer add-on for Godhome checks.

## Description

### Hall of Gods

This mod will randomize up to four locations for each Hall of Gods Boss based on their tier:
- Unlock --> An item is granted when interacting with a statue for the first time.
- Attuned --> An item is granted when defeating any version.
- Ascended --> An item is granted when defeating the Ascended or Radiant version.
- Radiant --> An item is granted when defeating the Radiant version.

The Statue Mark items will improve the statue's state for each boss by 1, meaning you'll unlock the statue and then get the Attuned, Ascended and Radiant marks in progressive order. The amount of copies of this item present in the playthrough will depend on the mod settings. Randomizing the statue access will add one copy, while randomizing the tier limits will add up to three extra copies, matching the four location types.

The ordeal location will grant an item instead of turning Zote's statue into gold and unlocking the Eternal Ordeal menu. The ordeal item will only turn the statue into gold. If you never completed the Eternal Ordeal, doing this with the mod available will not provide the secret menu reward.

### Pantheons

Ever got the feeling that doing all 5 pantheons with every single binding was just too painful for them to just grant you one single journal entry? Well, they will still be as painful, but now we can have checks added to that. Bonus points if for some reason you decide you hate yourself and want to play with All Bindings and Hitless options.

This allows you to randomize the obtention status of the Pantheon Bindings (Nail/Shell/Charms/Soul) both individually and all at once, and also the status of a hitless Pantheon run.

__NOTE:__ To obtain Godhome Lifeblood and Weathered Mask you need to get the individual binding item (or location if vanilla). Getting the "All Bindings" check doesn't add to the completion counter.

### Lifeblood

The mysterious blue room that contains a giant lifeblood cocoon can also be randomized. If you obtain the item, lifeseeds will spawn on Godhome's resting rooms. The room itself will now grant a random item instead of that bonification.

### Godhome Shop

Since this mod allows to add a significant number of duplicate items (44 if duping the Statue Marks), a counter-measure to prevent regular shops from being too overcrowded is the introduction of a new shop. This shop will check for Statue Marks obtained and use them as currency. It works similar to Grubs or essence, meaning it checks if you achieved a given threshold of Statue Marks, without actually consuming them.

An obtained Statue Mark means that a statue:
- Has been unlocked.
- Has an attuned cleared mark.
- Has an ascended cleared mark.
- Has a radiant cleared mark.

All this can add up to a whopping total of __214__ new locations and items to the Randomization pool!

## Settings:
- Enabled --> Boolean to define if the connection should be active or not.

### Godhome Shop
- Enabled --> Boolean to define if the Godhome Shop should appear or not.
- Minimum/Maximum Cost --> A range from 0 to 1, where 0 is 0% and 1 is 100% (176) of all available marks. Default is 25% (44) to 75% (132). Important notes:
    - A cost of 45 expects to clear at least one attuned boss if not randomized.
    - A cost of 89 (over 50%) expects to clear at least one ascended boss if not randomized.
    - A cost above 133 will require radiant clears if not randomized.

### Hall of Gods
- Randomize access --> Defines if statue access is vanilla, randomized or if all of them are unlocked by default. This last option will likely force you to fight Godhome bosses earlier in the progression.
- Limit --> Options: IncludeAll, ExcludeRadiant, ExcludeAscended, Vanilla. Defines which locations should be randomized and contain items and which should remain vanilla. Excluding Ascended also Excludes Radiant entries.
- Randomize Ordeal --> This adds the Glorious Invincible Fearless Sensual Mysterious Enchanting Vigorous Diligent Overwhelming Gorgeous Passionate Terryfing Beautiful Powerful Life Ender Mighty Zote's ordeal to the pool.
- Duplicate Marks --> This will add an extra copy of the Statue Mark items, while not affecting any logic behind them. Useful to complete HOG or unlock pantheons earlier in progression.

### Pantheons
- Apply access to Pantheon --> Defines if the access rules provided to Hall of Gods should also apply to Pantheons or leave it vanilla.
- Pantheons included --> Defines the top Pantheon to randomize. IE, if "Sage" is selected, Pantheons 1, 2 and 3 will be added to the pool.
- Completed: Checks if a Pantheon is completed or not.
- Lifeblood: Randomizes the blue room reward.
- Bindings:
    - Nail --> Checks if the nail binding is randomized or not.
    - Shell --> Checks if the shell binding is randomized or not.
    - Charms --> Checks if the charms binding is randomized or not.
    - Soul --> Checks if the soul binding is randomized or not.
    - AllAtOnce --> Randomizes all bindings at once. NOTE: Acquiring this doesn't provide the individual bindings for the blue room/weathered mask access.
    - Hitless --> Randomizes the wondrous talent of doing a hitless run.

## Dependencies:
- ItemChanger
- KorzUtils
- MenuChanger
- Randomizer 4
- RandomizerCore

## Integrations:
- ExtraRando: When using the "Randomize Pantheon Access" option, it takes precedence over the settings defined by this mod.
- FStats: When enabled, a new page for diverse Godhome achievements will be added, and it'll display the achievements regardless of randomization settings for them.
    - All Statues Unlocked: Obtained by unlocking all Hall of Gods statues.
    - All Attuned: Obtained a bronze mark for all Hall of Gods statues.
    - All Ascended: Obtained a silver mark for all Hall of Gods statues.
    - All Radiant: Obtained a gold mark for all Hall of Gods statues.
    - Eternal Ordeal: Turned Zote's statue into gold.
    - Pantheon 1 Completed: Obtained by clearing P1.
    - Pantheon 2 Completed: Obtained by clearing P2.
    - Pantheon 3 Completed: Obtained by clearing P3.
    - Pantheon 4 Completed: Obtained by clearing P4.
    - Pantheon 5 Completed: Obtained by clearing P5.
    - Blue Door Unlocked: Obtained by clearing 8 pantheon bindings.
    - All Lifeblood Unlocked: Obtained by clearing 16 pantheon bindings.
    - All Bindings Cleared: Obtained by clearing all 20 pantheon bindings.
    - Unleashed Pantheons: Obtained by clearing all pantheons with all bindings at once.
    - Unscarred Pantheons: Obtained by clearing all pantheons without receiving damage.

- Lost Artifacts: There is one element there that has its logic altered by the randomizing of Godhome assets.
- More Locations: Having the mod enabled will allow for Statue Marks to be included as currency for the Junk Shop.
- RandoSettingsManager
- TheRealJournalRando: This mod has quite a few interactions with TRJR:
    - Since access to boss fights can be altered, the logic now contemplates the option of beating them through Hall of Gods to obtain their journal entries.
    - This also applies for regular entries that are summoned by bosses (IE: Folly from Soul Warrior or Zotelings).
    - Access to the Weathered Mask Journal Entry may be affected if bindings are randomized, so the logic gets adjusted.
    - Access to the Void Idol Journal Entry now checks for this mod's statue marks for each tier.
