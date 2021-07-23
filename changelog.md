
## Beta-1
	Initial update for 1.2.1

## Beta-2
	Bug fixes, changed settings storage from local file to new KSP settings 

## Release 0.7.0
	Added Snacks.cfg to take care of the Snacks! tanks

## Release 0.7.1
	Updated ModuleManager to 2.7.3
	Updated WheelMotor.cfg & Wheeltire.cfg, changed ModuleWheel to ModuleWheelBase
	Added WheelMotor
	Added WheelTire
	Fixed glow to work even when going EVA
	Added ability to enable or disable failure mode for each type of module
	Fixed bug where switching to vessel in flight wouldn't properly initialize the popup buttons
	Updated the Radial bay to only allow access if it is opens
	Added spare parts to cargo bays
	Added code so that any cargo bay with parts will have to be open before accessing the parts
	Added DeployableAntenna
	Removed requirement for the Community Resource Pack, but is still compatible with it
	Following are from Entropy, originally by coffeeman
		Added Parachutes
		Added Motors (Animations)
		Added Generators
		Added Solar Panels
		Added SRBs
## Release 0.7.2
	Fixed settings for new games

## Release 0.7.3
	Fixed typo in SpareParts.cfg, line 153, "ModuleCargoBay" instead of "ModuleCargeBay"
	Fixed SpareParts.cfg Cargo section, so it now works properly and doesn't add to all parts

## Release 0.7.4
	Fixed problem with each additional module added to the parts was increasing the cost of the part by the part cost.

## Release 0.7.5
	Fixed problem where more than one Module of what was being looked for caused InvalidOperationExceptions, due to 
	the fact that the Linux Single() function throws that error if there is more than one

## Release 0.7.6
	Added MTBF multiplier
	Added Lifetime multipler
	Moved some items around in the settings window

## Release 0.7.7
	Changed minimum value for alarm from 1 to 0 
	Recompiled to fix issue of sound restarting when coming back from map view

## Release 0.7.8
	Added code to prevent the ResetShipGlow from being executed every fixed update

## Release 0.7.9
	Replaced the quick fix for the ResetShipGlow in previous patch with code sent by @EugeneButrik, a much better solution
	Fixed bug (again found and fixed by @eugeneButrik) where changing vessels would have failed parts not highlighted anymore

## Release 0.7.10
	Removed the lastTimeReset code, wasn't needed after 0.7.9
	Changed Log.Info to Logger.debug, to reduce log spamzt 

## release 0.7.11
	Thanks to EugeneButrik, removing unnecessary and old functions
	Added option to disable failed part glowing when F2 is pressed
	Fixed bug where if a part can have multiple parts, the highlighting woudln't consistently work.  This happens on SRBs (maybe others)
	Fixed typo in SRBs_enginesFX.cfg and in SRBs.cfg
	Fixed bug  when manual fails are allowed, the buttons weren't always set properly (they should just show Fail)

## release 0.7.12
	Fixed bug when cargo bay didn't have deplpyable module, was giving nullref

## release 0.7.13-beta
	Fixed nullref in WheelMotor on start
	Restricted alarm to sounding only when part is failed on active vessel
	Changed Update to LateUpdate in AlarmManager
	Restricted failures to active vessel only (in FailureModule)
	Updated for 1.3

## release 0.7.13.1
	Updated Snacks MM patch

## release 0.7.14
	Added button, calls modifier window, to add a modifier to the chances of the fialure with configurable decay of modifier


## release 0.7.14.1
	Fixed multiple toolbar buttons

## release 0.7.15
	updated for KSP 1.3.1

## release 0.7.16
	Fixed MM patches for new MM 3.0.1

## 0.7.17
	Updated for 1.4.1+
	Added support for ToolbarController
	Added support for ClickthroughBlocker

## 0.7.17.1
	Thanks to github user @bogen--ksp for this:
		Make parts repairable after reload

## 0.7.18.0
	Fixed alarm sound resounding when coming back from map view on failed batteries

## 0.7.18.1
	Version bump for 1.5 rebuild

## 0.7.18.3
	Version bump for 1.7.2
	Updated AssemblyVersion.tt for device-independent builds

0.7.18.4
	Thanks to github user @gnbo for this:
		Fixes error when in non-english language errors are thrown when repair attempts are made
		
0.7.19
	Added Installchecker
	Updated MiniAVC to 1.4
	Rebuild for 1.8

0.7.19.1
	Updated MiniAVC.dll to 1.4.0.2 to fix a nasty CTD

0.7.19.2
	Really did the MiniAVC update

0.7.20
	Updated toolbarControl code
	Removed useBlizzy option, now controlled by the ToolbarController
	Fixed nullref when landing gear exist which aren't wheels
	Replaced internal part highlight code with KSP_PartHighlighter
	Broken parts now flash instead of being solid
	Added new options in stock Settings page to control flash speed
	Added new dependency on the SpaceTuxLibrary
	Added new monitor window, to show all the failed parts
	Some code cleanup
	Replaced internal logging with KSP_Log
	Change toolbar button visibility to be spacecenter only for config, flight only for status
	Cleaned up logging code

0.7.20.1
	Added more tooltips to stock settings page
	Thanks to github user @ippo343 for this:
		Lowered additional part cost for storage containers significantly. Storage in 
			command pods and cargobay costs now 14$ per unit of spare parts instead 
			of 63$, making it slightly more expensive compared to the DangIt Repair Bay (12.8$ per unit).

		Additionaly made the resource initiation check for CommunityResourcePack and 
			made SpareParts transferable according to a suggestion by forum user Fulgora

0.7.20.2
	Fixed longstanding issue of alarm playing again after leaving the map view

0.7.20.3
	Disabled the highlighting when a part is repaired

0.7.20.4
	Fixed nullref in fixedUpdate when a kerbal went EVA
	Fixed new status window properly disabling the correct parts
	Changed dictionary key from Part to a hashed key
	Added alarmDisabled flag to allow alarm to be permenantly disabled
	Added support for KerbalChangeLog

0.7.20.5
	Added and improved tooltips on the stock settings page
	My thanks to forum user @Daniel Prates for his willingness to be a QA tester for me

0.7.20.6
	Added AssemblyFileVersion
	Updated version file for 1.12
	Updated dll for CKAN compatibility

============== Coffeeman's work below =========================
## ALPHA 6

### ALPHA 6.3
 - Added Daishi's amazing (and horribly, terribly, rediculously late on Coffeeman's part -- i'm so sorry) spare parts bay
 - Move to SpaceDock.info after KerbalStuff's implosion

### ALPHA 6.2
- Supports KSP 1.0.5
- Allows fully disabling on a per-save basis
- Fixes issue where you could use the amount tweaker to get half a spare part
- Correctly check temp when doing maintenance
- Correctly apply ModuleGimbalReliability to engine parts using ModuleEnginesFX 
- Move max temp config to a config file

### ALPHA 6.0.1
- Fix Ablator Leaking (Thanks @Corax)

### ALPHA 6.0
- Compatible with KSP 1.0.2
- Add FailureModules for steering and tires
- Rework experience system using the Stock XP system.
 - Configurable in the Module
 - Unfortunatley at this point nothing is refunded for having bought upgrades to Kerbal Experience. Sorry! (You can hack your funds up, it's ok, you have my permission :P )
 - Toggalable in settings
- Fully compatible with MissionController
- Tweaks to engine reliability calculation

## ALPHA 5

### ALPHA 5.3
- Compatible with KSP 0.90

### ALPHA 5.2
- Optimizes alarm code: I suspect this will be an onging challenge

### ALPHA 5.1

- Fixes bug in sound code that caused SEVERE lag

### ALPHA 5

- New Features
  - Sound warnings
    - Each failure has a priority
    - Number of alarm sound repeats configurable thru ingame GUI for each priority
    - All alarms can be muted from the right click menu
  - Intakes failuremodule imported from Entropy (intakes become clogged, stopping providing resources)
  - Coolant failuremodule imported from Entropy (causes engines to generate more heat than they can dissipate, causing you to have to run below 100%)
  - Jet engines have been buffed, should last about 10 hours
  - Added a system for FailureModules to show extra editor info
  - Added a system for FailureModules to hide themselves in editor
  - Moved DangIt! blacklist to a static class
  - Added a KSPAssembly tag
- Fixes
  - Tank module no longer shown for batteries
  - Tank module no longer shown for SRBs

## ALPHA 4

### ALPHA 4.4

- Fixes the negative cost of parts containing Spare Parts.
- Re-adds the 1 spare per click mechanic. Not yet configurable at the moment.
- Resolves compatibility issue with mods that add resources to asteroids.
- Increases the reliability of ion engines to balance them against LFO engines.

### ALPHA 4.3 

- Fixes a file handling exception on OSX.
- Removes the unity compatibility warning .
- Adds CKAN support.
- Improves the SpareParts cfg using SandWorm's edits.

### ALPHA 4.2

- Fixes the cost of parts including SpareParts.
- Now comes bundled with MiniAVC, an *opt-in* update checker.

### ALPHA 4.1

Adds Compatibility Checker (just a warning).
Updates blacklist for the new MKS resources.
Blacklists IntakeAir (how did nobody notice for this long!?),

### ALPHA 4

- PERKS SYSTEM:
	* Each module now has its own set of perks needed to interact with it.
	* A kerbal will need to be sufficiently trained before he can inspect, maintain, or repair a part.
	* Only kerbals that are at the KSC can be trained (at the expense of science and funds): they cannot be trained in flight.
	* The set of perks they spawn with is dependent on their intelligence.
- Complete rebalance pass of all the reliability values.
- Bugfixes:
	* tanks loaded the wrong value for the pole.
	* the last resource in a tank was never selected.
	* the cost of preemptive maintenance wasn't being spent.
- Drops FAR compatibility.

## ALPHA 3

### ALPHA 3.3.1

- Fixes an error in the formula for the chance of failure.

### ALPHA 3.3

- Fixes:
	* the spares container now responds correctly to the distance settings;
 	* changed the default max distance to 2m to allow interaction with parts up to 3.75m;
 	* tank leaks now ignore empty resources.
- Improvements:
	* the temperature now makes parts age even when they are not in use;
 	* the TemperatureMultiplier is now included in the computation of Lambda for all parts;
 	* unmanned command pods don't get spare parts. Supports up to 7 crew members.
 	* the atmospheric density is now used as LambdaMultiplier for control surfaces: this means they are less likely to fail at high altitudes.

### ALPHA 3.2.1

- Hotfix to patch the issue causing multiple instances of the app button to be created.
- Added the download URL to the .version file.

### ALPHA 3.2

- Heavy code changes to have the runtime implemented as a ScenarioModule which also holds settings.
- Now using the stock messaging system for inspections and failures.
- Added an app button that opens a settings window. These settings can be edited in the space centre and are reloaded at the beginning of the flight.
- There is now no difference between debug and release builds.

### ALPHA 3.1

- Partial FAR compatibility.
- Fixed the NRE issue with RAPIER engines.
- Exponential decay of the recovery value of a part.

### ALPHA 3

- Big Stuff:
 	* EVA Inspection: you can now inspect a part while on EVA to have a ballpark estimate of its reliability (currently hideous, will embellish asap);
 	* Inpsection bonus: additionally, performing an inspection will give a temporary reliability boost to the part;
 	* Preemptive maintenance: you can now permanently improve the reliability (by consuming spare parts);
 	* The higher a part's temperature, the faster it will age: be careful with those reentry angles!
- Small stuff:
 	* Now all the reliability infos are in their own panel in the editor;
 	* Added support for engines that use ModuleEngineFX: now all engines should be covered, at least in theory;
- Fixes:
 	* Timewarp now stops when a failure occurs;
 	* Resource leaks can't be stopped by shutting the valves;
 	* The kerbal's name is now displayed correctly during EVA events
 	* A bug that caused leaks to show incorrectly in the flight log
 	* The formatting of the flight log messages.

## ALPHA 2

### ALPHA 2.2

- New features:
	* failures are repairs are listed in the Flight Events log
	* Master settings file: located in DangIt\PluginData\DangIt\DangIt.cfg (don't move it). Contains the leak blacklist and also 3 master switches to enable / disable messages and the red glow (and a placeholder for sounds). This file is reloaded at each flight scene.
	* Silent failures will not display any notification even when the master is enabled. 
- Fixes:
	* the persistency of failures: now they should be loaded correctly between saves.
	* the leak blacklist. Now it actually works.
	* the lights on landing gear now fail correctly.
	* the GUI of reaction wheels is now hidden when the wheel fails.
	* new reliability buff for everything (+100% MTBF: I laughed at 343 for buffing the warthog by 30%)
- Small changes in the API regarding how failures are handled. I guess it doesn't really matter since I'm the only one using the API anyway.

### ALPHA 2.1

- Adds a black list for tank leaks. Now you can prevent a resource from leaking by listing it in the config file:

    	ignore = resourcename
    
- Huge buff for all the reliability values.

### ALPHA 2

- Big stuff:
    * Completely redone how the persistence and update logic is handled: now it should actually do what it is supposed to!*
    * Kerbals now receive a random discount on the cost of repairing a failed part: the lower the stupidity, the higher the possible discount.
    * Introduced silent failures: if a failure is silent, you will get no notification when it happens and the part won't light up.
    * You now have the possibility to initiate the failures manually: this of course won't be in the final version, but this way you can test it and give me feedback.
- Small stuff:
    * Control surfaces now only age in atmosphere.
    * Removed the AgeOnlyWhenActive flag
    * Fixed the bug that caused parts not updating unless they were in the active stage
    * Drastic changes in the cfgs' values
    * various bug-fixes and a lot of logging changes that don't really matter to you

*... probably

## ALPHA 1

### ALPHA 1.3

- The control surfaces now age when the ship is in atmosphere instead of always.
- Fixed the tanks not leaking correctly.

### ALPHA 1.2

- Fixed (?) the lifecycle of the module with proper initialization when the flight starts (and detecting a revert to launch).
- Fixed the part's glow not turning off.

### ALPHA 1

- Initial release: primitive age tracking and some failure modules.
