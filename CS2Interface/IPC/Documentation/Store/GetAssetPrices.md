# GET /Api/CS2Interface/{botNames}/GetAssetPrices/

Get prices and categories for items that users are able to purchase

> [!NOTE]
> This API is just a proxy for Steam's [ISteamEconomy/GetAssetPrices](https://partner.steamgames.com/doc/webapi/ISteamEconomy#GetAssetPrices) API, but with simplified query parameters

## Path Parameters

Name | Required | Description
--- | --- | ---
`botNames` | Yes | One or more ASF [bot names](https://github.com/JustArchiNET/ArchiSteamFarm/wiki/Commands#bots-argument)

> [!NOTE]
> Responses are not dependent on the account. You may provide multiple `botNames`, and the first available bot will be used to make the request.

## Query Parameters

Name | Required | Description
--- | --- | ---
`appID` | No | The app ID of the game to get the purchasable items of.  Assumed to be Counter-Strike 2 if not provided
`currency` | No | A 3 letter currency code may be provided, and the results will be filtered to only show prices of that currency

## Response Result

The response result is simply just the response from Steam's [ISteamEconomy/GetAssetPrices](https://partner.steamgames.com/doc/webapi/ISteamEconomy#GetAssetPrices) API

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/asf/GetAssetPrices
```

```javascript
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "result": {
      "success": true,
      "assets": [
        {
          "prices": {
            "USD": 199,
            "GBP": 148,
            "EUR": 175,
            "RUB": 16200,
            "BRL": 1049,
            "Unknown": 0,
            "JPY": 31000,
            "NOK": 2000,
            "IDR": 3249900,
            "MYR": 845,
            "PHP": 11200,
            "SGD": 265,
            "THB": 6500,
            "VND": 5300000,
            "KRW": 275000,
            "TRY": 0,
            "UAH": 8000,
            "MXN": 3799,
            "CAD": 269,
            "AUD": 305,
            "NZD": 335,
            "PLN": 755,
            "CHF": 165,
            "AED": 750,
            "CLP": 190000,
            "CNY": 1400,
            "COP": 750000,
            "PEN": 675,
            "SAR": 750,
            "TWD": 5900,
            "HKD": 1560,
            "ZAR": 3595,
            "INR": 17000,
            "ARS": 0,
            "CRC": 101000,
            "ILS": 650,
            "KWD": 60,
            "QAR": 729,
            "UYU": 8100,
            "KZT": 105000,
            "BYN": 0
          },
          "name": "1200",
          "date": "2013/08/28",
          "class": [
            {
              "name": "def_index",
              "value": "1200"
            }
          ],
          "classid": "191923205"
        },
        {
          "prices": {
            "USD": 199,
            "GBP": 148,
            "EUR": 175,
            "RUB": 16200,
            "BRL": 1049,
            "Unknown": 0,
            "JPY": 31000,
            "NOK": 2000,
            "IDR": 3249900,
            "MYR": 845,
            "PHP": 11200,
            "SGD": 265,
            "THB": 6500,
            "VND": 5300000,
            "KRW": 275000,
            "TRY": 0,
            "UAH": 8000,
            "MXN": 3799,
            "CAD": 269,
            "AUD": 305,
            "NZD": 335,
            "PLN": 755,
            "CHF": 165,
            "AED": 750,
            "CLP": 190000,
            "CNY": 1400,
            "COP": 750000,
            "PEN": 675,
            "SAR": 750,
            "TWD": 5900,
            "HKD": 1560,
            "ZAR": 3595,
            "INR": 17000,
            "ARS": 0,
            "CRC": 101000,
            "ILS": 650,
            "KWD": 60,
            "QAR": 729,
            "UYU": 8100,
            "KZT": 105000,
            "BYN": 0
          },
          "name": "1201",
          "date": "2019/11/25",
          "class": [
            {
              "name": "def_index",
              "value": "1201"
            }
          ],
          "classid": "3604678661"
        },
        // ...
      ]
    }
  }
}
```
