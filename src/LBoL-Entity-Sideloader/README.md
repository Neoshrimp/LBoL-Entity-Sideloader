Modding framework for working with LBoL entities.

#### Thanks to the contributors!

[IntoxicatedKid](https://github.com/IntoxicatedKid)

#### Change log

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
