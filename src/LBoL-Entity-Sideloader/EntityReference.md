### Card

#### CardImages
Optimal card image size is 743x512. It can be smaller as ultimately card image gets scaled down to 454x313.

#### CardConfig

|Config property|Explanation|
|-------|------|
|Index| for index ordering in collection, library etc. Can be kept track of easily by using `TemplateSequenceTable`. Sideloader ensures that every index is unique although duplicate indexes are not game breaking. All modded indexes start at 12000 and can vary depending on other mods loaded.|
|Id| should be left blank since it's eventually set by GetId() anyway.|
|Order| default priority for reactors/handlers. Priority is used to set order of actions which trigger at the same time, for example, on TurnStarted. 10 is default priority and priority argument can be used on reactors/handlers. **Lower number = higher priority**|
|AutoPerform| perform visual effects on cast automatically? Only one card, Expanding Border (Id:KuosanJiejie), has AutoPerform set to false. |
|Perform| visual effects to perform. A bit convoluted and experimentation is needed to fully understand how it works. It best to leave this empty and perform visual effects in Card Actions. TimeWalk is a good example how to perform various effects when a card is played.*|
|GunName| visual effect when the card is played. Has to be a GunConfig Id. No way to add these just yet.|
|GunNameBurst| different (stronger) visual effect when firepower exceed half of base dmg, card is upgraded or player is in Burst|
|DebugLevel| is the card being developed? Should always be 0 for card to appear in a run.|
|Revealable| can card be revealed in collection? Should most likely be false. This is a bit unintuitive but cards with Revealable false are revealed in the collection without need to find them.|
|IsPooled| can card be found naturally as reward, in shops, card gen? Cards like Yin-Yan orbs, Astrology and so on have this property set to false.|
|HideMesuem| hide card in the collection?|
|IsUpgradable| is card upgradable?|
|Rarity| rarity|
|Type| card type|
|TargetType| what is this card targeting? TargetType.All is bugged for attack cards, maybe some others as well|
|Colors| colors of a card, i.e., new List<ManaColor>() { ManaColor.Red, ManaColor.Blue, ManaColor.White } would make the card Red, Blue and White.*|
|IsXCost| does the card have X as mana cost option|
|Cost| mana cost, i.e., new ManaGroup() { Any=2, Red = 1, Blue = 2 } is 2RUU. Empty list = 0 cost|
|UpgradedCost| if upgrade changes the mana cost specify it here. If upgrade doesn't affect cost leave null|
|MoneyCost| how much money does this card cost to play?|
|Damage| dmg value if the card deals damage. Else leave null.|
|UpgradedDamage| upgraded damage value. If upgrade doesn't change damage leave null.|
|Block| block value if card grants block. Else leave null.|
|UpgradedBlock| upgraded block value. If upgrade doesn't change block leave null.|
|Shield| barrier value if card grants block. Else leave null.|
|UpgradedShield| upgraded barrier value. If upgrade doesn't change block leave null.|
|Value1| some value which card might use for its effect. For example, Impatience uses this value to set the amount of cards it draws.|
|UpgradedValue1| upgraded Value1 if upgrade affects it. Else null.|
|Value2| additional optional value, same as Value1|
|UpgradedValue2| upgraded Value2|
|Mana| some mana group a card might use for its effect. It could, for example, be mana granted like Celestial Flight cost change like|
|UpgradedMana| upgraded Mana if upgrade affects it.|
|Scry| scry amount if the card scrys.|
|UpgradedScry| upgraded scry.|
|ToolPlayableTimes| number times of a Limited card can be played. Referred to as {DeckCounter} in card description.
|Keywords| keywords which directly affect how the card works like Exile, Replenish etc. Multiple keywords can be specified by using `|` operator, i.e., `Keyword.Replenish | Keyword.Exile | Keyword.Accuracy`|
|UpgradedKeywords| keywords after upgrading. It's important to note that unlike other upgraded properties base card variant keywords need to respecified if they are desired to be kept in the upgraded version. Some upgrades might remove or change 'bad' keywords like Exile.|
|EmptyDescription| does the card have NO description? Some status cards don't have a description.|
|RelativeKeyword| keywords which don't have direct effect but are relevant for what the card does like Overdraft (Overdraft is triggered by yielding `LockRandomTurnManaAction` action in Card Actions). They will appear as a tooltip next to the card. Note that common keywords like block, barrier, scry can be inferred and will be added to the tooltip automatically without being specified.|
|UpgradedRelativeKeyword| if keywords requiring explanation change or remain the same after upgrade they will need to be (re)specified here.|
|RelativeEffects| list of StatusEffect Ids relevant for the effect of the card like Firepower or TimeAuraSe(Timepulse)|
|UpgradedRelativeEffects| list of StatusEffects after upgrading. Base effect might need to be respecified.|
|RelativeCards| list of Card Ids which are created or otherwise relevant to effect of the card. Example, Queen of Yin-Yang Orb (Id:YinyangQueen).|
|UpgradedRelativeCards| list of cards for upgraded version of the card. Base list might need to be respecified.|
|Owner| owner of the card, one of the playable characters. Leave `null` to make the card Neutral. VanillaCharNames can be used to avoid typos.|
|Unfinished| should the card have 'Unfinished' disclaimer on card image.|
|Illustrator| name of the artist.|
|SubIllustrator| list of subartist names. These name will be used as a key suffix when card images are cached to `ResourcesHelper.CardImages` dictionary.|

#### Perform

 Perform needs to be an array of string arrays where each inner array contains information about effect to be performed. The first element of inner array can be "1", "2", "3" or "4". This specifies the type of effect to perform. Second element is the name of the effect. Third, is optional additional property for the effect.

- "1" - Gun (attack effect). Name must be from GunConfig. 3rd optional parameter is occupationTime locking the game for certain time in seconds after the effect is performed. Can be float i.e. "1.2".
- "2" - Effect. Name must be from "EffectConfig". 3rd optional parameter is delay in seconds before the effect is performed (sideloader only). It can be used to chain effects one after the other. Can be float.
- "3" - unit animation. Player units have specific spine animations like "spell", "idle" (no full list yet). 3rd optional parameter is occupationTime.
- "4" - sfx. Name must be from SfxConfig. 3rd optional parameter is delay (sideloader only).


#### Colors

The colors are important for pooling rules. In the example, Colors = new List<ManaColor>() { ManaColor.Red, ManaColor.Blue, ManaColor.White }, the card could only be found if the player has permanent source of Red, Blue and White mana regardless of the card's cost. If the list is left empty (Colors = new List<ManaColor>() {}) the card won't have any color, for example, Tools don't have any color. This should not be confused with ManaColor.Colorless. Card having ManaColor.Colorless color means that the card cannot be found unless the player has source of Colorless mana i.e. Magic Mallet. Filthless World would be an example of a Colorless card. Technically, it's ok for a card to be Colorless and have some other colors like List<ManaColor>() {ManaColor.Colorless, ManaColor.Red}. This would require a source of Red and Colorless mana for the card to be pooled. ManaColor options ManaColor.Any and ManaColor.Philosophy should never be used here and will result in error if used.

### StatusEffect

#### Sprite
A 128x128 icon for status effect.

#### StatusEffectConfig
*so far the most cryptic of configs*

|Config property|Explanation|
|-------|------|
|Id| should be left blank since it's eventually set by GetId() anyway.|
|Order| default priority for reactors/handlers. priority argument on reactors/handlers can be used instead. Priority 10 is normal.|
|Type| Positive, negative or special. Special is something which doesn't interact with most of the game's mechanics i.e. Event Horizon's game over effect.|
|IsVerbose| does the effect have 'Brief' description? Brief description can be specified in yaml as 'Brief:' node. Brief will be used as status effect description outside of battle. Important if the status effect is generic, like Firepower or Flawless, and might need to be displayed as tooltip for cards. But for most cases it's fine to leave this false.|
|IsStackable| can the same status be added on top? Currently all status effects have this set to true but some of them doesn't have a level.|
|StackActionTriggerLevel| should effect status effect be Removed and execute StatusAction when reaching a certain level? Charge (Id:Charging) has StackActionTriggerLevel set to 8. When Charge reaches 8 it applies Burst through its StackAction.|
|HasLevel| does the StatusEffect have stack count? Most of ability status effects have stacks which allows them to get stronger when additional copies of the same ability are played. For example, Fire of Ena (Id:ModuoluoFireSe) deal damage equal to amount_of_stacks x rainbow_mana_spent.|
|LevelStackType| how should effects levels stack when an effect is applied while the same effect is already present? This property is irrelevant if HasLevel is false.*|
|HasDuration| does the status disappears after certain number of turns? Frail, Weak, Vuln ect. all expire after certain amount of turns.|
|DurationStackType| |
|DurationDecreaseTiming| DurationDecreaseTiming.Custom,|
|HasCount| has a counter? Number displayed on status effect icon for keeping track of some additional effects. For example, Rain of Hell (Id:HekaHellRainSe) use it to show total amount of damage which would be dealt at the end of the turn.|
|CountStackType| |
|LimitStackType| |
|ShowPlusByLimit| if the StatusEffect has limit appends `+` to effect's name.|
|Keywords| |
|RelativeEffects| |
|VFX| "Default"|
|VFXloop| "Default"|
|SFX| "Default"|


#### StackType
|Type| Explanation|
|-------|------|
|StackType.Add| add the two levels. The most common option.|
|StackType.Max| use the higher level.|
|StackType.Min| use the lower level.|
|StackType.Keep| always keep already present level.|
|StackType.Max| discard the present level and use the new one.|

### Exhibit

#### ExhibitSprites
A 512x512 icon. `Dictionary<string, Sprite> customSprites` can be specified for switching icons on `Exhibit.OverrideIconName`. See [FistOfTheThreeFairies.cs](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/272169d6e952902096c5f27a7b18b91048c978ff/src/GoodExamples/Exhibits/FistOfTheThreeFairies.cs#L99)

#### ExhibitConfig
*to be added*

|Config property|Explanation|
|-------|------|
|Index||
|Id||
|Order||
|IsDebug||
|IsPooled||
|IsSentinel||
|Revealable||
|Appearance||
|Owner||
|LosableType||
|Rarity||
|Value1||
|Value2||
|Value3||
|Mana||
|BaseManaRequirement||
|BaseManaColor||
|BaseManaAmount||
|HasCounter||
|InitialCounter||
|Keywords||
|RelativeEffects||
|RelativeCards||
