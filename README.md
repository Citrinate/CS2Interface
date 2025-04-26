# Counter-Strike 2 Interface Plugin for ArchiSteamFarm

[![Check out my other ArchiSteamFarm plugins](https://img.shields.io/badge/Check%20out%20my%20other%20ArchiSteamFarm%20plugins-blue?logo=github)](https://github.com/stars/Citrinate/lists/archisteamfarm-plugins) [![Help with translations](https://img.shields.io/badge/Help%20with%20translations-purple?logo=crowdin)](https://github.com/Citrinate/CS2Interface/tree/main/CS2Interface/Localization) ![GitHub all releases](https://img.shields.io/github/downloads/Citrinate/CS2Interface/total?logo=github&label=Downloads)

## Introduction

This plugin allows you to interact with Counter-Strike 2 using ArchiSteamFarm's [IPC interface](https://github.com/JustArchiNET/ArchiSteamFarm/wiki/IPC).

## Installation

- Download the .zip file from the [latest release](https://github.com/Citrinate/CS2Interface/releases/latest)
- Unpack the downloaded .zip file to the `plugins` folder inside your ASF folder.
- (Re)start ASF, you should get a message indicating that the plugin loaded successfully. 

> [!NOTE]
> This plugin is only tested to work with ASF-generic.  It may or may not work with other ASF variants, but feel free to report any issues you may encounter.

## Usage

### Commands

Command | Access | Description
--- | --- | ---
`cs2interface`|`FamilySharing`|Prints the current version of the plugin
`cstart [Bots] [Minutes]`|`Master`|Starts the CS2 Interface.  Optinally, automatically stop the interface after `Minutes` of inactivity
`cstop [Bots]`|`Master`|Stops the CS2 Interface
`cstatus [Bots]`|`Master`|Displays the status of the CS2 Interface

#### Command Aliases

Command | Alias |
--- | --- |
`cstatus asf`|`csa`

---

### AutoStartCS2Interface

`bool` type with default value of `false`.  This configuration setting can be added to your individual bot config files.  If set to `true`, the CS2 Interface will automatically start after the bot comes online.  When used, [`FarmingPreferences`](https://github.com/JustArchiNET/ArchiSteamFarm/wiki/Configuration#farmingpreferences) should also have the `FarmingPausedByDefault` flag enabled.

```json
"AutoStartCS2Interface": true,
"FarmingPreferences": 1,
```

> [!NOTE]
> It's not possible for a bot to farm non-CS2 cards and use the CS2 Interface at the same time.  These two operations can interfere with one another on startup, and so it's important to also enable the `FarmingPausedByDefault` flag.  This will prevent ASF's CardFarmer module from starting automatically.
> 
> If you want to farm cards you can still do so using ASF's `resume` command, and the CS2 Interface will automatically resume when ASF stops farming.  There are events however that will cause ASF to stop farming even if ASF isn't actually finished farming (such as when a new game is added to your bot's account).  To guarantee that ASF fishishes farming cards you should use the `cstop` command, and then manually resume the interface when card farming is complete using the `cstart` command.

---

### IPC Interface

> [!NOTE]
> Each bot can only process 1 request at a time.

#### Interface

API | Method | Parameters | Description
--- | --- | --- | ---
[`/Api/CS2Interface/{botNames}/Start`](/CS2Interface/IPC/Documentation/Interface/Start.md)|`GET`| |Starts the CS2 Interface
[`/Api/CS2Interface/{botNames}/Status`](/CS2Interface/IPC/Documentation/Interface/Status.md)|`GET`| |Get the CS2 Interface status
[`/Api/CS2Interface/{botNames}/Stop`](/CS2Interface/IPC/Documentation/Interface/Stop.md)|`GET`| |Stops the CS2 Interface

#### Items

API | Method | Parameters | Description
--- | --- | --- | ---
[`/Api/CS2Interface/{botName}/CraftItem/{recipeID}`](/CS2Interface/IPC/Documentation/Items/CraftItem.md)|`GET`|`itemIDs`|Craft an item
[`/Api/CS2Interface/{botName}/GetCrateContents/{crateID}`](/CS2Interface/IPC/Documentation/Items/GetCrateContents.md)|`GET`|`minimal`, `showDefs`|Get a storage unit's contents
[`/Api/CS2Interface/{botNames}/InspectItem`](/CS2Interface/IPC/Documentation/Items/InspectItem.md)|`GET`|`url`, `s`, `a`, `d`, `m`, `minimal`, `showDefs`|Inspect an item
[`/Api/CS2Interface/{botName}/Inventory`](/CS2Interface/IPC/Documentation/Items/Inventory.md)|`GET`|`minimal`, `showDefs`|Get a bot's inventory
[`/Api/CS2Interface/{botName}/RetrieveItem/{crateID}/{itemID}`](/CS2Interface/IPC/Documentation/Items/RetrieveItem.md)|`GET`| |Take an item out of a storage unit
[`/Api/CS2Interface/{botName}/StoreItem/{crateID}/{itemID}`](/CS2Interface/IPC/Documentation/Items/StoreItem.md)|`GET`| |Place an item into a storage unit

#### Players

API | Method | Parameters | Description
--- | --- | --- | ---
[`/Api/CS2Interface/{botName}/PlayerProfile/{steamID}`](/CS2Interface/IPC/Documentation/Players/PlayerProfile.md)|`GET`| |Get a friend's player profile

#### Data

API | Method | Parameters | Description
--- | --- | --- | ---
[`/Api/CS2Interface/Recipes`](/CS2Interface/IPC/Documentation/Data/Recipes.md)|`GET`|`showDefs`|Get a list of crafting recipes
[`/Api/CS2Interface/items_game.txt`](/CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md)|`GET`| |Get the contents of `items_game.txt`
[`/Api/CS2Interface/items_game_cdn.txt`](/CS2Interface/IPC/Documentation/Data/ItemsGameCdnTxt.md)|`GET`| |Get the contents of `items_game_cdn.txt`
[`/Api/CS2Interface/csgo_english.txt`](/CS2Interface/IPC/Documentation/Data/CsgoEnglishTxt.md)|`GET`| |Get the contents of `csgo_english.txt`
[`/Api/CS2Interface/steam.inf`](/CS2Interface/IPC/Documentation/Data/SteamInf.md)|`GET`| |Get the contents of `steam.inf`
