# GET /Api/CS2Interface/items_game_cdn.txt

## Description

Get the contents of the CS2 game file: [`scripts/items/items_game_cdn.txt`](https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/scripts/items/items_game_cdn.txt)

## Path Parameters

None

## Query Parameters

None

## Response Result

Property | Type | Description
--- | --- | ---
`ClientVersion` | `uint` | The version of CS2 that this data comes from
`Data` | `object` | The contents of `scripts/items/items_game_cdn.txt`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/items_game_cdn.txt
```

```javascript
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "ClientVersion": 2000468,
    "Data": {
      "leather_handwraps_handwrap_camo_grey": "http://media.steampowered.com/apps/730/icons/econ/default_generated/leather_handwraps_handwrap_camo_grey_light_large.04557b1a8d68bccdd60b18521346091328756ded.png",
      "leather_handwraps_handwrap_fabric_houndstooth_orange": "http://media.steampowered.com/apps/730/icons/econ/default_generated/leather_handwraps_handwrap_fabric_houndstooth_orange_light_large.08248935a70031a18cb246f3e3ac2bc0d8d66339.png",
      "leather_handwraps_handwrap_fabric_orange_camo": "http://media.steampowered.com/apps/730/icons/econ/default_generated/leather_handwraps_handwrap_fabric_orange_camo_light_large.f8453c60f74a846bd3c05310de4f004cd95a1aa2.png",
     //...
    }
  }
}
```
