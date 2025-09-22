Modding framework for working with LBoL entities.

[h1]Thanks to the contributors![/h1]

[url=https://github.com/IntoxicatedKid]IntoxicatedKid[/url],
[url=https://github.com/cyaneko]cyaneko[/url]

[h1]Change log[/h1]

'0.9.7849' Update for Steam workshop update. [b]Not compatible with previous versions[/b]. Fix "Prepare" boss reward being able to remove unremovable cards.

'0.9.7848' Make 'CustomGamerunSaveData' save files delete themselves when run ends. Add optional 'DeleteFileOnGamerunEnd' control and 'OnGamerunEnded' hook for 'CustomGamerunSaveData'.

'0.9.7847' Add optional 'IExtendedCardClone' interface, intended to be implemented on 'Card', for more controlled card cloning logic.

'0.9.7846' Add optional 'IExtendedKeywordName' interface for displaying more detailed keyword information on card's description.

'0.9.7845' Fix a rare softlock bug (hopefully).

'0.9.7844' Add overridable 'CardKeyword.CloneWithCard' method which exposes info of the cards being cloned. *thanks, cramps-man*

'0.9.7843' Prevent loud sfx when losing lots of mana. [i]thanks, IntoxicatedKid[/i]

'0.9.7842' Fix some interactions between cost reductions and in-battle upgrades. Mostly affects 1XX -> 2X style upgrades. *thanks, cyaneko*

'0.9.7840' Hard deprecate 'LBoLEntitySideloader.ExtraFunc.AutoCastAction'.

'0.9.7830' Small fix for 1.7.2.

'0.9.7820' Make Sideloader compatible with both beta and main branches of the game. Dual support will not be a thing in the future but, in this case, the issue was very minor and technical. Other mods are also likely to function properly on either branch.

[url=https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/README.md]Full changelog[/url]

-------------------------------------

More info at [url=https://github.com/Neoshrimp/LBoL-Entity-Sideloader/tree/master]github repo[/url]

 