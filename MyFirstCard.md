## First Card

Prerequisites: basic C#, basic OOP, resolve to debug errors ~~(and ability to read the bloody error messages)~~.

1. Get [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/).

1. Setup [BepInEx](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/Installation.md) with [Sideloader](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/LBoL-Entity-Sideloader.dll).

1. Setup [scriptengine](https://github.com/Neoshrimp/BepInEx.Debug/blob/master/src/ScriptEngine/ScriptEngine.dll) and create `BepInEx/scripts` folder.

1. Setup [VS project template](https://github.com/Neoshrimp/LBoL-ModdingTools/tree/master/src/SideloaderTemplate) and create new template project. Wait a moment for nuget packages to finish downloading. If GameFolder was set correctly there should be no errors. Else adjust the GameFolder and clean the project.

![image](https://user-images.githubusercontent.com/89428565/236564583-ab780602-6dc5-4eab-86b1-a9e00c8be472.png)


1. Create a new class, call it FirstCardDef. Make it `public sealed` and extend `CardTemplate`.

![image](https://user-images.githubusercontent.com/89428565/236564651-22de18a5-f1a7-498f-a586-f4b9404c29fd.png)

![image](https://user-images.githubusercontent.com/89428565/236564693-8aceffc3-7d76-4d1f-b5ca-897a05214707.png)



1. Class name should be underlined red. Alt+Enter while cursor is on class name, select 'Implement abstract class'.

![image](https://user-images.githubusercontent.com/89428565/236564727-928bc477-ff93-4f7f-be1e-005bca017630.png)

![image](https://user-images.githubusercontent.com/89428565/236564823-899c9ed1-729c-4302-93ae-cabe1fbc810f.png)



1. This should have generated a bunch of methods. Disregard them for a moment. Create a new class, for convenience, in the same file `public sealed class FirstCard : Card`. This class is going to define card's behavior.

![image](https://user-images.githubusercontent.com/89428565/236564867-650faa7f-a875-4300-ba7b-2ca92fa46962.png)


1. Put `[EntityLogic(typeof(FirstCardDef))]` on top of `FirstCard` class.

![image](https://user-images.githubusercontent.com/89428565/236564921-e8ff78d5-57c0-45d3-9034-e5833e5ecb46.png)

1. Now come back to the generated methods.

