# CountersPlus
A combination of widely-used counters, all configurable in one mod.

## What is this?
**Counters+** (Also known as CountersPlus) is a mod that bundles different counters, both widely used and quite unusual, into one mod. Counters+ also boasts a large amount of configuration, all available from a custom made settings menu that updates in real time!

## Available Counters
|Counter|Version Added|Description|
|-|-|-|
|***Progress***|`1.0.0`|Overwrites the in-game counter with the original we all know and love!|
|***Score***|`1.0.0`|Also overwrites with the original Score counter!|
|***Missed***|`1.0.0`|Counts missed notes in a song.|
|***Notes***|`1.0.0`|Notes hit over notes in total. Now in percentages!|
|***Speed***|`1.1.0`|*"Speed, motherfucker, do you speak it?"* Displays how fast your arms are swinging!|
|***Cut***|`1.1.0`|Displays the average cut score (Anywhere from 0-110), so you can see how well you are hitting those notes!|
|***Spinometer***|`1.4.1`|Displays angular velocity of your sabers. Can you beat 3600 degress per second?|
|***Personal Best***|`1.5.5`|Displays your high score in a map, and changes color depending on how close you are to beating it!|
|***Notes Left***|`1.5.8`|Notes Left counter has now been merged into Counters+. It's pretty self explanatory.|
|***Fail***|`1.5.8`|Display how many times you've failed across every song, or how many times you've restarted the same song!|

### Custom Counters
Counters+ also supports the addition of Custom Counters. Other mods can input their counters into the Counters+ system and allow the user to edit the position through the Counters+ UI.

#### Supported Custom Counters
If you wish to add your mod to this list, please DM me on Discord: *Caeden117#0117*

|Mod|Description|
|-|-|
|***FPS Counter***|Frames per second. Easy!|
|***Deviation Counter***|How early or late were you on cutting a block|

## Configuration

These are currently being moved over to the [Counters+ Wiki](https://github.com/Caeden117/CountersPlus/wiki).

### Plugin-wide settings
|Setting|Description|
|-|-|
|***Enabled***|Toggles the plugin on and off. The Counters+ UI will still be visible.|
|***Advanced Counter Properties***|Displays more information about the counters in the new menu.|
|***Combo Offset***|Before Distance is taken into account, how far away should counters be from Combo.|
|***Multiplier Offset***|Before Distance is taken into account, how far away should counters be from the Multiplier.|
|***Hide Combo***|Will attempt to hide the Combo counter when entering a song.|
|***Hide Multiplier***|Will attempt to hide the Multiplier counter when entering a song.|

### Every Counter has these 3 options
|Setting|Description|
|-|-|
|***Enabled***|Toggles the counter on or off.|
|***Position***|Sets the position of a counter relative to commonly used UI elements (Combo, Multiplier, Highway, etc.)|
|***Distance***|How far away from the Position a counter will be. A higher Distance would set the Counter farther away from the UI element defined in *Position*.|

### Advanced Options
|Setting|Counters|Description
|-|-|-|
|***Percentage Precision***|*Notes*, *Score*, *Speed*, and *Personal Best*|How precise do the decimals go?|
|***Show Precentage***|*Notes*|Displays the percentage of notes hit over notes in total.|
|***Display Rank***|*Score*|Displays the Rank you get during a song (SS, S, A, B, C, D, E)|
|***Progress From End***|*Progress*|The counter starts at full, and decreases as you progress though a song.|
|***Include Progress Ring***|*Progress*|Whether or not the *Progress From End* option will also effect the Progress Ring. Only available in *Original* mode.|
|***Mode***|*Speed*, *Progress*, *Spinometer*, and *Score*|Changes the display mode for the Counter (See Hint Text in-game for more detail)|
|***Text Size***|*Personal Best*|How large should the counter be?|
|***Below Score Counter***|*Personal Best*|Whether or not the Personal Best counter should be displayed below the Score Counter instead.|
|***Custom Miss Text Integration***|*Miss*|When Custom Miss Text is installed, replace the "Misses" label with one of the various insults.|
|***Track Restarts***|*Fail*|Instead of showing global fail count, instead show the times you've restarted the same song.|
|***Label Above Count***|*Notes Left*|Changes the Notes Left counter to look similarly with other Counters+ counters, with the label above the number.|
|***Hide First Score***|*Personal Best*|Hides your personal best if the song you are playing has not yet been completed.|

### Some Notes

- If ***Advanced HUD*** is Disabled:
  - If ***Progress Counter***'s *Mode* is set to **Base**:
    - Nothing will appear.
  - If ***Score Counter***'s *Base Game* option is set to **True**:
    - Only the score will appear. No rank.
  - If ***Score Counter***'s *Base Game* option is set to **False**:
    - No score will appear.

## For Developers
If you wish to add your own custom counter to Counters+, or see how it can be used in your plugin, see the [Wiki page.](https://github.com/Caeden117/CountersPlus/wiki/For-Developers)

### Contributing to Counters+
In order to build this project, please add your Beat Saber directory path to the `Counters+.csproj.user` file located in the project directory.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!-- Set "YOUR OWN" Beat Saber folder here to resolve most of the dependency paths! -->
    <BeatSaberDir>E:\Program Files (x86)\Steam\steamapps\common\Beat Saber</BeatSaberDir>
  </PropertyGroup>
</Project>
```

If you plan on adding any new dependencies which are located in the Beat Saber directory, it would be nice if you edited the paths to use `$(BeatSaberDir)` in `Counters+.csproj` like so to keep some consistency

```xml
...
<Reference Include="BS_Utils">
  <HintPath>$(BeatSaberDir)\Plugins\BS_Utils.dll</HintPath>
</Reference>
<Reference Include="IPA.Loader">
  <HintPath>$(BeatSaberDir)\Beat Saber_Data\Managed\IPA.Loader.dll</HintPath>
</Reference>
...
```