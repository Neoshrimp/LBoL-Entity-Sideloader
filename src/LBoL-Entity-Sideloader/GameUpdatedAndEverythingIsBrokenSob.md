### Game updated and everything is broken ToT
*Approximate guide for modders how to update mod projects after major game update*


1. Update [Sideloader](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/LBoL-Entity-Sideloader.zip)
1. Clean, unload and reload project in VS. This will force publizer to publicize updated game assemblies, reflecting game update in the mod project.

1. Try building. There will be errors. Most of them are due to devs renaming, moving or removing namespaces, classes or members. The usual suspects:
    
    - 
    -
1. Entity Configs also likely have changed. Devs are known to change data stored by configs. Unfortunately, due to lack of unified system from Sideloader each config constructor will need to be updated individually. 

In game version `1.4` these configs were changed:

- `StatusEffectConfig` got `Index` field. Save to leave `0`.
- `CardConfig` got `ImageId` and `UpgradeImageId` fields. If left `""` card Id will be used to find card image, the same way it used to work before `1.4`.
- `GunConfig` got `ForceHitAnimationSpeed` field. Save to leave at `0f` maybe.


5. After building the project, run the game. There still might be errors. Frequently, they will be related to Harmony patches or missing methods/members. Double checking Harmony patches after an update is not a terrible idea anyway.


In reasonably best case, scenario this should be it.
