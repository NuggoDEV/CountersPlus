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
|***Spinometer***|`1.4.1`, suggested by [Ragesaq](https://www.twitch.tv/ragesaq) and [Steven](https://www.twitch.tv/StevenTheCat)|Displays angular velocity of your sabers. Can you beat 3600 degress per second?|

### Custom Counters
Counters+ also supports the addition of Custom Counters. Other mods can input their counters into the Counters+ system and it will work just like any other Counters+ counter!

#### Supported Custom Counters
If you wish to add your mod to this list, please DM me on Discord: *Caeden117#0117*

|Mod|Description|
|-|-|
|***YURFit***, previously *BeFit*|Among other things, displays a calorie counter that increases while you play a song.|
|***Notes Left Counter***|Displays the remaining notes in a song. Easy as that!|

## Configuration

### Plugin-wide settings
|Setting|Description|
|-|-|
|***Enabled***|Toggles the plugin on and off. The Counters+ UI will still be visible.|
|***Advanced Counter Properties***|Displays more information about the counters in the new menu.|
|***Combo Offset***|Before Distance is taken into account, how far away should counters be from Combo.|
|***Multiplier Offset***|Before Distance is taken into account, how far away should counters be from the Multiplier.|

### Every Counter has these 3 options
|Setting|Description|
|-|-|
|***Enabled***|Toggles the counter on or off.|
|***Position***|Sets the position relative to commonly used UI elements (Combo, Multiplier, etc.)|
|***Distance***|Otherwise known as the order. A higher Distance would set the Counter farther away from the position.|

### Advanced Options
|Setting|Counters|Description
|-|-|-|
|***Percentage Precision***|*Notes*, *Score*, and *Speed*|How precise do the decimals go?|
|***Show Precentage***|*Notes*|Displays the percentage of notes hit over notes in total.|
|***Display Rank***|*Score*|Displays the Rank you get during a song (SS, S, A, B, C, D, E)|
|***Progress From End***|*Progress*|The counter starts at full, and decreases as you progress though a song.|
|***Include Progress Ring***|*Progress*|Whether or not the *Progress From End* option will also effect the Progress Ring. Only available in *Original* mode.|
|***Mode***|*Speed*, *Progress*, *Spinometer*, and *Score*|Changes the display mode for the Counter (See Hint Text in-game for more detail)|

### Some Notes

- If ***Advanced HUD*** is Disabled:
  - If ***Progress Counter***'s *Mode* is set to **Base**:
    - Nothing will appear.
  - If ***Score Counter***'s *Base Game* option is set to **True**:
    - Only the score will appear. No rank.
  - If ***Score Counter***'s *Base Game* option is set to **False**:
    - No score will appear.

## For Developers
For plugin developers who plan on adding a counter of their own, Counters+ has a way to easily integrate your created counter into the Counters+ system.

Adding a Custom Counter will not make your plugin dependent on Counters+ (Unless you *really* want it to be).

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
			SectionName = "testCounter", //Used as an identifier in the Counters+ config file. Don't plan on changing this.
			Name = "Test", //Display name that will appear in the Counters+ settings list.
			Mod = this, //IPA Plugin. Will show up in Credits in the Counters+ settings  list.
			Counter = "testCounterGameObject", //Name of the GameObject that holds your Counter component. Used to hook into the Counters+ system.
		};
		CustomCounterCreator.CreateCustomCounter(counter);
	}

}
```

A few things to keep in mind:
1. Try to keep your counters under one GameObject each (Have anything else as a child), you can only add one GameObject per custom counter!
2. Add Counters before the Menu scene is loaded (It'll thrown an exception after), this is so it can create UI in settings without having to reload.
3. Your `SectionName` is an identifier in the Counters+ config file. Make sure this identifier is unique, and will not be used by anyone else.
4. Do not plan on changing `SectionName` after you release your first public build that uses Counters+ custom counters.

And you're done! If it has not yet been created, Counters+ will append the custom counter to its `CountersPlus.ini` file, and will go off of that from now on. Your Counter can now be subject to the same base configuration settings as every counter!

Custom Counters no longer have a `Delete` option. Instead, simply disabling the Custom Counter via the Counters+ menu will achieve the same effect. ~~to be honest though I should probably add that back~~
