# GET /Api/CS2Interface/{botName}/FreeRewards

## Description

Get information about weekly care package rewards for the given `botName`.

## Path Parameters

Name | Required | Description
--- | --- | ---
`botName` | Yes | The ASF bot name to get free rewards for

## Query Parameters

Name | Required | Description
--- | --- | ---
`minimal` | No | If set to true, the response will only contain the data recieved from CS2
`showDefs` | No | If set to true, the response will include a `defs` property containing additional game data

## Response Result

Property | Type | Description
--- | --- | ---
`generation_time` | `uint` | Personal store generation timestamp in seconds
`redeemable_balance` | `uint` | Number of rewards that can currently be redeemed
`itemids` | `array<ulong>` | Raw reward item IDs from personal store data
`items` | `array<object>` | Resolved inventory items from `itemids`. Item objects use the same shape as in [`/Api/CS2Interface/{botName}/Inventory`](/CS2Interface/IPC/Documentation/Items/Inventory.md)

> [!NOTE]
> Some values in `itemids` are of the type 0xF0000000XXXXXXXXXXXXXXXX and represent already redeemed rewards. Those entries are not included in `items`

> [!NOTE]
> An error message of "Personal store not loaded yet" usually means that `botName` isn't eligible for the weekly care package.

## Example Responses

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/FreeRewards
```

```json
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "generation_time": 1775311842,
    "redeemable_balance": 1,
    "itemids": [
      50855763465,
      17293822569188360228,
      50855763467,
      50855763468
    ],
    "items": [
      {
        "iteminfo": {
          "id": 50855763467,
          "account_id": 1074391737,
          "inventory": 1,
          "def_index": 34,
          "quality": 4,
          "flags": 0,
          "origin": 24,
          "attribute": [
            {
              "def_index": 277,
              "value_bytes": "AQAAAA=="
            },
            {
              "def_index": 6,
              "value_bytes": "AOCrRA=="
            },
            {
              "def_index": 7,
              "value_bytes": "IPZXQw=="
            },
            {
              "def_index": 8,
              "value_bytes": "2oeOPQ=="
            }
          ],
          "interior_item": null,
          "equipped_state": [],
          "rarity": 1
        },
        "attributes": {
          "free reward status": 1,
          "set item texture prefab": 1375,
          "set item texture seed": 215.96143,
          "set item texture wear": 0.069595054
        },
        "moveable": false,
        "full_name": "MP9 | Dizzy (Factory New)",
        "full_type_name": "Consumer Grade SMG",
        "rarity_name": "Consumer Grade",
        "quality_name": "Unique",
        "origin_name": "Level Up Reward",
        "type_name": "SMG",
        "item_name": "Dizzy",
        "weapon_name": "MP9",
        "wear_name": "Factory New",
        "wear": 0.06959505379199982,
        "wear_min": 0,
        "wear_max": 0.65,
        "stattrak": false,
        "commodity": false,
        "name_id": "[soe_ddpat_swirl_bw]weapon_mp9",
        "set_name_id": "set_timed_drops_achroma",
        "set_name": "The Achroma Collection"
      },
      {
        "iteminfo": {
          "id": 50855763468,
          "account_id": 1074391737,
          "inventory": 1,
          "def_index": 1348,
          "quality": 4,
          "flags": 0,
          "origin": 24,
          "attribute": [
            {
              "def_index": 277,
              "value_bytes": "AQAAAA=="
            },
            {
              "def_index": 113,
              "value_bytes": "pQYAAA=="
            },
            {
              "def_index": 233,
              "value_bytes": "CQAAAA=="
            }
          ],
          "interior_item": null,
          "equipped_state": [],
          "rarity": 1
        },
        "attributes": {
          "free reward status": 1,
          "sticker slot 0 id": 1701,
          "spray tint id": 9
        },
        "moveable": false,
        "full_name": "Sealed Graffiti | Keep the Change (Frog Green)",
        "full_type_name": "Base Grade Graffiti",
        "rarity_name": "Base Grade",
        "quality_name": "Unique",
        "origin_name": "Level Up Reward",
        "type_name": "Graffiti",
        "item_name": "Keep the Change",
        "tool_name": "Sealed Graffiti",
        "tint_name": "Frog Green",
        "stattrak": false,
        "commodity": true,
        "name_id": "[spray_std_dollar]spray",
        "stickers": {
          "1701": {
            "full_name": "Sticker | Keep the Change",
            "full_type_name": "Base Grade Sticker",
            "rarity_name": "Base Grade",
            "quality_name": "Unique",
            "type_name": "Sticker",
            "item_name": "Keep the Change",
            "tool_name": "Sticker",
            "commodity": true,
            "name_id": "[spray_std_dollar]sticker"
          }
        }
      },
      {
        "iteminfo": {
            "id": 50855763465,
            "account_id": 1074391737,
            "inventory": 1,
            "def_index": 4818,
            "quality": 4,
            "flags": 0,
            "origin": 24,
            "attribute": [
            {
                "def_index": 277,
                "value_bytes": "AQAAAA=="
            }
            ],
            "interior_item": null,
            "equipped_state": [],
            "rarity": 1
        },
        "attributes": {
            "free reward status": 1
        },
        "moveable": true,
        "full_name": "Dreams & Nightmares Case",
        "full_type_name": "Base Grade Container",
        "rarity_name": "Base Grade",
        "quality_name": "Unique",
        "origin_name": "Level Up Reward",
        "type_name": "Container",
        "item_name": "Dreams & Nightmares Case",
        "stattrak": false,
        "commodity": true,
        "name_id": "crate_community_30"
        }
    ]
  }
}
```

---

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/FreeRewards
```

```json
{
  "Message": "Personal store not loaded yet",
  "Success": false
}
```
