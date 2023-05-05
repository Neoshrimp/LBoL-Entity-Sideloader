## First Card

Prerequisites: basic C#, basic Object Oriented Programming, resolve to debug errors ~~(and ability to read the bloody error messages)~~.

1. [One time] Get [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/).

1. [One time] Setup [BepInEx](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/Installation.md) with [Sideloader](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/LBoL-Entity-Sideloader.dll).

1. [One time]Setup [scriptengine](https://github.com/Neoshrimp/BepInEx.Debug/blob/master/src/ScriptEngine/ScriptEngine.dll) and create `BepInEx/scripts` folder.

1. [One time] Setup [VS project template](https://github.com/Neoshrimp/LBoL-ModdingTools/tree/master/src/SideloaderTemplate).

1. Create new template project. Wait a moment for nuget packages to finish downloading. If GameFolder was set correctly there should be no errors. Else adjust the GameFolder and clean the project.

1. Create a new class, call it FirstCardDef. Make it `public sealed` and extend `CardTemplate`.

1. Class name should be underlined red. Alt+Enter while cursor is on class name, select 'Implement abstract class'.

1. This should have generated a bunch of methods. These methods are like slots to be filled in with components necessary for a card. Disregard them for a moment. Create a new class, for convenience, in the same file `public sealed class FirstCard : Card`. This class is going to define card's behavior.

1. Put `[EntityLogic(typeof(FirstCardDef))]` on top of `FirstCard` class.

1. Now come back to the generated methods. Make `GetId()` return name of the entity logic class `return nameof(FirstCard)` (string). For now it's a strict requirement that Id is the same as the logic type name.

1. Crop image to 743x512 , call it same as the Id, `FirstCard.png`, paste it into the `Resources` folder. Any files or folder in the `Resources` will be automatically embedded in the compiled dll. It's the most convenient way to include files, for now.

1. In `LoadCardImages` method create a new instance of `CardImages` specifying the embedded source. It acts as container to hold any images a card might need and eventually be passed to Sideloader.

1. Call `imgs.AutoLoad` method and specify ".png" extension. Return images. Alternatively, `return null` can be used skip loading images for now and leave the card without image.

1. 

