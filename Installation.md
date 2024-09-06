
### Installation
*more relevant for mod developers at this point. Use [r2modman](https://thunderstore.io/c/touhou-lost-branch-of-legend/p/ebkr/r2modman/) if you just want to play modded game.*

1. Download [BepInEx](https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x64_5.4.21.0.zip).
2. Locate LBol install directory. Probably at `C:\Program Files (x86)\Steam\steamapps\common\LBoL\` Otherwise, go to Steam > Library > LBoL > Manage(:gear: icon) > Properties > Local files > Browse
3. Extract BepInEx in LBoL directory (where `LBoL.exe` is).
4. `[Running for the fist time]` Launch LBoL (from Steam) and BepInEx will generate relevant files including `BepInEx\plugins` folder. Close the game.
5. `[Running for the fist time]` Download [Sideloader](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/LBoL-Entity-Sideloader.zip) and extract it in `BepInEx/plugins` folder. Final path should look like `Bepinex/plugins/LBoL-Entity-Sideloader/<sideloader dlls>`. That's imporant for reference path used in [mod project template](https://github.com/Neoshrimp/LBoL-ModdingTools/tree/master/src/SideloaderTemplate).
6. (If developing mods) `[Running for the fist time]` After installing Sideloader run the game once and close it after it loads to main menu. There should .cfg file generated `BepInEx\config\neo.lbol.frameworks.entitySideloader.cfg`. Open it with a text editor and change `DevMode` to `true`.
6. ~~[Download mods](https://github.com/Neoshrimp/LBoL-Entity-Sideloader#the-mods). Place .dll files in `plugins` folder.~~ *Download mods from Thunderstore.*
7. (OPTIONAL) Backup save data. Unnecessary as none of the mods affect it directly or advance meta/story progression for free. Save data location: `%appdata%\..\LocalLow\AliothStudio\LBoL`
8. Enjoy!

### 
[*Detailed BepInEx guide*](https://docs.bepinex.dev/master/articles/user_guide/installation/unity_mono.html) 

### Uninstallation
Delete .dll files from `plugins` folder and optionally plugin configuration files from `config` folder.
