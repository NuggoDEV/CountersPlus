# CountersPlus
A combination of widely-used counters, all configurable in one mod.

## What is this?
**Counters+** (Also known as CountersPlus) is a mod that combines counters, both new and widely used, into one mod. Counters+ also supports a large amount of customization, going from positions and ordering, to even using the counters available in game!

## Available Counters
|Counter|Version Added|Description|
|-|-|-|
|***Progress***|`1.0.0`|Overwrites the in-game counter with the original we all know and love!|
|***Score***|`1.0.0`|Also overwrites with the original Score counter!|
|***Missed***|`1.0.0`|Counts missed notes in a song.|
|***Notes***|`1.0.0`, previously *Accuracy*|Notes hit over notes in total. Now in percentages!|
|***Speed***|`1.1.0`, suggested by [Ragesaq](https://www.twitch.tv/ragesaq)|*"Speed, motherfucker, do you speak it?"* Displays how fast your arms are swinging!|
|***Cut***|`1.1.0`|Displays the average cut score (Anywhere from 0-110), so you can see how well you are hitting those notes!|

### Custom Counters
Counters+ also supports the addition of Custom Counters. Other mods can input their counters into the Counters+ system and it will work just like any other Counters+ counter!

#### Supported Custom Counters
If you wish to add your mod to this list, please DM me on Discord: *Caeden117#0117*

|Mod|Description|
|-|-|
|***BeFit***|Among other things, displays a calorie counter that increases while you play a song.|
|***Notes Left Counter***|Displays the remaining notes in a song. Easy as that!|

## Configuration

### Plugin-wide settings
|Setting|Description|
|-|-|
|***Enabled***|Toggles the plugin on and off, and hides all other submenus if disabled!|
|***RNG***|Adds some RNG flair to your counters. Change different settings over time!|
|***Disable Menus***|Hides other submenus while keeping Counters+ enabled.|

### Every Counter has these 3 options
|Setting|Description|
|-|-|
|***Enabled***|Toggles the counter on or off.|
|***Position***|Sets the position relative to commonly used UI elements (Combo, Multiplier, etc.)|
|***Index***|Otherwise known as the order. A higher Index would set the Counter farther away from the position.|

### Advanced Options
|Setting|Counters|Description
|-|-|-|
|***Decimal Precision***|*Notes*, *Score*, and *Speed*|How precise do the decimals go?|
|***Override Base Game Counter***|*Score*|Whether or not to use the base game counter instead of replacing it with our own. **Some features will not be reflected when using the base game counters!**|
|***Show Precentage***|*Notes*|Displays the percentage of notes hit over notes in total.|
|***Display Rank***|*Score*|Displays the Rank you get during a song (SS, S, A, B, C, D, E)|
|***Progress Time Left***|*Progress*|The counter starts at full, and decreases as you progress though a song.|
|***Mode***|*Speed*, *Progress*|Changes the display mode for the Counter (See Hint Text in-game for more detail|

### Some Notes

- If ***Advanced HUD*** is Disabled:
  - If ***Progress Counter***'s *Mode* is set to **Base**:
    - Nothing will appear.
  - If ***Score Counter***'s *Override* option is set to **True**:
    - Only the score will appear. No rank.
  - If ***Score Counter***'s *Override* option is set to **False**:
    - No score will appear.

## For Developers
For plugin developers who plan on adding a counter of their own, Counters+ has a way to easily integrate your created counter into the Counters+ system.

Adding your own Counter is a simple as:

```csharp
using CountersPlus.Custom; //Add CountersPlus.dll as a Reference
using IllusionInjector; //Add References from Beat Saber_Data/Managed

public class Plugin : IPlugin {

	public void OnApplicationStart(){
		if(PluginManager.Plugins.Any(x => x.Name == "Counters+")) {
			AddCustomCounter();
		}//If the user does not have Counters+ installed, don't worry about it.
	}
	
	void AddCustomCounter(){
		CustomCounter counter = new CustomCounter {
			JSONName = "testCounter", //Name in config system. Also used as an identifier. Don't plan on changing this.
			Name = "Test", //Display name that will appear in the SettingsUI.
			Mod = this, //IPA Plugin. Will show up in Credits in the SettingsUI.
			Counter = "testCounterGameObject", //Name of the GameObject that holds your Counter component. Used to hook into the Counters+ system.
		};
		CustomCounterCreator.CreateCustomCounter(counter);
	}

}
```

A few things to keep in mind:
1. Try to keep your counters under one GameObject each (Have anything else as a child), you can only add one GameObject per custom counter!
2. Add Counters before the Menu scene is loaded (It'll thrown an exception after), this is so it can create UI in settings without having to reload.

And you're done! If it has not yet been created, Counters+ will create a configuration `.json` file in the `Custom Counters` folder of UserData, and will go off of that from now on. Your Counter can now be subject to the same base configuration settings as every counter!

Custom Counters also have a Delete option (To delete custom counters), and a Credits option which will display the mod that created the counter.
