## First Card

Prerequisites: basic C#, basic OOP, resolve to debug errors ~~(and ability to read the bloody error messages)~~.

1. Get [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/).

1. Setup [BepInEx](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/Installation.md) with [Sideloader](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/LBoL-Entity-Sideloader.dll).

1. Setup [scriptengine](https://github.com/Neoshrimp/BepInEx.Debug/blob/master/src/ScriptEngine/ScriptEngine.dll) and create `BepInEx/scripts` folder.

1. Setup [VS project template](https://github.com/Neoshrimp/LBoL-ModdingTools/tree/master/src/SideloaderTemplate) and create new template project. Wait a moment for nuget packages to finish downloading. If GameFolder was set correctly there should be no errors. Else adjust the GameFolder and clean the project.

1. Create a new class, call it FirstCardDef. Make it `public sealed` and extend `CardTemplate`.

1. Class name should be underlined red. Alt+Enter while cursor is on class name, select 'Implement abstract class'.

1. This should have generated a bunch of methods. Disregard them for a moment. Create a new class, for convenience, in the same file `public sealed class FirstCard : Card`. This class is going to define card's behavior.

1. Put `[EntityLogic(typeof(FirstCardDef))]` on top of `FirstCard` class.

1. Now come back to the generated methods.

