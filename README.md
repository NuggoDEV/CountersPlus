# Hey You!
![Brand new Counters+ UI!](https://i.imgur.com/gDzdybO.png "Brand new Counters+ UI!")

Yes, you! Do you want to help Counters+ become bug free before the next major update?

I have been hard at work over at the [UI Rework branch](https://github.com/Caeden117/CountersPlus/tree/ui-rework), and I believe I am at a spot where I can release a beta build for people to mess around and report back any bugs.

*I may put your name in the contributors list if you help out,* so that's something for you!

## Interested in testing?

[Download the beta build right here.](https://drive.google.com/file/d/1o1QCQ4z4UXaCRVq_0OCAgf7YfBGZdF_a/view?usp=sharing) Check it out, and mess around with the brand spanking new Counters+ menu!

Report any bugs and issues over [here.](https://github.com/Caeden117/CountersPlus/issues) If you believe you may have a solution to these problems, fork the UI rework branch, and send me a pull request.

Good luck testing!

*Update 1:* I just got Score Counter working again, that took fucking forever.

---

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
|***YURFit***, previously *BeFit*|Among other things, displays a calorie counter that increases while you play a song.|
|***Notes Left Counter***|Displays the remaining notes in a song. Easy as that!|

## Configuration

### Plugin-wide settings
|Setting|Description|
|-|-|
|***Enabled***|Toggles the plugin on and off, and hides all other submenus if disabled!|
|***Disable Menus***|Hides other submenus while keeping Counters+ enabled.|
|***Combo Offset***|Before Index is taken into account, how far away should counters be from Combo.|
|***Multiplier Offset***|Before Index is taken into account, how far away should counters be from the Multiplier.|

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
|***Use Base Game Counter***|*Score*|Whether or not to use the base game counter instead of replacing it with our own. **Some features will not be reflected when using the base game counters!**|
|***Show Precentage***|*Notes*|Displays the percentage of notes hit over notes in total.|
|***Display Rank***|*Score*|Displays the Rank you get during a song (SS, S, A, B, C, D, E)|
|***Progress Time Left***|*Progress*|The counter starts at full, and decreases as you progress though a song.|
|***Mode***|*Speed*, *Progress*|Changes the display mode for the Counter (See Hint Text in-game for more detail)|

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
			SectionName = "testCounter", //Used as an identifier. Don't plan on changing this.
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

And you're done! If it has not yet been created, Counters+ will append the custom counter to its `CountersPlus.ini` file, and will go off of that from now on. Your Counter can now be subject to the same base configuration settings as every counter!

Custom Counters also have a Delete option (To delete custom counters), and a Credits option which will display the mod that created the counter.
