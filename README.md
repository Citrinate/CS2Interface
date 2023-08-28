# Counter-Strike 2 Interface Plugin for ArchiSteamFarm

This plugin allows you to interact with Counter-Strike 2 using ArchiSteamFarm's [IPC interface](https://github.com/JustArchiNET/ArchiSteamFarm/wiki/IPC).

## Installation

- Download the .zip file from the [latest release](https://github.com/Citrinate/CS2Interface/releases/latest)
- Unpack the downloaded .zip file to the `plugins` folder inside your ASF folder.
- (Re)start ASF, you should get a message indicating that the plugin loaded successfully. 

> Please note, this plugin is only tested to work with ASF-generic.  It may or may not work with other ASF variants.

## Usage

### Commands

Command | Access | Description
--- | --- | ---
`cstart [Bots]`|`Master`|Starts the CS2 Interface
`cstop [Bots]`|`Master`|Stops the CS2 Interface
`cstatus [Bots]`|`Master`|Displays the status of the CS2 Interface

---

### AutoStartCS2Interface

`"AutoStartCS2Interface": <true/false>,`

Example: 
```
"AutoStartCS2Interface": true,
"Paused": true,
```

This `bool` type configuration setting can be added to your individual bot config files.  If set to `true`, the CS2 Interface will automatically start after the bot comes online.

> Note: It's not possible for a bot to farm non-CS2 cards and use the CS2 Interface at the same time.  To prevent interference between the two when enabling auto-start, it's important to also set `Paused` to `true`.  This will prevent ASF's CardFarmer module from starting automatically.  Even with `Paused` set to `true`, your bot will still farm cards normally whenever the CS2 Interface is stopped.

By default, this is set to `false`

---

### IPC Interface Endpoints

Each bot can only process 1 request at a time.  If multiple `botNames` are provided the first available bot will be chosen to process the request.

> Once the plugin is installed additional documentation can be found, by default, at: [`/swagger`](http://localhost:1242/swagger)

API | Method | Parameters | Description
--- | --- | --- | ---
`/Api/CS2Interface/{botNames}/Start`|`GET`||Starts the CS2 Interface
`/Api/CS2Interface/{botNames}/Stop`|`GET`||Stops the CS2 Interface
`/Api/CS2Interface/{botNames}/InspectItem`|`GET`|`url`, `s`, `a`, `d`, `m`, `minimal`, `showDefs`|Inspect a CS2 Item
`/Api/CS2Interface/{botNames}/PlayerProfile/{steamID}`|`GET`||Get a CS2 player profile
`/Api/CS2Interface/{botName}/Inventory`|`GET`|`minimal`, `showDefs`|Get the given bot's CS2 inventory
`/Api/CS2Interface/{botName}/GetCrateContents/{crateID}`|`GET`|`minimal`, `showDefs`|Get the contents of the given bot's crate
`/Api/CS2Interface/{botName}/StoreItem/{crateID}/{itemID}`|`GET`||Stores an item into the specified crate
`/Api/CS2Interface/{botName}/RetrieveItem/{crateID}/{itemID}`|`GET`||Retrieves an item from the specified crate