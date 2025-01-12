# GET /Api/CS2Interface/{botName}/GetCrateContents/{crateID}

## Description

Get the contents of storage unit `crateID` owned by the given `botName`

> [!NOTE]
> Requests might fail if too many are made in a short period of time.  It's recommended that you cache this data and use the crate's `attributes.modification date` property to determine if your cache is fresh.

## Path Parameters

Name | Required | Description
--- | --- | ---
`botName` | Yes | The ASF bot name for the owner of the storage unit
`crateID` | Yes | The storage unit's item ID

## Query Parameters

Name | Required | Description
--- | --- | ---
`minimal` | No | If set to true, the response will only contain the data recieved from CS2
`showDefs` | No | If set to true, the response will include a `defs` property containing additional game data

## Response Result

Property | Type | Description
--- | --- | ---
`iteminfo` | `object` | The raw item info recieved from CS2.  The details of this object are defined by [`CSOEconItem`](https://github.com/SteamDatabase/Protobufs/blob/master/csgo/base_gcmessages.proto)
`attributes` | `object` | Item attributes which will vary depending on the item.  The type for these attribute's values will depend on the attribute, and can be any of: `string`, `float`, or `uint`
`casket_id` | `ulong` | The item ID for the storage unit this item is currently in
`moveable` | `bool` | True if this item can be placed into a storage unit
`full_name` | `string` | The item's name (as it would appear in the url of the item's Steam Marketplace listing page)
`full_type_name` | `string` | The item's type (as it would appear on the Steam Marketplace)
`rarity_name` | `string` | The item's rarity
`quality_name` | `string` | The item's quality
`origin_name` | `string` | How the item was obtained
`type_name` | `string` | The item's type
`item_name` | `string` | The item's name
`tool_name` | `string` | What type of tool this is (usually the same as `type_name`, except for Sealed Graffiti)
`tint_name` | `string` | The item's tint
`weapon_image_url` | `string` | An image of the item (if one exists in [items_game_cdn.txt](CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md))
`weapon_name` | `string` | The weapon this applies to
`wear_name` | `string` | The name for the float range this item falls into
`wear` | `float` | The item's float value
`wear_min` | `float` | The item's minimum possible float
`wear_max` | `float` | The item's maximum possible float
`name_id` | `string` | The unique string ID for this kind of item
`set_name_id` | `string` | The unique string ID for the collection this item belongs to
`set_name` | `string` | The name of the collection this item belongs to
`crate_name_id` | `string` | The unique string ID for the crate this item can be found in
`crate_defindex` | `uint` | The definition index for the crate this item can be found in (the definition for which can be found in [items_game.txt](CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `items`)
`crate_name` | `string` | The name of the crate this item can be found in
`defs.item_def` | `object` | Related game data found in [items_game.txt](CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `items`
`defs.paint_kit_def` | `object` | Related game data found in [items_game.txt](CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `paint_kits`
`defs.sticker_kit_def` | `object` | Related game data found in [items_game.txt](CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `sticker_kits`
`defs.music_def` | `object` | Related game data found in [items_game.txt](CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `music_definitions`
`defs.keychain_def` | `object` | Related game data found in [items_game.txt](CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `keychain_definitions`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/GetCrateContents/31555237276
```

```json
{
  "Message": "OK",
  "Success": true,
  "Result": [
    {
      "iteminfo": {
        "id": 41020250307,
        "account_id": 84122798,
        "inventory": 3221225475,
        "def_index": 4818,
        "quantity": 1,
        "level": 1,
        "quality": 4,
        "flags": 0,
        "origin": 24,
        "attribute": [
          {
            "def_index": 75,
            "value_bytes": "APZ0Zw=="
          },
          {
            "def_index": 272,
            "value_bytes": "nLXWWA=="
          },
          {
            "def_index": 273,
            "value_bytes": "BwAAAA=="
          }
        ],
        "interior_item": null,
        "in_use": false,
        "equipped_state": [],
        "rarity": 1
      },
      "attributes": {
        "tradable after date": 1735718400,
        "casket item id low": 1490466204,
        "casket item id high": 7
      },
      "casket_id": 31555237276,
      "moveable": true,
      "full_name": "Dreams & Nightmares Case",
      "full_type_name": "Base Grade Container",
      "rarity_name": "Base Grade",
      "quality_name": "Unique",
      "origin_name": "Level Up Reward",
      "type_name": "Container",
      "item_name": "Dreams & Nightmares Case",
      "name_id": "crate_community_30"
    },
    {
      "iteminfo": {
        "id": 41005308811,
        "account_id": 84122798,
        "inventory": 3221225475,
        "def_index": 36,
        "quantity": 1,
        "level": 1,
        "quality": 4,
        "flags": 0,
        "origin": 23,
        "attribute": [
          {
            "def_index": 6,
            "value_bytes": "AIBBRA=="
          },
          {
            "def_index": 7,
            "value_bytes": "DLIGQw=="
          },
          {
            "def_index": 8,
            "value_bytes": "SMWOPQ=="
          },
          {
            "def_index": 75,
            "value_bytes": "gKRzZw=="
          },
          {
            "def_index": 272,
            "value_bytes": "nLXWWA=="
          },
          {
            "def_index": 273,
            "value_bytes": "BwAAAA=="
          }
        ],
        "interior_item": null,
        "in_use": false,
        "equipped_state": [],
        "rarity": 3
      },
      "attributes": {
        "set item texture prefab": 774,
        "set item texture seed": 131.4292,
        "set item texture wear": 0.06968424,
        "tradable after date": 1735632000,
        "casket item id low": 1490466204,
        "casket item id high": 7
      },
      "casket_id": 31555237276,
      "moveable": true,
      "full_name": "P250 | Small Game (Factory New)",
      "full_type_name": "Mil-Spec Grade Pistol",
      "rarity_name": "Mil-Spec Grade",
      "quality_name": "Unique",
      "origin_name": "Quest Reward",
      "type_name": "Pistol",
      "item_name": "Small Game",
      "weapon_name": "P250",
      "wear_name": "Factory New",
      "wear": 0.069684237241745,
      "wear_min": 0,
      "wear_max": 0.7,
      "name_id": "[soch_hunter_blaze_p250]weapon_p250",
      "set_name_id": "set_realism_camo",
      "set_name": "The Sport & Field Collection"
    }
  ]
}
```
