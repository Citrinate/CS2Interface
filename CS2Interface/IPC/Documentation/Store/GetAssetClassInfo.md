# GET /Api/CS2Interface/{botNames}/GetAssetClassInfo/

Get item details for items specified by their classIDs

> [!NOTE]
> This API is just a proxy for Steam's [ISteamEconomy/GetAssetClassInfo](https://partner.steamgames.com/doc/webapi/ISteamEconomy#GetAssetClassInfo) API, but with simplified query parameters

## Path Parameters

Name | Required | Description
--- | --- | ---
`botNames` | Yes | One or more ASF [bot names](https://github.com/JustArchiNET/ArchiSteamFarm/wiki/Commands#bots-argument)

> [!NOTE]
> Responses are not dependent on the account. You may provide multiple `botNames`, and the first available bot will be used to make the request.

## Query Parameters

Name | Required | Description
--- | --- | ---
`classIDs` | Yes | A comma separated list of classIDs.  There's no limit to the number of classIDs that can be requested, or if there is you'll get a "RequestUriTooLong" error before you reach it.  If you'd like to also include instanceIDs, you can format an individual element of the list as such: `classID_instanceID`
`appID` | No | The appID for the game the `classIDs` belong to.  Assumed to be Counter-Strike 2 if not provided
`language` | No | A 2 letter language code that any strings in the result will appear in

## Response Result

The response result is simply just the response from Steam's [ISteamEconomy/GetAssetClassInfo](https://partner.steamgames.com/doc/webapi/ISteamEconomy#GetAssetClassInfo) API

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/asf/GetAssetClassInfo?classIDs=191923205,3604678661
```

```javascript
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "success": true,
    "result": {
      "191923205": {
        "appid": "730",
        "classid": "191923205",
        "instanceid": "0",
        "icon_url": "i0CoZ81Ui0m-9KwlBY1L_18myuGuq1wfhWSaZgMttyVfPaERSR0Wqmu7LAocGJG51EejH-LVxIT5Z3jCtwYt9YLw7F--F0ipzcG3pCQJ66f_bqA7c6GSCzfGwO9047BvATm9ks8vFHUO",
        "icon_url_large": "",
        "icon_drag_url": "",
        "name": "Name Tag",
        "market_hash_name": "Name Tag",
        "market_name": "Name Tag",
        "name_color": "b0c3d9",
        "background_color": "393b3e",
        "type": "Base Grade Tag",
        "tradable": "1",
        "marketable": "1",
        "commodity": "1",
        "market_tradable_restriction": "7",
        "market_marketable_restriction": "7",
        "descriptions": {
          "0": {
            "type": "html",
            "value": "This item will rename a weapon. A custom engraved nameplate will be applied to the weapon and viewable in game.",
            "name": "description"
          },
          "1": {
            "type": "html",
            "value": " ",
            "name": "blank"
          },
          "2": {
            "type": "html",
            "value": "",
            "color": "00a000",
            "name": "attribute"
          }
        },
        "owner_descriptions": "",
        "actions": {
          "0": {
            "type": "inspect",
            "name": "Inspect in Game...",
            "link": "steam://rungame/730/76561202255233023/+csgo_econ_action_preview%20S%owner_steamid%A%assetid%D12433566796270994112"
          }
        },
        "market_actions": {
          "0": {
            "type": "inspect",
            "name": "Inspect in Game...",
            "link": "steam://rungame/730/76561202255233023/+csgo_econ_action_preview%20M%listingid%A%assetid%D12433566796270994112"
          }
        },
        "tags": {
          "0": {
            "internal_name": "CSGO_Tool_Name_TagTag",
            "name": "Tag",
            "category": "Type",
            "category_name": "Type"
          },
          "1": {
            "internal_name": "normal",
            "name": "Normal",
            "category": "Quality",
            "category_name": "Category"
          },
          "2": {
            "internal_name": "Rarity_Common",
            "name": "Base Grade",
            "category": "Rarity",
            "color": "b0c3d9",
            "category_name": "Quality"
          }
        }
      },
      "3604678661": {
        "appid": "730",
        "classid": "3604678661",
        "instanceid": "0",
        "icon_url": "i0CoZ81Ui0m-9KwlBY1L_18myuGuq1wfhWSaZgMttyVfPaERSR0Wqmu7LAocGJG51EejH_XV0MGkITXE5AB094KtuwG0Exv1yMfkqXcCtvT_MPw5JPTKV2bDk7Z3sudtHSjr2w0ptCMWPT2u",
        "icon_url_large": "",
        "icon_drag_url": "",
        "name": "Storage Unit",
        "market_hash_name": "Storage Unit",
        "market_name": "Storage Unit",
        "name_color": "b0c3d9",
        "background_color": "393b3e",
        "type": "Base Grade Tool",
        "tradable": "0",
        "marketable": "0",
        "owner_only": "1",
        "commodity": "1",
        "market_tradable_restriction": "7",
        "market_marketable_restriction": "7",
        "descriptions": {
          "0": {
            "type": "html",
            "value": "The Storage Unit allows you to store up to 1,000 of your surplus items which would otherwise exceed inventory limit. At any time you can move items from your inventory into the Storage Unit, retrieve items back into your inventory, or you can just use it to organize your collectibles.",
            "name": "description"
          },
          "1": {
            "type": "html",
            "value": " ",
            "name": "blank"
          },
          "2": {
            "type": "html",
            "value": "",
            "color": "00a000",
            "name": "attribute"
          },
          "3": {
            "type": "html",
            "value": " ",
            "name": "blank"
          },
          "4": {
            "type": "html",
            "value": "",
            "name": "attribute"
          }
        },
        "owner_descriptions": "",
        "actions": {
          "0": {
            "type": "inspect",
            "name": "Inspect in Game...",
            "link": "steam://rungame/730/76561202255233023/+csgo_econ_action_preview%20S%owner_steamid%A%assetid%D6930058704473099977"
          }
        },
        "market_actions": {
          "0": {
            "type": "inspect",
            "name": "Inspect in Game...",
            "link": "steam://rungame/730/76561202255233023/+csgo_econ_action_preview%20M%listingid%A%assetid%D6930058704473099977"
          }
        },
        "tags": {
          "0": {
            "internal_name": "CSGO_Type_Tool",
            "name": "Tool",
            "category": "Type",
            "category_name": "Type"
          },
          "1": {
            "internal_name": "normal",
            "name": "Normal",
            "category": "Quality",
            "category_name": "Category"
          },
          "2": {
            "internal_name": "Rarity_Common",
            "name": "Base Grade",
            "category": "Rarity",
            "color": "b0c3d9",
            "category_name": "Quality"
          }
        }
      }
    }
  }
}
```
