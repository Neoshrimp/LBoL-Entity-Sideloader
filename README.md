# LBoL-Entity-Sideloader
Entity manager/loader for modding [Lost Branch of Legend](https://store.steampowered.com/app/1140150/Touhou_Lost_Branch_of_Legend/)




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




[_Yarn script problem_]((https://docs.yarnspinner.dev/using-yarnspinner-with-unity/faq#how-do-i-generate-a-yarn-project-at-runtime-how-do-i-load-compile-yarn-scripts-at-runtime))

### Game Entity reference

#### Card

