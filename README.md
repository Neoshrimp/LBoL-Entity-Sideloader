# LBoL-Entity-Sideloader
Entity manager/loader for modding [Lost Branch of Legend](https://store.steampowered.com/app/1140150/Touhou_Lost_Branch_of_Legend/). \
Made using  [Harmony](https://github.com/pardeike/Harmony) and [BepInEx](https://github.com/BepInEx/BepInEx).

## Important! Currently requires latest beta branch to work!
*[Switching to beta branch](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/BetaBranch.md)*

## The Mods
*very tiny modifications, but hopefully there will be more time to focus on content mods after Sideloader reaches desirable state (probably at least [Enemy loading](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/README.md#roadmap))*
- [Good Examples](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/tree/master/src/GoodExamples)
- [Vanilla Tweaks](https://github.com/Neoshrimp/LBoL-Gameplay-mods/tree/master/src/VanillaTweaks)
- [Gun](https://github.com/Neoshrimp/LBoL-Gameplay-mods/tree/master/src/GunToolCard)
- [Help Me Eirin](https://github.com/Neoshrimp/LBoL-Gameplay-mods/tree/master/src/HelpMeEirin) (doesn't need Sideloader)

### Installation

Download [Sideloader.dll](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/LBoL-Entity-Sideloader.dll) and place it in `BepInEx/plugins` folder.

[*Detailed Installation guide*](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/Installation.md)

### For mod creators
This modding framework attempts to streamline, simplify and standardize common game entity (Card, Enemy etc.) creation and loading.

The general idea is that the Sideloader provides abstract [template](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/tree/master/src/LBoL-Entity-Sideloader/Entities) types which should be extended and implemented concretely in your own plugin. Each abstract template method corresponds to one component of the game entity. For example, `CardTemplate` expects `LoadCardImages` to load CardImages, `MakeConfig` to define CardConfig. Additionally, entity logic type should be written and marked with [`EntityLogic`](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/Attributes/EntityLogicAttribute.cs) attribute. In case of a card, entity logic will be extending a `Card` type and defining its behavior in the same manner as a vanilla card would. That is, overriding `Actions` adding custom triggers (reactors/handlers) and so on.

Sideloader has many convenience methods and types designed to reduce clutter and speed up development. Examples include `ResourceLoader`, `CardImages.AutoLoad`, `GlobalLocalization`. It also provides thorough error feedback to help define templates correctly and feature to [reload mods](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/MyFirstCard.md) while the game is running.


#### Modding guides


##### Essential tools

1. [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/)(or any other IDE ofc) with .NET development package.
2. [Sideloader mod template](https://github.com/Neoshrimp/LBoL-ModdingTools/tree/master/src/SideloaderTemplate). Has tons of configuration preset.
3. [dnSpyEx](https://github.com/dnSpyEx/dnSpy). For reading game's code and understanding how to code entity behavior.
4. [scriptengine](https://github.com/Neoshrimp/BepInEx.Debug/blob/master/src/ScriptEngine/ScriptEngine.dll). For hot reload. My fork has a bug fix for LBoL.
5. [DebugMode](https://github.com/Neoshrimp/LBoL-ModdingTools#debugmode). For quickly building decks and fighting enemies.

##### First card
[*tutorial and introduction to Sideloader*](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/MyFirstCard.md)


##### Hot reload

1. In `BepInEx/config/neo.lbol.frameworks.entitySideloader.cfg` change `DevMode = false` to `DevMode = true`.
2. Download and put [scriptengine](https://github.com/Neoshrimp/BepInEx.Debug/blob/master/src/ScriptEngine/ScriptEngine.dll) to `plugins` folder.
3. Create `BepInEx/scripts` folder. This from where scriptengine will load the plugins. Sideloader mods which are being developed should go there. If [Sideloader mod template](https://github.com/Neoshrimp/LBoL-ModdingTools/tree/master/src/SideloaderTemplate) is used it will copy dlls to `scripts` folder automatically after building.
4. In-game, press F3 to reload the mods. If in-run level will need to be restarted for changes to take effects. By default, that will be done automatically. There might some odd issues with Collection.

Sometimes the game freezes and crashes at the moment of reloading plugins. This is probably related to pc resource usage.


##### Examples
- [Changing already existing entities](https://github.com/Neoshrimp/LBoL-Gameplay-mods/blob/master/src/VanillaTweaks/FairyTree.cs)
- [Overcommented examples](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/GoodExamples/CycleAbilities/RedCycleAbility.cs) and [this](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/GoodExamples/Exhibits/FistOfTheThreeFairies.cs)


##### Game Entity reference
[Mini wiki](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/EntityReference.md) detailing game entity and config specifics.

### Roadmap

Suggestions, contributions, issues, bug reports/fixes and critique are all very welcome.


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
  |-- size reduction/compression
  |-- whatever else..
|-- Rewritting and refactoring Sideloader backend..
</pre>

[_Yarn script problem_](https://docs.yarnspinner.dev/using-yarnspinner-with-unity/faq#how-do-i-generate-a-yarn-project-at-runtime-how-do-i-load-compile-yarn-scripts-at-runtime)





