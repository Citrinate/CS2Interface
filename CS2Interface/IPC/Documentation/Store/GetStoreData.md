# GET /Api/CS2Interface/{botNames}/GetStoreData/

Get information about the items currently available for purchase in the in-game store

> [!NOTE]
> An alternate source for this information would be Steam's [ISteamEconomy/GetAssetPrices](https://partner.steamgames.com/doc/webapi/ISteamEconomy#GetAssetPrices) and [ISteamEconomy/GetAssetClassInfo](https://partner.steamgames.com/doc/webapi/ISteamEconomy#GetAssetClassInfo) APIs.

> [!NOTE]
> Each bot can only process 1 `GetStoreData` request at a time.

## Path Parameters

Name | Required | Description
--- | --- | ---
`botNames` | Yes | One or more ASF [bot names](https://github.com/JustArchiNET/ArchiSteamFarm/wiki/Commands#bots-argument)

> [!NOTE]
> Responses are not dependent on the account. You may provide multiple `botNames`, and the first available bot will be used to make the request.

## Query Parameters

Name | Required | Description
--- | --- | ---
`showDefs` | No | If set to true, the response will include a `price_sheet_items[].defs` property containing additional game data

## Response Result

Property | Type | Description
--- | --- | ---
`result` | `int` | 
`price_sheet_version` | `uint` | Timestamp for when this data was last updated
`price_sheet.store_banner_layout` | `object` | 
`price_sheet.store_metadata` | `object` | 
`price_sheet.entries` | `object` | A list of items in the in-game shop
`price_sheet.entries[].item_link` | `string` | The item's name id
`price_sheet.entries[].prices` | `object` | The original price of the item
`price_sheet.entries[].sale_prices` | `object` | If the item is on sale, the sale price of the item
`price_sheet.currencies` | `object` | 
`price_sheet_items` | `object` | Additional information about the items in `price_sheet.entries`, using `price_sheet.entries[].item_link` as an index
`price_sheet_items[].def_index` | `uint` | The item's definition index
`price_sheet_items[].name` | `string` | The name of the item
`price_sheet_items[].tournament_id` | `uint` | For items related to a tournament, the `eventid` of that tournament
`price_sheet_items[].defs.item_def` | `object` | Related game data found in [items_game.txt](/CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `items`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/asf/GetStoreData
```

```javascript
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "result": 1,
    "price_sheet_version": 1758848888,
    "price_sheet": {
      "store_banner_layout": {
        "1200": {
          "custom_format": "single"
        },
        "4288": {
          "custom_format": "double",
          "market_link": 1
        },
        "4950": {
          "custom_format": "single",
          "perfectworld": "end"
        },
        "7012": {
          "custom_format": "coupon",
          "w": 0.44541994
        },
        // ...
      },
      "store_metadata": {
        "categories": {
          "Misc": {
            "label_token": "#Store_Misc",
            "home": 1,
            "default": 1
          }
        }
      },
      "entries": {
        "Name Tag": {
          "item_link": "Name Tag",
          "category_tags": "Misc",
          "prices": {
            "GBP": 148,
            "EUR": 175,
            "RUB": 16200,
            "BRL": 1139,
            "JPY": 28500,
            "NOK": 2000,
            "IDR": 3249900,
            "MYR": 845,
            "PHP": 11200,
            "SGD": 265,
            "THB": 6500,
            "VND": 4950000,
            "KRW": 275000,
            "UAH": 8000,
            "MXN": 3799,
            "CAD": 269,
            "AUD": 305,
            "NZD": 335,
            "CNY": 1400,
            "TWD": 5900,
            "HKD": 1560,
            "INR": 17000,
            "AED": 750,
            "SAR": 750,
            "ZAR": 3595,
            "COP": 807000,
            "PEN": 715,
            "CLP": 190000,
            "CHF": 165,
            "CRC": 101000,
            "ILS": 695,
            "KZT": 105000,
            "KWD": 60,
            "PLN": 755,
            "QAR": 729,
            "UYU": 8100,
            "USD": 199
          }
        },
        "casket": {
          "item_link": "casket",
          "category_tags": "Misc",
          "prices": {
            "GBP": 148,
            "EUR": 175,
            "RUB": 16200,
            "BRL": 1139,
            "JPY": 28500,
            "NOK": 2000,
            "IDR": 3249900,
            "MYR": 845,
            "PHP": 11200,
            "SGD": 265,
            "THB": 6500,
            "VND": 4950000,
            "KRW": 275000,
            "UAH": 8000,
            "MXN": 3799,
            "CAD": 269,
            "AUD": 305,
            "NZD": 335,
            "CNY": 1400,
            "TWD": 5900,
            "HKD": 1560,
            "INR": 17000,
            "AED": 750,
            "SAR": 750,
            "ZAR": 3595,
            "COP": 807000,
            "PEN": 715,
            "CLP": 190000,
            "CHF": 165,
            "CRC": 101000,
            "ILS": 695,
            "KZT": 105000,
            "KWD": 60,
            "PLN": 755,
            "QAR": 729,
            "UYU": 8100,
            "USD": 199
          }
        },
        "tournament_pass_aus2025_charge": {
          "item_link": "tournament_pass_aus2025_charge",
          "category_tags": "Misc",
          "prices": {
            "GBP": 219,
            "EUR": 265,
            "RUB": 24500,
            "BRL": 1699,
            "JPY": 42700,
            "NOK": 3000,
            "IDR": 4879900,
            "MYR": 1275,
            "PHP": 16800,
            "SGD": 395,
            "THB": 9700,
            "VND": 7450000,
            "KRW": 415000,
            "UAH": 12000,
            "MXN": 5699,
            "CAD": 409,
            "AUD": 460,
            "NZD": 499,
            "CNY": 2100,
            "TWD": 9000,
            "HKD": 2300,
            "INR": 25500,
            "AED": 1100,
            "SAR": 1125,
            "ZAR": 5349,
            "COP": 1213000,
            "PEN": 1075,
            "CLP": 290000,
            "CHF": 245,
            "CRC": 152000,
            "ILS": 1035,
            "KZT": 157000,
            "KWD": 90,
            "PLN": 1130,
            "QAR": 1099,
            "UYU": 12200,
            "USD": 299
          }
        },
        "crate_sticker_pack_aus2025_legends": {
          "item_link": "crate_sticker_pack_aus2025_legends",
          "category_tags": "Misc",
          "prices": {
            "GBP": 75,
            "EUR": 89,
            "RUB": 8100,
            "BRL": 575,
            "JPY": 14200,
            "NOK": 1000,
            "IDR": 1619900,
            "MYR": 420,
            "PHP": 5500,
            "SGD": 130,
            "THB": 3200,
            "VND": 2450000,
            "KRW": 137000,
            "UAH": 4000,
            "MXN": 1899,
            "CAD": 135,
            "AUD": 155,
            "NZD": 165,
            "CNY": 700,
            "TWD": 3000,
            "HKD": 760,
            "INR": 8400,
            "AED": 375,
            "SAR": 375,
            "ZAR": 1795,
            "COP": 402000,
            "PEN": 360,
            "CLP": 95000,
            "CHF": 80,
            "CRC": 51000,
            "ILS": 345,
            "KZT": 52000,
            "KWD": 30,
            "PLN": 375,
            "QAR": 359,
            "UYU": 4100,
            "USD": 99
          },
          "sale_prices": {
            "GBP": 18,
            "EUR": 25,
            "RUB": 2100,
            "BRL": 145,
            "JPY": 3600,
            "NOK": 250,
            "IDR": 409900,
            "MYR": 110,
            "PHP": 1400,
            "SGD": 35,
            "THB": 800,
            "VND": 650000,
            "KRW": 35000,
            "UAH": 1000,
            "MXN": 499,
            "CAD": 35,
            "AUD": 40,
            "NZD": 42,
            "CNY": 200,
            "TWD": 800,
            "HKD": 200,
            "INR": 2200,
            "AED": 100,
            "SAR": 95,
            "ZAR": 449,
            "COP": 102000,
            "PEN": 90,
            "CLP": 24000,
            "CHF": 20,
            "CRC": 13000,
            "ILS": 90,
            "KZT": 14000,
            "KWD": 8,
            "PLN": 95,
            "QAR": 89,
            "UYU": 1100,
            "USD": 25
          }
        },
        // ...
      },
      "currencies": {
        "USD": 88888,
        "GBP": 65709,
        "EUR": 76767,
        "RUB": 6977652,
        "BRL": 489302,
        "JPY": 12990544,
        "NOK": 896694,
        "IDR": 1460631848,
        "MYR": 381774,
        "PHP": 5090393,
        "SGD": 114171,
        "THB": 2909660,
        "VND": 2329960842,
        "KRW": 121898672,
        "UAH": 3722267,
        "MXN": 1701795,
        "CAD": 122089,
        "AUD": 137671,
        "NZD": 148748,
        "PLN": 328093,
        "CHF": 72164,
        "AED": 326434,
        "CLP": 84310268,
        "CNY": 638171,
        "COP": 360414622,
        "PEN": 319841,
        "SAR": 333523,
        "TWD": 2634107,
        "HKD": 697792,
        "ZAR": 1589437,
        "INR": 7676023,
        "CRC": 44874017,
        "ILS": 308404,
        "KWD": 27193,
        "QAR": 323597,
        "UYU": 3604362,
        "KZT": 46392889
      }
    },
    "price_sheet_items": {
      "Name Tag": {
        "def_index": 1200,
        "name": "Name Tag"
      },
      "casket": {
        "def_index": 1201,
        "name": "Storage Unit"
      },
      "tournament_pass_aus2025_charge": {
        "def_index": 5116,
        "name": "Austin 2025 Souvenir Package",
        "tournament_id": 24
      },
      "crate_sticker_pack_aus2025_legends": {
        "def_index": 5117,
        "name": "Austin 2025 Legends Sticker Capsule",
        "tournament_id": 24
      },
      // ...
    }
  }
}
```
