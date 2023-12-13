
### Installation
*more relevant for mod developers at this point. Use [r2modman](https://thunderstore.io/c/touhou-lost-branch-of-legend/p/ebkr/r2modman/) if you just want to play modded game.*

1. Download [BepInEx](https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x64_5.4.21.0.zip).
2. Locate LBol install directory. Probably at `C:\Program Files (x86)\Steam\steamapps\common\LBoL\` Otherwise, go to Steam > Library > LBoL > Manage(:gear: icon) > Properties > Local files > Browse
3. Extract BepInEx in LBoL directory (where `LBoL.exe` is).
4. `[Running for the fist time]` Launch LBoL (from Steam) and BepInEx will generate relevant files including `BepInEx\plugins` folder. Close the game.
5. `[Running for the fist time]` Download [Sideloader.dll](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/LBoL-Entity-Sideloader.dll) and place it in `BepInEx/plugins` folder.
6. [Download mods](https://github.com/Neoshrimp/LBoL-Entity-Sideloader#the-mods). Place .dll files in `plugins` folder.
7. (OPTIONAL) Backup save data. Unnecessary as none of the mods affect it directly or advance meta/story progression for free. Save data location: `%appdata%\..\LocalLow\AliothStudio\LBoL`
8. Enjoy!

### 
[*Detailed BepInEx guide*](https://docs.bepinex.dev/master/articles/user_guide/installation/unity_mono.html) 

### Uninstallation
Delete .dll files from `plugins` folder and optionally plugin configuration files from `config` folder.
