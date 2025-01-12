# GET /Api/CS2Interface/items_game.txt

## Description

Get the contents of the CS2 game file: [`scripts/items/items_game.txt`](https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/scripts/items/items_game.txt)

## Path Parameters

None

## Query Parameters

None

## Response Result

Property | Type | Description
--- | --- | ---
`ClientVersion` | `uint` | The version of CS2 that this data comes from
`Data` | `object` | The contents of `scripts/items/items_game.txt`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/items_game.txt
```

```javascript
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "ClientVersion": 2000468,
    "Data": {
      "game_info": {
        "first_valid_class": 2,
        "last_valid_class": 3,
        "first_valid_item_slot": 0,
        "last_valid_item_slot": 54,
        "num_item_presets": 4,
        "max_num_stickers": 5,
        "max_num_patches": 3
      },
      "rarities": {
        "default": {
          "value": 0,
          "loc_key": "Rarity_Default",
          "loc_key_weapon": "Rarity_Default_Weapon",
          "loc_key_character": "Rarity_Default_Character",
          "color": "desc_default",
          "drop_sound": "EndMatch.ItemRevealRarityCommon"
        },
        //...
      },
      "qualities": {
        "normal": {
          "value": 0,
          "weight": 1,
          "hexColor": "#B2B2B2"
        },
        //...
      },
      "colors": {
        "desc_level": {
          "color_name": "ItemAttribLevel",
          "hex_color": "#756b5e"
        },
        //...
      },
      "graffiti_tints": {
        "brick_red": {
          "id": 1,
          "hex_color": "#874444"
        },
        //...
      },
      "player_loadout_slots": {
        "0": "LOADOUT_POSITION_MELEE",
        //...
      },
      "alternate_icons2": {
        "weapon_icons": {
          "65604": {
            "icon_path": "econ/default_generated/weapon_deagle_hy_ddpat_urb_light"
          },
          //...
        },
        //...
      },
      "prefabs": {
        "weapon_supports_keychains": {
          "capabilities": {
            "can_keychain": 1
          },
          "keychains": "single"
        },
        //...
      },
      "items": {
        "1": {
          "name": "weapon_deagle",
          "prefab": "weapon_deagle_prefab",
          "item_quality": "normal",
          "baseitem": 1,
          "flexible_loadout_slot": "secondary4",
          "flexible_loadout_default": 1,
          "item_shares_equip_slot": 1
        },
        //...
      },
      "attributes": {
        "1": {
          "name": "always tradable",
          "attribute_class": "always_tradable",
          "description_format": "value_is_additive",
          "description_string": "#Attrib_AlwaysTradable",
          "hidden": 1,
          "effect_type": "positive",
          "stored_as_integer": 1
        },
        //...
      },
      "sticker_kits": {
        "0": {
          "name": "default",
          "item_name": "#StickerKit_Default",
          "description_string": "#StickerKit_Desc_Default"
        },
        //...
      },
      "paint_kits": {
        "0": {
          "name": "default",
          "description_string": "#PaintKit_Default",
          "description_tag": "#PaintKit_Default_Tag",
          "wear_gradient": "canvas",
          "wear_default": 0.1,
          "wear_remap_min": 0.06,
          "wear_remap_max": 0.8,
          "seed": 0,
          "style": 0
        },
        //...
      },
      "paint_kits_rarity": {
        "so_olive": "common",
        //...
      },
      "item_sets": {
        "set_community_3": {
          "name": "#CSGO_set_community_3",
          "set_description": "#CSGO_set_community_3_desc",
          "is_collection": 1,
          "items": {
            "[cu_tec9_asiimov]weapon_tec9": 1,
            "[cu_ssg08_immortal]weapon_ssg08": 1,
            "[cu_retribution]weapon_elite": 1,
            "[hy_galil_kami]weapon_galilar": 1,
            "[cu_p90_scorpius]weapon_p90": 1,
            "[am_nitrogen]weapon_cz75a": 1,
            "[am_gyrate]weapon_cz75a": 1,
            "[an_royalbleed]weapon_p90": 1,
            "[cu_p2000_pulse]weapon_hkp2000": 1,
            "[cu_aug_progressiv]weapon_aug": 1,
            "[cu_bizon_antique]weapon_bizon": 1,
            "[cu_mac10_decay]weapon_mac10": 1,
            "[cu_xm1014_heaven_guard]weapon_xm1014": 1,
            "[cu_korupt]weapon_mac10": 1,
            "[am_m4a1-s_alloy_orange]weapon_m4a1_silencer": 1,
            "[cu_scar_cyrex]weapon_scar20": 1,
            "[cu_usp_spitfire]weapon_usp_silencer": 1,
            "[cu_kaiman]weapon_usp_silencer": 1,
            "[cu_ak47_rubber]weapon_ak47": 1,
            "[cu_titanstorm]weapon_m4a1": 1
          }
        },
        //...
      },
      "client_loot_lists": {
        "set_nuke_2_common": {
          "[hy_blueprint_white]weapon_bizon": 1,
          "[hy_blueprint_red]weapon_p250": 1,
          "[hy_blueprint_bluered]weapon_ump45": 1,
          "[hy_ducts_green]weapon_fiveseven": 1,
          "[hy_ducts_grey]weapon_nova": 1
        },
        //...
      },
      "revolving_loot_lists": {
        "1": "crate_valve_1",
        //...
      },
      "quest_reward_loot_lists": {
        "1": "set_dust",
        //...
      },
      "item_levels": {
        "KillEaterRank": {
          "0": {
            "score": 2
          },
          //...
        }
      },
      "kill_eater_score_types": {
        "0": {
          "type_name": "Kills",
          "model_attribute": "stattrak model"
        },
        "1": {
          "type_name": "OCMVPs"
        }
      },
      "music_definitions": {
        "1": {
          "name": "valve_cs2_01",
          "loc_name": "#musickit_valve_cs2_01",
          "loc_description": "#musickit_valve_cs2_01_desc",
          "image_inventory": "econ/music_kits/valve_cs2_01",
          "pedestal_display_model": "models/inventory_items/music_kits/music_kit_valve_cs2_01.vmdl"
        },
        //...
      },
      "quest_definitions": {
        "11": {
          "name": "Casual Kills: SMG",
          "loc_name": "#Quest_Weapon_Mode",
          "loc_description": "#Quest_Weapon_Mode_desc",
          "gamemode": "casual",
          "expression": "%weapon_smg% && %act_kill_human%"
        },
        //...
      },
      "campaign_definitions": {
        "1": {
          //...
          "loc_name": "#csgo_campaign_eurasia",
          "loc_description": "#csgo_campaign_eurasia_desc",
          "season_number": 4
        },
        //...
      },
      "quest_schedule": {
        "start": 1632254400,
        "length": "P9W",
        "slack": "P3W"
      },
      "skirmish_modes": {
        "1": {
          "name": "stabstabzap",
          "loc_name": "Skirmish_CC_SSZ_name",
          "loc_rules": "Skirmish_CC_SSZ_rules",
          "loc_description": "Skirmish_CC_SSZ_desc",
          "loc_details": "Skirmish_CC_SSZ_details",
          "icon": "icon-skirmish-ssz",
          "gamemode": "casual",
          "server_exec": "execwithwhitelist op08_stab_stab_zap.cfg"
        },
        //...
      },
      "skirmish_rank_info": {
        "1": {
          "pips": 2,
          "winstreak": 3,
          "loss": 0
        },
        //...
      },
      "recipes": {
        "0": {
          "name": "#RT_MP_A",
          "n_A": "#RI_R1p",
          "desc_inputs": "#RDI_AB",
          "desc_outputs": "#RDO_AB",
          "di_A": 10,
          "di_B": "#RI_R1p",
          "do_A": 1,
          "do_B": "#RI_R2",
          "all_same_class": 0,
          "always_known": 1,
          "premium_only": 0,
          "disabled": 0,
          "input_items": {
            "10": {
              "conditions": {
                "0": {
                  "field": "*rarity",
                  "operator": "string==",
                  "value": "common",
                  "required": 1
                },
                "1": {
                  "field": "*quality",
                  "operator": "string==",
                  "value": "unique",
                  "required": 1
                },
                "2": {
                  "field": "craft_class",
                  "operator": "string==",
                  "value": "weapon",
                  "required": 1
                }
              }
            }
          },
          "output_items": {
            "item1": {
              "conditions": {
                "0": {
                  "field": "*match_set_rarity",
                  "operator": "string==",
                  "value": "uncommon",
                  "required": 1
                }
              }
            }
          },
          "category": "crafting",
          "filter": -3
        },
        //...
      },
      "seasonaloperations": {
        "8": {
          "quest_looping_track": 1,
          "quest_reward": {
            "[0]": {
              "item_name": "CommunitySeasonNine2019 Coin 1",
              "ui_order": 3
            },
            "[*]": {
              "none": "none"
            },
            "item_name": "selfopeningitem_crate_spray_std2_1",
            "ui_order": 3,
            "ui_image": "operations/op9/op9_graffiti_pack_02",
            "ui_image_inspect": "operations/op9/op9_graffiti_pack_02_inspect",
            "ui_image_thumbnail": "operations/op9/op9_graffiti_pack_02_thumbnail",
            "none": "none",
            "callout": "#CSGO_set_norse_short"
          },
          "quest_mission_card": {
            "id": 8001,
            "name": "#UI_Operation09_MissionCard_04",
            "quests": "1030,949-952,1027-1029,953-954",
            "operational_points": 10
          },
          "xp_reward": "5,10,15,20,25,30,35,40,45,50,55,60,65,70,75,80,85,90,95,100"
        },
        //...
      },
      "pro_event_results": {
        "1": {
          "place_names": {
            "1": "#Place_Name_1st",
            "2": "#Place_Name_2nd",
            "3": "#Place_Name_3rd-4th",
            "4": "#Place_Name_5th-8th",
            "5": "#Place_Name_9th-12th",
            "6": "#Place_Name_13th-16th"
          },
          "team_places": {
            "10": 4,
            "11": 5,
            "12": 6,
            "13": 5,
            "14": 6,
            "15": 6,
            "16": 5,
            "06": 1,
            "01": 2,
            "04": 3,
            "03": 3,
            "09": 4,
            "08": 4,
            "02": 4,
            "07": 5,
            "05": 6
          }
        },
        //...
      },
      "pro_players": {
        "36": {
          "name": "Sf",
          "code": "sf",
          "dob": "-",
          "geo": "FR",
          "events": {
            "3": {
              "team": 7
            },
            "4": {
              "team": 35
            }
          }
        },
        //...
      },
      "pro_teams": {
        "1": {
          "tag": "NiP",
          "geo": "SE"
        },
        //...
      },
      "items_game_live": {
        "pro_players": {
          "*": {
            "events": {
              "16": {
                "clutch_kills": 0,
                "pistol_kills": 0,
                "opening_kills": 0,
                "sniper_kills": 0,
                "KDR": 1,
                "enemy_kills": 0,
                "deaths": 0,
                "matches_played": 0,
                "*": {
                  "clutch_kills": 0,
                  "pistol_kills": 0,
                  "opening_kills": 0,
                  "sniper_kills": 0,
                  "KDR": 1,
                  "enemy_kills": 0,
                  "deaths": 0,
                  "matches_played": 0
                }
              }
            }
          }
        }
      },
      "keychain_definitions": {
        "1": {
          "name": "kc_missinglink_ava",
          "loc_name": "#keychain_kc_missinglink_ava",
          "loc_description": "#keychain_kc_missinglink_ava_desc",
          "item_rarity": "rare",
          "image_inventory": "econ/keychains/missinglink/kc_missinglink_ava",
          "pedestal_display_model": "weapons/keychains/missinglink/vmdl/kc_missinglink_ava.vmdl"
        },
        //...
      }
    }
  }
}
```
