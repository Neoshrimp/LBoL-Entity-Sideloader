## Template for [Sideloader](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/tree/master) mod.

Use is highly encouraged as it has dependencies, publicizer and common boilerplate setup.


Instructions:
- Copy [`LBoL Sideloader Template.zip`](https://github.com/Neoshrimp/LBoL-ModdingTools/raw/master/src/SideloaderTemplate/LBoL%20Sideloader%20Template.zip) to `<User>\Documents\Visual Studio 2022\Templates\ProjectTemplates` do NOT unzip.

- Create a new project, search for LBoL Sideloader template.

![image](https://user-images.githubusercontent.com/89428565/236344254-6eefaa12-c897-4406-867c-1abfa2259f65.png)
- Change _GameFolder_ in .csproj file to target the game installation folder.

![image](https://user-images.githubusercontent.com/89428565/236344281-02c506b5-42bf-4398-a8fc-19a07d727785.png)
- In `PluginInfo.cs` class fill out `GUID` and `Name`. Mod will fail to load without GUID!

![image](https://user-images.githubusercontent.com/89428565/236587701-cbeea462-62ff-4762-a1b0-54175b8a0918.png)


For first time setup:
- Download [Sideloader.dll](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/LBoL-Entity-Sideloader.dll) and put it in `BepInEx/plugins` folder. It will be used as reference in the project.
- Download [scriptengine.dll](https://github.com/Neoshrimp/BepInEx.Debug/blob/master/src/ScriptEngine/ScriptEngine.dll), put in plugins folder. Create `BepInEx/plugins/scripts` directory. Technically, this is optional but workflow without scriptengine is 10 times slower.

Change post-build command to copy to `plugins` folder instead of `scripts` if ScriptEngine is not used.

`https://nuget.bepinex.dev/v3/index.json` might need to be added as a source for nuget manager for BepInEx packages to be installed correctly.

![image](https://user-images.githubusercontent.com/89428565/236344506-aeba2284-a134-418c-aa65-39967290f6cc.png)
