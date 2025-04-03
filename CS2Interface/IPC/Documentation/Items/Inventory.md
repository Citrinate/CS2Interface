# GET /Api/CS2Interface/{botName}/Inventory

## Description

Get the CS2 inventory for the given `botName`

> [!NOTE]
> There's a few seconds of delay between when the interface first connects and when the bot's inventory is available.

## Path Parameters

Name | Required | Description
--- | --- | ---
`botName` | Yes | The ASF bot name to get the inventory of

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
`weapon_image_url` | `string` | An image of the item (if one exists in [items_game_cdn.txt](/CS2Interface/IPC/Documentation/Data/ItemsGameCdnTxt.md))
`weapon_name` | `string` | The weapon this applies to
`wear_name` | `string` | The name for the float range this item falls into
`wear` | `float` | The item's float value
`wear_min` | `float` | The item's minimum possible float
`wear_max` | `float` | The item's maximum possible float
`name_id` | `string` | The unique string ID for this kind of item
`set_name_id` | `string` | The unique string ID for the collection this item belongs to
`set_name` | `string` | The name of the collection this item belongs to
`crate_name_id` | `string` | The unique string ID for the crate this item can be found in
`crate_defindex` | `uint` | The definition index for the crate this item can be found in (the definition for which can be found in [items_game.txt](/CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `items`)
`crate_name` | `string` | The name of the crate this item can be found in
`stickers` | `object` | Details for each of the stickers or patches attached to this item
`keychains` | `object` | Details for each of the keychains attached to this item
`defs.item_def` | `object` | Related game data found in [items_game.txt](/CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `items`
`defs.paint_kit_def` | `object` | Related game data found in [items_game.txt](/CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `paint_kits`
`defs.sticker_kit_def` | `object` | Related game data found in [items_game.txt](/CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `sticker_kits`
`defs.music_def` | `object` | Related game data found in [items_game.txt](/CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `music_definitions`
`defs.keychain_def` | `object` | Related game data found in [items_game.txt](/CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `keychain_definitions`

## Example Responses

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/Inventory
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
        "inventory": 1,
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
          }
        ],
        "interior_item": null,
        "in_use": false,
        "equipped_state": [],
        "rarity": 1
      },
      "attributes": {
        "tradable after date": 1735718400
      },
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
        "inventory": 1,
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
        "tradable after date": 1735632000
      },
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
    },
    {
      "iteminfo": {
        "id": 34865269642,
        "account_id": 84122798,
        "inventory": 0,
        "def_index": 874,
        "quantity": 1,
        "level": 1,
        "quality": 4,
        "flags": 0,
        "origin": 0,
        "attribute": [],
        "interior_item": null,
        "in_use": false,
        "equipped_state": [],
        "rarity": 6
      },
      "attributes": {},
      "moveable": false,
      "full_name": "5 Year Veteran Coin",
      "full_type_name": "Extraordinary Collectible",
      "rarity_name": "Extraordinary",
      "quality_name": "Unique",
      "origin_name": "Timed Drop",
      "type_name": "Collectible",
      "item_name": "5 Year Veteran Coin",
      "name_id": "Five Year Service Coin"
    },
    {
      "iteminfo": {
        "id": 31555237276,
        "account_id": 84122798,
        "inventory": 1,
        "def_index": 1201,
        "quantity": 1,
        "level": 1,
        "quality": 4,
        "flags": 0,
        "origin": 2,
        "attribute": [
          {
            "def_index": 111,
            "value_bytes": "CgEx"
          },
          {
            "def_index": 270,
            "value_bytes": "AAAAAA=="
          },
          {
            "def_index": 271,
            "value_bytes": "qtJwZw=="
          }
        ],
        "interior_item": null,
        "in_use": false,
        "equipped_state": [],
        "rarity": 1
      },
      "attributes": {
        "custom name attr": "1",
        "items count": 0,
        "modification date": 1735447210
      },
      "moveable": false,
      "full_name": "Storage Unit",
      "full_type_name": "Base Grade Tool",
      "rarity_name": "Base Grade",
      "quality_name": "Unique",
      "origin_name": "Purchased",
      "type_name": "Tool",
      "item_name": "Storage Unit",
      "name_id": "casket"
    },
    {
      "iteminfo": {
        "id": 22487518448,
        "account_id": 84122798,
        "inventory": 2,
        "def_index": 1314,
        "quantity": 1,
        "level": 1,
        "quality": 4,
        "flags": 0,
        "origin": 0,
        "attribute": [
          {
            "def_index": 2,
            "value_bytes": "AQAAAA=="
          },
          {
            "def_index": 166,
            "value_bytes": "KAAAAA=="
          }
        ],
        "interior_item": null,
        "in_use": false,
        "equipped_state": [],
        "rarity": 3
      },
      "attributes": {
        "cannot trade": 1,
        "music id": 40
      },
      "moveable": false,
      "full_name": "Music Kit | Halo, The Master Chief Collection",
      "full_type_name": "High Grade Music Kit",
      "rarity_name": "High Grade",
      "quality_name": "Unique",
      "origin_name": "Timed Drop",
      "type_name": "Music Kit",
      "item_name": "Halo, The Master Chief Collection",
      "name_id": "[halo_01]musickit"
    }
  ]
}
```

---

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/Inventory
```

```json
{
  "Message": "Inventory not loaded yet",
  "Success": false
}
```
