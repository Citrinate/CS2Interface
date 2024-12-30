# GET /Api/CS2Interface/{botNames}/InspectItem

## Description

Inspect a CS2 Item

## Path Parameters

Name | Required | Description
--- | --- | ---
`botNames` | Yes | One or more ASF [bot names](https://github.com/JustArchiNET/ArchiSteamFarm/wiki/Commands#bots-argument)

> [!NOTE]
> Responses are not dependent on the account used to inspect an item. You may provide multiple `botNames`, and the first available bot will be used to make the request.

## Query Parameters

Name | Required | Description
--- | --- | ---
`url` | No | The item's inspect link
`s` | No | The `S` value from the item's inspect link (not needed if using the `url` parameter)
`a` | No | The `A` value from the item's inspect link (not needed if using the `url` parameter)
`d` | No | The `D` value from the item's inspect link (not needed if using the `url` parameter)
`m` | No | The `M` value from the item's inspect link (not needed if using the `url` parameter)
`minimal` | No | If set to true, the response will only contain the data recieved from CS2
`showDefs` | No | If set to true, the response will include a `defs` property containing additional game data

## Response Result

Property | Type | Description
--- | --- | ---
`iteminfo` | `object` | The raw item info recieved from CS2.  The details of this object are defined by [`CEconItemPreviewDataBlock`](https://github.com/SteamDatabase/Protobufs/blob/master/csgo/cstrike15_gcmessages.proto)
`full_name` | `string` | The item's name (as it would appear in the url of the item's Steam Marketplace listing page)
`full_type_name` | `string` | The item's type (as it would appear on the Steam Marketplace)
`rarity_name` | `string` | The item's rarity
`quality_name` | `string` | The item's quality
`origin_name` | `string` | How the item was obtained
`type_name` | `string` | The item's type
`item_name` | `string` | The item's name
`tool_name` | `string` | What type of tool this is (usually the same as `type_name`, except for Sealed Graffiti)
`tint_name` | `string` | The item's tint
`weapon_image_url` | `string` | An image of the item (if one exists in [items_game_cdn.txt](https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/scripts/items/items_game_cdn.txt))
`weapon_name` | `string` | The weapon this applies to
`wear_name` | `string` | The name for the float range this item falls into
`wear` | `float` | The item's float value
`wear_min` | `float` | The item's minimum possible float
`wear_max` | `float` | The item's maximum possible float
`name_id` | `string` | The unique string ID for this kind of item
`set_name_id` | `string` | The unique string ID for the collection this item belongs to
`set_name` | `string` | The name of the collection this item belongs to
`crate_name_id` | `string` | The unique string ID for the crate this item can be found in
`crate_defindex` | `uint` | The definition index for the crate this item can be found in (the definition for which can be found in [items_game.txt](https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/scripts/items/items_game.txt) under `items`)
`crate_name` | `string` | The name of the crate this item can be found in
`defs.item_def` | `object` | Related game data found in [items_game.txt](https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/scripts/items/items_game.txt) under `items`
`defs.paint_kit_def` | `object` | Related game data found in [items_game.txt](https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/scripts/items/items_game.txt) under `paint_kits`
`defs.sticker_kit_def` | `object` | Related game data found in [items_game.txt](https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/scripts/items/items_game.txt) under `sticker_kits`
`defs.music_def` | `object` | Related game data found in [items_game.txt](https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/scripts/items/items_game.txt) under `music_definitions`
`defs.keychain_def` | `object` | Related game data found in [items_game.txt](https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/scripts/items/items_game.txt) under `keychain_definitions`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/asf/InspectItem?url=steam://rungame/730/76561202255233023/+csgo_econ_action_preview%20M625254122282020305A6760346663D30614827701953021
```

```json
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "iteminfo": {
      "itemid": 6760346663,
      "defindex": 7,
      "paintindex": 282,
      "rarity": 5,
      "quality": 9,
      "paintwear": 1041772571,
      "paintseed": 566,
      "killeaterscoretype": 0,
      "killeatervalue": 0,
      "stickers": [
        {
          "slot": 0,
          "sticker_id": 815
        },
        {
          "slot": 1,
          "sticker_id": 1008
        },
        {
          "slot": 2,
          "sticker_id": 328
        },
        {
          "slot": 3,
          "sticker_id": 1223
        }
      ],
      "inventory": 44,
      "origin": 8,
      "keychains": []
    },
    "full_name": "StatTrak™ AK-47 | Redline (Minimal Wear)",
    "full_type_name": "StatTrak™ Classified Rifle",
    "rarity_name": "Classified",
    "quality_name": "StatTrak™",
    "origin_name": "Found in Crate",
    "type_name": "Rifle",
    "item_name": "Redline",
    "weapon_image_url": "http://media.steampowered.com/apps/730/icons/econ/default_generated/weapon_ak47_cu_ak47_cobra_light_large.7494bfdf4855fd4e6a2dbd983ed0a243c80ef830.png",
    "weapon_name": "AK-47",
    "wear_name": "Minimal Wear",
    "wear": 0.14862100780010223,
    "wear_min": 0.1,
    "wear_max": 0.7,
    "name_id": "[cu_ak47_cobra]weapon_ak47",
    "set_name_id": "set_community_2",
    "set_name": "The Phoenix Collection",
    "crate_name_id": "crate_community_2",
    "crate_defindex": 4011,
    "crate_supply_series": 11,
    "crate_name": "Operation Phoenix Weapon Case"
  }
}
```

---

```
http://127.0.0.1:1242/Api/CS2Interface/asf/InspectItem?url=steam://rungame/730/76561202255233023/+csgo_econ_action_preview%20M625254122282020305A6760346663D30614827701953021&showDefs=true
```

```json
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "iteminfo": {
      "itemid": 6760346663,
      "defindex": 7,
      "paintindex": 282,
      "rarity": 5,
      "quality": 9,
      "paintwear": 1041772571,
      "paintseed": 566,
      "killeaterscoretype": 0,
      "killeatervalue": 0,
      "stickers": [
        {
          "slot": 0,
          "sticker_id": 815
        },
        {
          "slot": 1,
          "sticker_id": 1008
        },
        {
          "slot": 2,
          "sticker_id": 328
        },
        {
          "slot": 3,
          "sticker_id": 1223
        }
      ],
      "inventory": 44,
      "origin": 8,
      "keychains": []
    },
    "full_name": "StatTrak™ AK-47 | Redline (Minimal Wear)",
    "full_type_name": "StatTrak™ Classified Rifle",
    "rarity_name": "Classified",
    "quality_name": "StatTrak™",
    "origin_name": "Found in Crate",
    "type_name": "Rifle",
    "item_name": "Redline",
    "weapon_image_url": "http: //media.steampowered.com/apps/730/icons/econ/default_generated/weapon_ak47_cu_ak47_cobra_light_large.7494bfdf4855fd4e6a2dbd983ed0a243c80ef830.png",
    "weapon_name": "AK-47",
    "wear_name": "Minimal Wear",
    "wear": 0.14862100780010223,
    "wear_min": 0.1,
    "wear_max": 0.7,
    "name_id": "[cu_ak47_cobra]weapon_ak47",
    "set_name_id": "set_community_2",
    "set_name": "The Phoenix Collection",
    "crate_name_id": "crate_community_2",
    "crate_defindex": 4011,
    "crate_supply_series": 11,
    "crate_name": "Operation Phoenix Weapon Case",
    "defs": {
      "item_def": {
        "name": "weapon_ak47",
        "prefab": "weapon_ak47_prefab",
        "item_quality": "normal",
        "baseitem": 1,
        "flexible_loadout_slot": "rifle1",
        "flexible_loadout_default": 1,
        "item_class": "weapon_ak47",
        "item_name": "#SFUI_WPNHUD_AK47",
        "item_description": "#CSGO_Item_Desc_AK47",
        "item_rarity": "uncommon",
        "image_inventory": "econ/weapons/base_weapons/weapon_ak47",
        "model_player": "weapons/models/ak47/weapon_rif_ak47.vmdl",
        "model_world": "weapons/models/ak47/weapon_rif_ak47.vmdl",
        "model_dropped": "weapons/models/ak47/weapon_rif_ak47.vmdl",
        "icon_default_image": "materials/icons/inventory_icon_weapon_ak47.vtf",
        "used_by_classes": {
          "terrorists": 1
        },
        "attributes": {
          "magazine model": "weapons/models/ak47/weapon_rif_ak47_mag.vmdl",
          "primary reserve ammo max": 90,
          "recovery time crouch": 0.305257,
          "recovery time crouch final": 0.419728,
          "recovery time stand": 0.368,
          "recovery time stand final": 0.506,
          "inaccuracy jump initial": 100.94,
          "inaccuracy jump": 140.76,
          "heat per shot": 0.3,
          "addon scale": 0.9,
          "tracer frequency": 3,
          "max player speed": 215,
          "is full auto": 1,
          "in game price": 2700,
          "armor ratio": 1.55,
          "crosshair delta distance": 4,
          "penetration": 2,
          "damage": 36,
          "range": 8192,
          "cycletime": 0.1,
          "time to idle": 1.9,
          "flinch velocity modifier large": 0.4,
          "flinch velocity modifier small": 0.55,
          "spread": 0.6,
          "inaccuracy crouch": 4.81,
          "inaccuracy stand": 6.41,
          "inaccuracy land": 0.242,
          "inaccuracy ladder": 140,
          "inaccuracy fire": 7.8,
          "inaccuracy move": 175.06,
          "recoil angle": 0,
          "recoil angle variance": 70,
          "recoil magnitude": 30,
          "recoil magnitude variance": 0,
          "recoil seed": 223,
          "primary clip size": 30,
          "weapon weight": 25,
          "rumble effect": 4,
          "inaccuracy crouch alt": 4.81,
          "inaccuracy fire alt": 7.8,
          "inaccuracy jump alt": 140.76,
          "inaccuracy ladder alt": 140,
          "inaccuracy land alt": 0.242,
          "inaccuracy move alt": 175.06,
          "inaccuracy stand alt": 6.41,
          "max player speed alt": 215,
          "recoil angle alt": 0,
          "recoil angle variance alt": 70,
          "recoil magnitude alt": 30,
          "recoil magnitude variance alt": 0,
          "spread alt": 0.6,
          "stattrak model": "models/weapons/stattrack.vmdl",
          "recovery transition start bullet": 2,
          "recovery transition end bullet": 5,
          "allow hand flipping": 1,
          "attack movespeed factor": 1,
          "bot audible range": 2000,
          "bullets": 1,
          "cannot shoot underwater": 0,
          "crosshair min distance": 4,
          "cycletime alt": 0.3,
          "headshot multiplier": 4,
          "has burst mode": 0,
          "has silencer": 0,
          "hide view model zoomed": 0,
          "idle interval": 20,
          "inaccuracy jump apex": 0,
          "inaccuracy reload": 0,
          "inaccuracy pitch shift": 0,
          "inaccuracy alt sound threshold": 0,
          "is melee weapon": 0,
          "is revolver": 0,
          "itemflag select on empty": 0,
          "itemflag no auto reload": 0,
          "itemflag no auto switch empty": 0,
          "itemflag limit in world": 0,
          "itemflag exhaustible": 0,
          "itemflag do hit location dmg": 0,
          "itemflag no ammo pickups": 0,
          "itemflag no item pickup": 0,
          "kill award": 300,
          "model right handed": 1,
          "primary default clip size": -1,
          "range modifier": 0.98,
          "spread seed": 0,
          "secondary clip size": 0,
          "secondary default clip size": -1,
          "secondary reserve ammo max": 0,
          "unzoom after shot": 0,
          "zoom fov 1": 90,
          "zoom fov 2": 90,
          "zoom levels": 0,
          "zoom time 0": 0,
          "zoom time 1": 0,
          "zoom time 2": 0
        },
        "inventory_image_data": {
          "camera_angles": "-2.0 -135.0 0.0",
          "camera_offset": "18.0 3.7 -2.5",
          "camera_fov": 35,
          "override_default_light": 1,
          "spot_light_key": {
            "position": "-120 120 180",
            "color": "2 2.1 2.3",
            "lookat": "0.0 0.0 0.0",
            "inner_cone": 0.5,
            "outer_cone": 1
          },
          "spot_light_rim": {
            "position": "10.0 -90.0 -60.0",
            "color": "3 5 5",
            "lookat": "0.0 0.0 0.0",
            "inner_cone": 0.04,
            "outer_cone": 0.5
          }
        },
        "paint_data": {
          "PaintableMaterial0": {
            "Name": "rif_ak47",
            "OrigMat": "ak47",
            "ViewmodelDim": 2048,
            "WorldDim": 512,
            "BaseTextureOverride": 0,
            "WeaponLength": 37.7462,
            "UVScale": 0.549
          }
        },
        "visuals": {
          "muzzle_flash_effect_1st_person": "weapon_muzzle_flash_assaultrifle",
          "muzzle_flash_effect_3rd_person": "weapon_muzzle_flash_assaultrifle",
          "heat_effect": "weapon_muzzle_smoke",
          "addon_location": "primary_rifle",
          "eject_brass_effect": "weapon_shell_casing_rifle",
          "tracer_effect": "weapon_tracers_assrifle",
          "weapon_type": "Rifle",
          "player_animation_extension": "ak",
          "primary_ammo": "BULLET_PLAYER_762MM",
          "sound_single_shot": "Weapon_AK47.Single",
          "sound_nearlyempty": "Default.nearlyempty"
        },
        "item_type_name": "#CSGO_Type_Rifle",
        "flexible_loadout_category": "rifle",
        "flexible_loadout_group": "rifle",
        "inv_group_equipment": "rifle",
        "mouse_pressed_sound": "sounds/weapons/m4a1/m4a1_clipout.vsnd",
        "drop_sound": "sounds/weapons/m4a1/m4a1_clipin.vsnd",
        "taxonomy": {
          "rifle": 1,
          "primary": 1,
          "weapon": 1,
          "self_damage_on_miss__inflicts_damage": 1
        },
        "item_gear_slot": "primary",
        "item_gear_slot_position": 0,
        "capabilities": {
          "nameable": 1,
          "paintable": 1,
          "can_stattrack_swap": 1,
          "can_sticker": 1,
          "can_keychain": 1
        },
        "craft_class": "weapon",
        "craft_material_type": "weapon",
        "min_ilevel": 1,
        "max_ilevel": 1,
        "image_inventory_size_w": 128,
        "image_inventory_size_h": 82,
        "stickers": "weapon",
        "keychains": "single",
        "hidden": 1
      },
      "paint_kit_def": {
        "name": "cu_ak47_cobra",
        "use_legacy_model": 1,
        "description_string": "#PaintKit_cu_awp_cobra",
        "description_tag": "#PaintKit_cu_awp_cobra_tag",
        "style": 7,
        "wear_remap_min": 0.1,
        "wear_remap_max": 0.7,
        "wear_gradient": "canvas",
        "wear_default": 0.1,
        "seed": 0
      }
    }
  }
}
```
