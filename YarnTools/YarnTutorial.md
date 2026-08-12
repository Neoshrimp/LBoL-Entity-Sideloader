# Creating Custom Events (Adventures)

Events (also known as adventures in the source code) are a bit of a complex web of C# classes, YAMLs, and Yarn scripts. Hopefully this should be an understandable guide to make it easy. The examples here are done in my silly project "TrukeGuns", so obviously, replace that with whatever your own project name is.

---

### Step 1: Install YarnTools & Update `.csproj` (One-time setup)

1. Download **[YarnTools.zip](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/YarnTools/YarnTools.zip)** from the Sideloader repo.
2. Unzip it into a `YarnTools` folder located at the top level of your mod project (right next to your `Resources` folder and `.csproj`).

Next, open your mod's `.csproj` file, scroll down to the bottom (right above your `<Target Name="PostBuild"...`), and paste this snippet:

```xml
<!-- Configurable Yarn Folders -->
<PropertyGroup>
    <YarnSourceFolder Condition="'$(YarnSourceFolder)' == ''">Resources\yarn</YarnSourceFolder>
    <YarnOutputFolder Condition="'$(YarnOutputFolder)' == ''">Resources\yarnc</YarnOutputFolder>
</PropertyGroup>

<!-- Identify .yarn source files -->
<ItemGroup>
    <YarnFiles Include="$(YarnSourceFolder)\*.yarn" />
</ItemGroup>

<!-- Run Yarn compilation ONLY if YarnTools/ysc.exe exists AND there are .yarn files -->
<Target Name="CompileYarn" BeforeTargets="BeforeBuild" Condition="Exists('$(ProjectDir)YarnTools\ysc.exe') And '@(YarnFiles)' != ''">
    <Message Importance="high" Text="[YarnCompiler] Compiling .yarn files from '$(YarnSourceFolder)' to '$(YarnOutputFolder)'..." />

    <MakeDir Directories="$(ProjectDir)$(YarnOutputFolder)" Condition="!Exists('$(ProjectDir)$(YarnOutputFolder)')" />

    <Exec Command="&quot;$(ProjectDir)YarnTools\ysc.exe&quot; compile &quot;%(YarnFiles.Identity)&quot; -o &quot;$(ProjectDir)$(YarnOutputFolder)&quot; -n &quot;%(YarnFiles.Filename).yarnc&quot;" />
</Target>

<!-- Embed all files in Resources/ (including Resources/yarnc/*.yarnc) -->
<ItemGroup>
    <EmbeddedResource Include="Resources\**" />
</ItemGroup>
```

> **Note:** The `<EmbeddedResource Include="Resources\**" />` line should probably already be in your `.csproj`. You can change the source and output folder properties to whatever paths you want.

---

### Step 2: Set Up Batch Localization (One-time setup)

In your localization class, add a `BatchLocalization` for adventures. Optionally, if you plan to introduce characters that don't already exist in the base game, add an enemy unit localization batch as well. Make sure it has `IsUnitNameSource = true` enabled.

```csharp
public sealed class TrukeGunLocalization
{
    public static string Adventure = "Adventure";
    public static string EnemiesUnit = "EnemyUnit";

    public static BatchLocalization AdventuresBatchLoc = new BatchLocalization(BepinexPlugin.directorySource, typeof(AdventureTemplate), Adventure);
    public static BatchLocalization EnemiesUnitBatchLoc = new BatchLocalization(BepinexPlugin.directorySource, typeof(EnemyUnitTemplate), EnemiesUnit)
    {
        IsUnitNameSource = true
    };

    public static void Init()
    {
        AdventuresBatchLoc.DiscoverAndLoadLocFiles(Adventure);
        EnemiesUnitBatchLoc.DiscoverAndLoadLocFiles(EnemiesUnit);
    }
}
```

---

### Step 3: Create the Adventure C# Classes

You need a C# file for the event containing both a `Def` class (inheriting from Sideloader's `AdventureTemplate`) and the entity class inheriting from LBoL's base `Adventure`.

```csharp
namespace TrukeGun.Source.Adventures
{
    public sealed class YuukaGardenDef : AdventureTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(YuukaGarden);
        }

        public override LocalizationOption LoadLocalization()
        {
            return TrukeGunLocalization.AdventuresBatchLoc.AddEntity(this);
        }

        public override AdventureImages LoadAdventureImages()
        {
            var imgs = new AdventureImages();
            // Automatically loads YuukaGarden.png from directorySource (or embeddedSource)
            imgs.AutoLoad(this, BepinexPlugin.directorySource, extraSuffixes: "_angry");
            return imgs;
        }

        public override YarnData LoadYarnData()
        {
            var yarnData = new YarnData();
            // Auto loads YuukaGarden.yarnc from embeddedSource 
            // and YuukaGardenEn.yaml from directorySource
            yarnData.AutoLoad(this, BepinexPlugin.embeddedSource, BepinexPlugin.directorySource);
            return yarnData;
        }

        public override AdventureConfig MakeConfig()
        {
            var config = DefaultConfig();

            config.Id = GetId();
            config.HostId = "Yuuka";
            return config;
        }
    }

    [EntityLogic(typeof(YuukaGardenDef))]
    public sealed class YuukaGarden : Adventure
    {
    }
}
```

#### Code Breakdown:
* **`GetId()`**: ID of the event. It determines what your asset files must be named. For example, `YuukaGarden` requires `YuukaGarden.png`, `YuukaGarden.yarn`, `YuukaGardenEn.yaml`, etc. You can also use a helper method like `DefaultID`:
  ```csharp
  public static string DefaultID(EntityDefinition entity)
  {
      string IDdef = entity.GetType().Name;
      return IDdef.Remove(IDdef.Length - 3); // Removes "Def"
  }
  ```
* **`LoadLocalization()`**: Registers the event ID into the `Adventure` batch localization.
* **`LoadAdventureImages()`**: Chooses where to load event art from. Use `BepinexPlugin.directorySource` for loose files on disk, or `BepinexPlugin.embeddedSource` for embedded DLL resources. You must place an image with the same name as your event (e.g., `YuukaGarden.png`).
  * **`extraSuffixes`**: Lets you register additional images for the event (e.g. `_angry` lets you load `YuukaGarden_angry.png` in-game).
* **`LoadYarnData()`**: Loads the compiled `.yarnc` binary and the event dialogue YAML. The first argument specifies the `.yarnc` source, and the second specifies the dialogue YAML source. It's recommended to keep `.yarnc` embedded and dialogue YAMLs loose in `DirResources`.
* **`AdventureConfig`**: Configures the event. Setting `Id` and `HostId` is generally enough. `HostId` determines which character unit speaks in your event and must match an existing enemy unit ID.

---

### Step 4: Add Event Metadata (`AdventureEn.yaml`)

Add an entry for your event inside your `AdventureEn.yaml` file (and any other locale file like `ja` or `zhs` if you support them):

```yaml
YuukaGarden:
  Title: Flowers of Mugenkan
  HostName: Yuuka
```

---

### Step 5: (Optional) Create a Lore Enemy Host

If your host character doesn't already exist in the base game (check the base game's `UnitName` and `EnemyUnits` yamls first), you must create a "lore enemy". It's best to set up a reusable template for lore enemies:

```csharp
public class LoreEnemyUnitTemplate : EnemyUnitTemplate
{
    public override IdContainer GetId()
    {
        return TrukeGunDefaultConfig.DefaultID(this);
    }

    public override EnemyUnitConfig MakeConfig()
    {
        return GetEnemyUnitDefaultConfig();
    }

    public override LocalizationOption LoadLocalization()
    {
        return TrukeGunLocalization.EnemiesUnitBatchLoc.AddEntity(this);
    }

    public override Type TemplateType()
    {
        return typeof(EnemyUnitTemplate);
    }

    // Config stats don't matter much as long as OnlyLore is true
    public EnemyUnitConfig GetEnemyUnitDefaultConfig()
    {
        return new EnemyUnitConfig(
            Id: "",
            RealName: true,
            OnlyLore: true,
            BaseManaColor: new ManaColor[] {},
            Order: 10,
            ModleName: "",
            NarrativeColor: null,
            Type: EnemyType.Normal,
            IsPreludeOpponent: false,
            HpLength: null,
            MaxHpAdd: null,
            MaxHp: 20,
            Damage1: null,
            Damage2: null,
            Damage3: null,
            Damage4: null,
            Power: null,
            Defend: null,
            Count1: null,
            Count2: null,
            MaxHpHard: null,
            Damage1Hard: null,
            Damage2Hard: null,
            Damage3Hard: null,
            Damage4Hard: null,
            PowerHard: null,
            DefendHard: null,
            Count1Hard: null,
            Count2Hard: null,
            MaxHpLunatic: null,
            Damage1Lunatic: null,
            Damage2Lunatic: null,
            Damage3Lunatic: null,
            Damage4Lunatic: null,
            PowerLunatic: null,
            DefendLunatic: null,
            Count1Lunatic: null,
            Count2Lunatic: null,
            PowerLoot: new MinMax(0, 0),
            BluePointLoot: new MinMax(0, 0),
            Gun1: new List<string> { "Simple1" },
            Gun2: new List<string> { "Simple1" },
            Gun3: new List<string> { "Simple1" },
            Gun4: new List<string> { "Simple1" }
        );
    }
}
```

Now define the enemy unit class matching your `HostId`:

```csharp
public sealed class YuukaDef : LoreEnemyUnitTemplate
{
}

[EntityLogic(typeof(YuukaDef))]
public sealed class Yuuka : EnemyUnit
{
}
```

And localize the host unit in `EnemyUnitEn.yaml`:

```yaml
Yuuka:
  Name: 'Yuuka Kazami'
  Title: 'Nice Flower Girl'
  Default: Yuuka Kazami
  Short: Yuuka
```

> **Note:** `Name` here might not be actively used by events (`Default`, `Short`, and `Title` are), but it's good to include just in case. If your character already exists in the base game, you can skip this step and use their vanilla ID.

---

### Step 6: Write the Yarn Dialogue Script (`YuukaGarden.yarn`)

Create a `.yarn` file and place it inside `Resources/yarn/YuukaGarden.yarn`. 

To write Yarn scripts, you can reference decompiled base game Yarn files (ask in the `mod-dev` channel if you need them). Here is an example Yarn script offering 2 choices:
1. Run away: take damage and gain a specific card.
2. Apologize: lose power and gain a random exhibit.

```yarn
title: Main
---
<<declare $hpLoss = 0>>
<<declare $powerLoss = 0>>
<<declare $cardReward = "">>
<<declare $exhibitReward = "">>
<<setAdventureImage "">>
<<enemyTitle Yuuka>>
<<lEnemyName Yuuka>>
Yuuka: Oh, who's there in my garden? Are you enjoying yourself with the beauty of these flowers? #line:yuuka_001
<<lEnemyName Yuuka>>
Yuuka: Surely you did not stomp on any of them, right? #line:yuuka_002
<<optionCard 1 {$cardReward}>>
<<optionRandomExhibit 2 {$exhibitReward}>>
-> Run. Lose {$hpLoss} HP, gain |{getCardName($cardReward)}|. #line:opt_run
    <<setAdventureImage "_angry">>
    (You get hit by some danmaku while running away.) #line:yuuka_003
    <<damage {$hpLoss}>>
    <<gainCards {$cardReward}>>
-> Apologize. Lose {$powerLoss} <sprite="Point" name="Power">, gain a random Exhibit. #line:opt_apologize
    <<lEnemyName Yuuka>>
    Yuuka: Good. I'm glad some people here are still respectful. #line:yuuka_004
    <<losePower {$powerLoss}>>
    <<gainExhibit {$exhibitReward}>>
<<stop>>
===
```

#### Important Yarn Writing Rules:
* **Declare Variables:** Declare your variables at the top of the Yarn script (`<<declare $var = ...>>`) and set their values in C# (see Step 7).
* **Speaker Commands:** `<<lEnemyName Yuuka>>` and `<<lPlayerName>>` placed above every dialogue line to set who is talking (otherwise, it will be a narrator).
* **No Blank Lines:** Make sure there are **no empty lines** in the dialogue flow, as blank lines will break Yarn's line sequencing.
* **Line IDs:** The `#line:tag` at the end of each text line connects the dialogue line to its translation in your YAML file.

---

### Step 7: Inject Variables in C#

Back in your adventure logic class (`YuukaGarden`), override `InitVariables` to populate the Yarn variables:

```csharp
[EntityLogic(typeof(YuukaGardenDef))]
public sealed class YuukaGarden : Adventure
{
    protected override void InitVariables(IVariableStorage storage)
    {
        // Pre-pick the card reward for option 1
        Card cardReward = LBoL.Core.Library.CreateCard<SummerFlower>();
        storage.SetValue("$cardReward", cardReward.Id);
		
        // Damage taken for option 1
        storage.SetValue("$hpLoss", 12f); 

        // Pre-pick a random exhibit for option 2
        Exhibit exhibitReward = base.Stage.GetEliteEnemyExhibit();
        storage.SetValue("$exhibitReward", exhibitReward.Config.Id);

        // Power loss for option 2
        storage.SetValue("$powerLoss", 20f); 
    }
}
```

---

### Step 8: Write Dialogue Localization (`YuukaGardenEn.yaml`)

Create `YuukaGardenEn.yaml` and put it in `DirResources` to localize all `#line:` tags from your Yarn script:

```yaml
line:yuuka_001: "Oh, who's there in my garden? Are you enjoying yourself with the beauty of these flowers?"
line:yuuka_002: "Surely you did not stomp on any of them, right?"
line:yuuka_003: "(You get hit by some danmaku while running away.)"
line:yuuka_004: "Good. I'm glad some people here are still respectful."
line:opt_run: "Run. Lose {0} HP, gain |{1}|."
line:opt_apologize: "Apologize. Lose {0} <sprite=\"Point\" name=\"Power\">, gain a random Exhibit."
```

> **Formatting Note:** `{0}`, `{1}`, etc., map to arguments passed in the original text of in the Yarn.

---

### Step 9: Build Your Project

Build your solution in Visual Studio or via `dotnet build`. 

The `.csproj` target added in Step 1 will automatically invoke `YarnTools/ysc.exe`, compile `Resources/yarn/YuukaGarden.yarn` into `Resources/yarnc/YuukaGarden.yarnc`, and embed it into your DLL. You can safely ignore any extra `.csv` files generated alongside it.

---

### Step 10: Test In-Game

1. Start LBoL with **Debug Mode** enabled.
2. Open the debug panel and navigate to the **Events** tab (3rd tab).
3. You will find it at the end.

However, your event hasn't been added to the actual pool of encounterable events within the act yet.

---

### Step 11: Adding the event to an act.

To actually make your event encounterable during gameplay, you must add it to the pool of available adventures in the required stage.

Stages have two distinct adventure pools:
* **`FirstAdventurePool`**: Events in this pool will always be encountered as your **very first event node** of the act. (For example, Patchouli, Junko, or Shinmyoumaru in Act 2. Act 1's first adventure pool is empty in vanilla).
* **`AdventurePool`**: The general pool of events encountered throughout the rest of the act.

For example, let's say we have two events: `YuukaGarden` and `ReimuSteal`. We want `YuukaGarden` to always be a candidate for the first event node of Act 1, and `ReimuSteal` to be in the general event pool for Act 1.


Inside your BepinexPlugin class (or whichever class registers your mod's harmony patches and to entity manager), add the delegate to modify the stage `BambooForest`, which is the class for act 1.
```
private void Awake()
{
    log = Logger; 

    DontDestroyOnLoad(gameObject);
    gameObject.hideFlags = HideFlags.HideAndDontSave;

    EntityManager.RegisterSelf();

    harmony.PatchAll();

    // Add the delegate here.
    // You can also add enemies and other stuff to the pool here.
    StageTemplate.ModifyStage(nameof(BambooForest), stage =>
    {
        stage.FirstAdventurePool.Add(typeof(YuukaGarden), 1.2f); // (AdventureType, Weight)
        stage.AdventurePool.Add(typeof(ReimuSteal), 1.2f); // (AdventureType, Weight)

        return stage;
    });
}
```

#### Modifying ALL Stages (`StageTemplate.ModifyStageList`)
`StageTemplate.ModifyStageList` can be used to iterate over all stages that would appear in the run instead. This can be used, for example, to ensure your event always appears in act 1, including in custom ones.

```csharp
// Example: Add YuukaGarden to ALL Act 1 stages in the run
StageTemplate.ModifyStageList(stages =>
{
    foreach (var stage in stages)
    {
        if (stage.Level == 1) // Act 1
        {
            stage.AdventurePool.Add(typeof(YuukaGarden), 1.0f);
        }
    }
    return stages;
});
```

(Though a custom act 1 might have its own events and doesn't want other events added to it, so use with caution.)

---

### Step 12 (Optional): Adding Custom Encounter Weights (`IAdventureWeighter`)

Say you want your event to only trigger under specific conditions, or for it to trigger at higher odds depending on game state (e.g., current player character, money, HP, or exhibits).

To do this, add an `[AdventureInfo]` attribute to your adventure class and point it to a custom `IAdventureWeighter` class.

For example, `ReimuSteal` is an event where you can fight Reimu for gold. We want it to be **impossible** to appear if you're playing as Reimu, and **much more likely** to appear if you're low on money:
```csharp
[AdventureInfo(WeighterType = typeof(ReimuStealWeighter))]
[EntityLogic(typeof(ReimuStealDef))]
public sealed class ReimuSteal : Adventure
{
    public class ReimuStealWeighter : IAdventureWeighter
    {
        public float WeightFor(Type type, GameRunController gameRun)
        {
            if (gameRun.Player is Reimu) return 0; // Weights are multiplied by 0 if you are Reimu
            if (gameRun.Money < 50) return 5; // If you are not Reimu, and are poor, then you are 5x more likely to encounter this event.
            
            return 1; // Otherwise this has a normal appearance rate.
        }
    }
    protected override void InitVariables(IVariableStorage storage)
    {
        storage.SetValue("$goldGain", 200f);
        storage.SetValue("$reimuOpponent", "Reimu");
    }
}
```
