## First Card

Prerequisites: basic C#, basic Object Oriented Programming, resolve to debug errors ~~(and ability to read the bloody error messages)~~.

- [One time] Get [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/).
    - Open Visual Studio Installer and make sure you have .NET desktop development packages
      
      ![image](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/assets/89428565/1463d658-95c6-47fa-9d94-53915456ed3b)

    - You don't need all of them, these should be enough
      
      ![image](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/assets/89428565/1b6417b3-e605-426a-8d55-0502c093662e)


- [One time] Setup [BepInEx](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/Installation.md) with [Sideloader](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/LBoL-Entity-Sideloader-thunderstore.zip).

- [One time] Setup [scriptengine](https://github.com/Neoshrimp/BepInEx.Debug/blob/master/src/ScriptEngine/ScriptEngine.dll) and create `BepInEx/scripts` folder.

- [One time] Get [DebugMode](https://github.com/Neoshrimp/LBoL-ModdingTools#debugmode)

- [One time] Get MODDED [Watermark](https://github.com/Neoshrimp/LBoL-ModdingTools/blob/master/src/LBoL-AddWatermark/LBoL-AddWatermark.dll)

- [One time] Setup [VS project template](https://github.com/Neoshrimp/LBoL-ModdingTools/tree/master/src/SideloaderTemplate).

- Create new project using Sidleoader template. Wait a moment for nuget packages to finish downloading. If GameFolder was set correctly there should be no errors. Else adjust the GameFolder and clean the project.

![image](https://user-images.githubusercontent.com/89428565/236564583-ab780602-6dc5-4eab-86b1-a9e00c8be472.png)

- Create a new class, call it FirstCardDef. Make it `public sealed` and extend `CardTemplate`.

![image](https://user-images.githubusercontent.com/89428565/236564651-22de18a5-f1a7-498f-a586-f4b9404c29fd.png)

![image](https://user-images.githubusercontent.com/89428565/236564693-8aceffc3-7d76-4d1f-b5ca-897a05214707.png)



- Class name should be underlined red. Alt+Enter while cursor is on class name, select 'Implement abstract class'.

![image](https://user-images.githubusercontent.com/89428565/236564727-928bc477-ff93-4f7f-be1e-005bca017630.png)

![image](https://user-images.githubusercontent.com/89428565/236564823-899c9ed1-729c-4302-93ae-cabe1fbc810f.png)



- This should have generated a bunch of methods. These methods are like slots to be filled up with components which define a card. Disregard them for the moment. Create a new class, for convenience, placed in the same file: `public sealed class FirstCard : Card`. This class is going to define card's behavior.

![image](https://user-images.githubusercontent.com/89428565/236564867-650faa7f-a875-4300-ba7b-2ca92fa46962.png)


- Put `[EntityLogic(typeof(FirstCardDef))]` on top of `FirstCard` class.

![image](https://user-images.githubusercontent.com/89428565/236564921-e8ff78d5-57c0-45d3-9034-e5833e5ecb46.png)

- Now come back to the generated methods. Make `GetId()` return name of the entity logic class `return nameof(FirstCard)` (string). For now it's a strict requirement that **Id is the same as the logic type name**.

![image](https://user-images.githubusercontent.com/89428565/236586953-58ee8e80-160f-403c-9615-e69036e465fb.png)

- Crop image to 743x512 , call it same as the Id, `FirstCard.png`, paste it into the `Resources` folder. Any files or folder in the `Resources` will be automatically embedded in the compiled dll. It's the most convenient way to include files, for now.

![image](https://user-images.githubusercontent.com/89428565/236586980-ebb19bef-e87f-4f88-be2d-b73b7154616b.png)

- In `LoadCardImages` method create a new instance of `CardImages` specifying the embedded source. It acts as container to hold any images a card might need and eventually will be passed to Sideloader.

- Call `imgs.AutoLoad` method and specify ".png" extension. AutoLoad is possible because the card image is named the same as the Id. Return images. Alternatively, `return null` can be used skip loading images for now and leave the card without an image.

```csharp
public override CardImages LoadCardImages()
{
    var imgs = new CardImages(BepinexPlugin.embeddedSource);
    imgs.AutoLoad(this, extension: ".png");
    return imgs;
}
```


- In `Resources` folder create `CardsEn.yaml` file.

![image](https://user-images.githubusercontent.com/89428565/236587175-d7fd9356-7212-433a-be8a-3d4186dcafa1.png)

- `LoadLocalization` can return either `LocalizationFiles` or `GlobalLocalization`. `GlobalLocalization` is just a better choice. When setting up global localization only one CardTemplate should specify the localization files with `loc.LocalizationFiles.AddLocaleFile` method. Other cards should just `return new GlobalLocalization(BepinexPlugin.embeddedSource);` to indicate that they use global localization.


```csharp
public override LocalizationOption LoadLocalization()
{
    var loc = new GlobalLocalization(BepinexPlugin.embeddedSource);
    loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "CardsEn.yaml");
    return loc;
}
```

- Open `CardsEn.yaml`. In the end it, should look very similar to game's localization files, found at `<LBoL_install_folder>\LBoL_Data\StreamingAssets\Localization\en`. Make sure yaml is formatted legally.

```
CardId:
  Name: SomeName
  Description: "Description"
```

- For now make `MakeConfig()` `return DefaultConfig();`. You can worry about specifics in a bit.

- If everything was done correctly, you should have a skeleton of the card. Build the project, the dll should be automatically copied to `BepInEx/scripts` folder, run the game.

![image](https://user-images.githubusercontent.com/89428565/236587200-678e5c16-d8c5-430b-b7d0-70b80c374b5e.png)


- If BepInEx has [Logging.Console] enabled in  `BepInEx\config\BepInEx.cfg` you should see various messages (or errors) related to sideloader.

- In game, go to collection, scroll to the very bottom without filtering and there should be your card, sitting, not doing much.

![image](https://user-images.githubusercontent.com/89428565/236587228-255beb99-b807-4b56-ad26-d0e7e1d11c55.png)

- Now comes the difficult part, actually implementing the card. Let's start small and say we want to create a stronger version of Youkai Buster.

- Go back to `FirstCardDef.MakeConfig`. You don't have to close the game just close the collection.

- Unfortunately, for now, the fastest way to fill out config is to copy code. Copy  CardConfig constructor with named parameters from [here](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/ae2ef3c77ef28da9035603b182640c711dd55d06/src/LBoL-Entity-Sideloader/Entities/CardTemplate.cs#L47).

- The more detailed explanation of each config property can be found in [Entity Reference](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/EntityReference.md). For now change `Index`,  `GunName`, `GunNameBurst`, `Type`, `IsPooled`, `TargetType`, `Colors`, `Cost` and `Damage`. `sequenceTable.Next` is convenient way to keep track of indexes but any positive number can be assigned as index as well.
```
Index: BepinexPlugin.sequenceTable.Next(typeof(CardConfig)),
...
GunName: "Simple1",
GunNameBurst: "Simple2",
...
IsPooled: true,
...
Rarity: Rarity.Common,
Type: CardType.Attack,
TargetType: TargetType.SingleEnemy,
Colors: new List<ManaColor>() { ManaColor.Red, ManaColor.White },
...
Cost: new ManaGroup() { Red = 1, White = 1 },
...
Damage: 7,
...

```

- Compile the project again and go back to the game. Close Collection, press F3 to refresh the mods. The card should be an `Attack` and have cost and colors.

![image](https://user-images.githubusercontent.com/89428565/236587270-0d66f1a2-e384-45d3-99c5-f69cfc63a06b.png)

- [One time] Create 'Debug' profile in-game for testing stuff. 

- [One time] Download [dnSpyEx](https://github.com/dnSpyEx/dnSpy).

- Open these LBoL assemblies in dnSpy. `LBoL.EntityLib.dll` is the one where the game entities are implemented.

![image](https://user-images.githubusercontent.com/89428565/236587299-d8ee5fae-c5e5-4e79-8906-9c7ff7a45068.png)

- Find the Id of Youkai Buster. To do that either search Card.yaml localization files or use DebugMode menu option `Output All Config` and check `CardConfig.txt`. Id of Youkai Buster is `YaoguaiBuster`.

- In dnSpy search for a Type named `YaoguaiBuster`. Make sure Files in Same Folder option is selected.

![image](https://user-images.githubusercontent.com/89428565/236587341-a0100779-5c72-4023-8129-39a160fa46ff.png)

- Investigating `YaoguaiBuster` type we can see that it extends `Card`. It seems that `SetGuns` method can be used to increase number of hits performed. Let's copy the method and paste it in our own `FirstCard` class.

- Let's increase number of hits performed to 3/4 by adding an item to each array.

```csharp
[EntityLogic(typeof(FirstCardDef))]
public sealed class FirstCard : Card
{
    protected override void SetGuns()
    {
        if (this.IsUpgraded)
        {
            CardGuns = new Guns(new string[]
            {
            Config.GunNameBurst,
            Config.GunName,
            Config.GunNameBurst,
            Config.GunName,
            });
            return;
        }
        CardGuns = new Guns(new string[]
        {
            Config.GunName,
            Config.GunNameBurst,
            Config.GunName,

        });
    }
}
```

- Time to test it! Compile the project, reload mods and start a debug run with F5 (while in main menu). After adding the cards to the deck, giving yourself some shining exhibits you can click one of these buttons to quickly advance the map node. This will save the game and you won't have to redo your setup when restarting the level.

![image](https://user-images.githubusercontent.com/89428565/236587365-38b4d057-2f2d-4b13-a95f-f1e9e4c6a158.png)

- Both upgraded and unupgraded card versions seems to function as intended and console is clean of errors. The only thing left to do is to finish writing the description. I've chosen to differentiate upgraded card description by using 'UpgradedDescription:' node.

## [[First Card code]](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/tree/master/src/FirstCard)
