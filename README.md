# LBoL-Entity-Sideloader
Entity manager/loader for modding [Lost Branch of Legend](https://store.steampowered.com/app/1140150/Touhou_Lost_Branch_of_Legend/). \
Made using  [Harmony](https://github.com/pardeike/Harmony) and [BepInEx](https://github.com/BepInEx/BepInEx).

## Important! Currently requires latest beta branch to work!
*[Switching to beta branch](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/BetaBranch.md)*

## The Mods
*very tiny modifications, but hopefully there will be more time to focus on content mods after Sideloader reaches desirable state (probably at least [Enemy loading](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/edit/master/README.md#roadmap))*
- [Good Examples](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/tree/master/src/GoodExamples)
- [Vanilla Tweaks](https://github.com/Neoshrimp/LBoL-Gameplay-mods/tree/master/src/VanillaTweaks)
- [Gun](https://github.com/Neoshrimp/LBoL-Gameplay-mods/tree/master/src/GunToolCard)

### Installation

Download [Sideloader.dll](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/LBoL-Entity-Sideloader.dll) and put in `BepInEx/plugins` folder.

[*Detailed Installation guide*]()

### For mod creators
This modding framework attempts to streamline, simplify and standardize common game entity (Card, Enemy etc.) creation and loading.




### Roadmap


<pre>
*Game Entity loading*
|-- Create Cards ☑️
  |-- Create custom Effects
    |-- Workflow/templates for working with Effect prefabs in Unity
  |-- Create custom Guns (attack vfx)
    |-- PieceConfig (Gun particle of sorts?)
    |-- BulletConfig
    |-- LaserConfig
    |-- whatever else..
|-- Create StatusEffects (Abilities) ☑️
|-- Create Exhibits ☑️
  |-- Display modded Exhibits in Collection
|-- Load custom SFX
  |-- SfxConfig
  |-- BgmConfig
|-- Create Enemies
  |-- Load custom spine models
    |-- UnitModelConfig
  |-- Create Enemy groups (Battles)
    |-- Add modded battles to stage encounter pools
|-- SpellCards
  |-- Custom starting decks (if possible)
    |-- Custom deck selection UI
  |-- Inherent character mechanics (maybe)
|-- *Playable characters at this point?*
|-- Create GapOption
|-- Load Yarn Spinner scripts (devs help!)
  |-- Create Adventures (Encounters)
    |-- Modded Adventure pooling system
|-- Create JadeBox challenges

*Sideloader features and maintenance*
|-- Better docs/tutorials/wikis..
|-- Sideloader usage API improvements and polish
|-- Support Localization ☑️
|-- Dynamic entity reload for development ☑️
|-- Overwrite or modify vanilla entities ☑️
  |-- Overwrite individual entity components selectively ☑️
    |-- Merge configs overwritten by different mods (maybe)
|-- Error feedback and handling ☑️
  |-- Better error feedback and handling..
|-- Support loading from AssetBundles
|-- main/beta double branch support (maybe)
|-- Handle duplicate entity Ids
|-- Performance and profiling
  |-- async loading
  |-- Addressables
  |-- whatever else..
|-- Rewritting and refactoring Sideloader backend..
</pre>

[_Yarn script problem_](https://docs.yarnspinner.dev/using-yarnspinner-with-unity/faq#how-do-i-generate-a-yarn-project-at-runtime-how-do-i-load-compile-yarn-scripts-at-runtime)



### [Game Entities reference](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/EntityReference.md)
Mini wiki detailing game entity and config specifics.



