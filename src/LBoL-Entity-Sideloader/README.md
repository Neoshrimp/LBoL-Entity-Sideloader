Modding framework for working with LBoL entities.

#### Thanks to the contributors!

[IntoxicatedKid](https://github.com/IntoxicatedKid),
[cyaneko](https://github.com/cyaneko)

#### Change log
`0.9.7842` Fix some interactions between cost reductions and in-battle upgrades. Mostly affects 1XX -> 2X style upgrades.

`0.9.7840` Hard deprecate `LBoLEntitySideloader.ExtraFunc.AutoCastAction`.

`0.9.7830` Small fix for 1.7.2.

`0.9.7820` Make Sideloader compatible with both beta and main branches of the game. Dual support will not be a thing in the future but, in this case, the issue was very minor and technical. Other mods are also likely to function properly on either branch.

`0.9.7810` Small fix for 1.7.1.

`0.9.7800` Update for LBoL version 1.7.0. **Not backwards compatible**. [Mod breaking changelist](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/1.7.0_update_change_checklist.md) (non-exhaustive yet).

`0.9.7710` Fix embedded resource file name matching bug.

`0.9.7700` Update for LBoL 1.6.1 beta. **Not compatible with previous versions.**

`0.9.7602` Add `StartGameUI` patches to unlock all difficulties, requests and jadeboxes. *thanks, cyaneko*

`0.9.7601` Fix `UnitModelTemplate.CheckModelOptions` null check.

`0.9.7600` Slight but breaking change in keyword API. `descPos` argument removed from `CardKeyword` constructor. Now it's meant to be supplied via object initializer. Reason for the change is the fact that mono or mono.cecil behaves weirdly with enum constants/default arguments when the enum definition is from another loaded plugin (Sideloader).

`0.9.7500` Add API for custom keywords. [Docs](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/tree/master/src/LBoL-Entity-Sideloader/CustomKeywords) in the code. [Examples](https://github.com/Neoshrimp/LBoL_Doremy/blob/DLrework/LBoL_Doremy/DoremyChar/Keywords/DoremyKw.cs) in Doremy codebase.

`0.9.7405` Add fix to precalculated block/shield rounding.

`0.9.7402` Koishi random retargeting fix.

`0.9.7401` Update for LBoL 1.6.0 beta. **Not compatible with previous versions.**

`0.9.7400` Fix custom handler hash not being computed correctly.

`0.9.7300` Auto sort card colours in WUBRGC order.

`0.9.7100` Fix shop softlock when there are not enough cards in the pool (somehow).

`0.9.7000` Add enemy intention template `IntentionTemplate`. 
Add method for adding custom Eiki summon `PlayerUnitTemplate.AssociateEikiSummon`.
Fix *Fantasy Seal -Worn-* not dealing increased damage after upgrade.

`0.9.6500` Add support for multiple SE icons via optional `LoadExtraIcons` override in `StatusEffectTemplate`.

`0.9.6440` Fix custom save data from disk loaded assemblies not registering when hotreloading.

`0.9.6430` Patch some UI crap.

`0.9.6420` Allow gain Empty Exhibit Box from boss. Thanks, int.

`0.9.6413` Fix ModifyStageList bug when adding multiples of the same stage.

`0.9.6410` Add ITypeConverter option for custom persistent data.

`0.9.6400` Move BattleModifiers, a system for modifying units via lambdas and delegates, from FullElite.

`0.9.6300` Add custom handlers for gameRun and Battle.

`0.9.6200` Add animations for `AddCardsToExileAction`.

`0.9.6100` New feature. Added template class for storing game run load persistent values.

`0.9.6010` Fix Seija damage cap when dealing over 300 damage.

`0.9.6000` Update for LBoL v1.42.

`0.9.5000` Add `BatchLocalization` support. Deprecate `GlobalLocalization`.

`0.9.4500` Fix localization files error.

`0.9.4400` Prevent unlosable exhibits from being swap in debut. Thanks Intoxicated.

`0.9.4300` Fix locale change bug. Add `GlobalLocalization.DiscoverAndLoadLocFiles`.

`0.9.4210` Small ult skill title localization fix.

`0.9.4200` Add small character portrait to top-left of main menu if quit mid-run. (thanks lvalon)

`0.9.4100` Adjust sprite loading, make large tooltips fit better.

`0.9.4000` Change how auto indexing works, stage bgm patch.

`0.9.3000` Fixed version number?

`0.9.3` Update for LBoL 1.4+. Not compatible with previous game version.

`0.9.231` Added Watermark dependency.

`0.9.23` Made hard/soft reload to always hard reload. Sort of a workaround for reloading bugs.

`0.9.22` Add EnemyUnitTemplate.AddBossNodeIcon

`0.9.21` Rename PlayerUnitTemplate.AddExtraLoadout to AddLoadout

`0.9.2` Add custom stage gameObjects.

`0.9.13` Add spell name and title text to bomb splash. Add `SpellTemplate` (practically useless).

`0.9.12` Add `AutoCastAction` in `LBoLEntitySideloader.ExtraFunc.CardHelper`

`0.9.11` Remove error message suppression in ViewConsumeMana. An explicit fix is required to make auto cast work more properly.

`0.9.1` Improve Texture2D scaling quality by adding mipmap generation.

`0.9.0` added template for new playable characters and helpers methods. 

Added methods for manipulating Stage pools and Stage list itself (in `StageTemplate`). 

Routed async sprite loading through ImageLoader library.  

Added cropping templates for various images.

ResourceLoader can load AssetBundle s now.

`0.8.4` better logging. Set ExtraLogging to false to avoid logging individual entities. Added documentation xml.

`0.8.3` enable exhibit dupes (thanks to IntoxicatedKid).

`0.8.1` added UnitModelTemplate. Custom enemy or players models are now possible.

`0.8.0` added custom starting loadouts in `PlayerUnitTemplate`

`0.7.9` add EnemyGroupTemplate and custom enemy formations.

`0.7.81` fixed MiniSelectCard Ui issue.

`0.7.8` added Sfx and uiSounds templates. Actually works nicely with template gen.

`0.7.7` added UltimateSkillTemplate.

`0.7.6` added template for EnemyUnit which governs enemy behavior. For now it only makes sense to use it with `[OverwriteVanilla]`

`0.7.5` added template for Jade Boxes.

`0.7.3` added dynamic template generation. Refer to [this](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/TermplateGenTests/Generation.cs) for an example.


-------------------------------------

More info at [github repo](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/tree/master)
