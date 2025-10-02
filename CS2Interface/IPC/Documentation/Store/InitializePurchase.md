# GET /Api/CS2Interface/{botName}/InitializePurchase

## Description

Begin the process of purchasing an item from the in-game store

> [!NOTE]
> Each bot can only process 1 `InitializePurchase` request at a time.

## Path Parameters

Name | Required | Description
--- | --- | ---
`botName` | Yes | The name of the ASF bot this purchase will be completed by

## Query Parameters

Name | Required | Description
--- | --- | ---
`itemID` | Yes | The definition index for the item you want to purchase
`quantity` | Yes | Number of copies to purchase.  Maximum value of `20`
`cost` | Yes | The total price (before taxes) for `quantity` items in the currency used by `botName` on the Steam store.  This value should be an integer representing an amount in that currency’s lowest unit (ex: 250 cents, not 2.50 dollars).  If the item is on sale, the sale price is used instead of the base price
`supplementalData` | No | Additional data needed for certain values of `itemID`.  Souvenir Packages for example use the `matchid` of a tournament match to determine the kind of Souvenir Package

> [!NOTE]
> `itemID` and `cost` can be determined using the [GetStoreData](/CS2Interface/IPC/Documentation/Store/GetStoreData.md) API.  `supplementalData` can be determined using the [GetTournamentInfo](/CS2Interface/IPC/Documentation/Matches/GetTournamentInfo.md) API.

## Response Result

Property | Type | Description
--- | --- | ---
`OrderDetails.lineitems[].description` | `string` | The name of the item to be purchased
`OrderDetails.lineitems[].gameitemid` | `long` | The item's definition index
`OrderDetails.lineitems[].amount` | `long` | The total price for the specified quantity of this item (before taxes)
`OrderDetails.lineitems[].quantity` | `integer` | The quantity of this item to be purchased
`OrderDetails.transid` | `long` | A unique id used for the purchase by the Steam store
`OrderDetails.orderid` | `long` | A unique id used for the purchase by the CS2 game coordinator
`OrderDetails.appid` | `integer` | Counter-Strike 2's appid
`OrderDetails.currency` | `integer` | [ECurrencyCode](https://github.com/SteamRE/SteamKit/blob/3a7c46c92b8a64e765a8753ce8299f5fd2e8ce37/SteamKit2/SteamKit2/Base/Generated/SteamLanguage.cs#L2832), the currency of `OrderDetails.vat`, `OrderDetails.total`, and `OrderDetails.Tax`
`OrderDetails.vat` | `integer` | The total VAT tax for this purchase
`OrderDetails.total` | `long` | The total cost for this purcahse (including taxes)
`OrderDetails.Tax` | `long` | The total tax for this purchase
`OrderDetails.language` | `integer` | 
`OrderDetails.sandbox` | `integer` | 
`OrderDetails.BillingCurrency` | `integer` | [ECurrencyCode](https://github.com/SteamRE/SteamKit/blob/3a7c46c92b8a64e765a8753ce8299f5fd2e8ce37/SteamKit2/SteamKit2/Base/Generated/SteamLanguage.cs#L2832), the currency of `OrderDetails.BillingTotal`
`OrderDetails.BillingTotal` | `long` | The total cost for this purcahse (including taxes)
`OrderDetails.SteamRealm` | `integer` | [ESteamRealm](https://github.com/SteamRE/SteamKit/blob/3a7c46c92b8a64e765a8753ce8299f5fd2e8ce37/Resources/SteamLanguage/enums.steamd#L1644)
`OrderDetails.RequiresCachedPmtMethod` | `integer` | 
`OrderDetails.Refundable` | `integer` | Whether or not the purchase is refundable
`WalletDetails.HasWallet` | `integer` | Whether or not `botName` has a Steam wallet
`WalletDetails.nAmount` | `long` | The balance for the Steam wallet of `botName`
`WalletDetails.eCurrencyCode` | `integer` | [ECurrencyCode](https://github.com/SteamRE/SteamKit/blob/3a7c46c92b8a64e765a8753ce8299f5fd2e8ce37/SteamKit2/SteamKit2/Base/Generated/SteamLanguage.cs#L2832), the Steam wallet balance currency for `botName`
`WalletDetails.nAmountDelayed` | `long` | The delayed balance for the Steam wallet of `botName`
`WalletDetails.eCurrencyCodeDelayed` | `integer` | [ECurrencyCode](https://github.com/SteamRE/SteamKit/blob/3a7c46c92b8a64e765a8753ce8299f5fd2e8ce37/SteamKit2/SteamKit2/Base/Generated/SteamLanguage.cs#L2832), the delayed Steam wallet balance currency for `botName`
`WalletDetails.SteamRealm` | `integer` | [ESteamRealm](https://github.com/SteamRE/SteamKit/blob/3a7c46c92b8a64e765a8753ce8299f5fd2e8ce37/Resources/SteamLanguage/enums.steamd#L1644)
`PurchaseUrl` | `string` | A link to complete the purchase in a web browser (must be logged in as `botName`)

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/InitializePurchase?itemID=5116&quantity=1&cost=299&supplementalData=3759444260701602000
```

```json
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "OrderDetails": {
      "lineitems": {
        "0": {
          "description": "Austin 2025 Souvenir Package",
          "gameitemid": 5116,
          "amount": 299,
          "quantity": 1
        }
      },
      "transid": 353091901589090859,
      "orderid": 2330218620,
      "appid": 730,
      "currency": 1,
      "vat": 0,
      "total": 323,
      "Tax": 24,
      "language": 0,
      "sandbox": 0,
      "BillingCurrency": 1,
      "BillingTotal": 323,
      "SteamRealm": 1,
      "RequiresCachedPmtMethod": 0,
      "Refundable": 1
    },
    "WalletDetails": {
      "HasWallet": 1,
      "nAmount": 1337,
      "eCurrencyCode": 1,
      "nAmountDelayed": 42,
      "eCurrencyCodeDelayed": 1,
      "SteamRealm": 1
    },
    "PurchaseUrl": "https://checkout.steampowered.com/checkout/approvetxn/353091901589090859/?returnurl=https%3A%2F%2Fstore.steampowered.com%2Fbuyitem%2F730%2Ffinalize%2F2330218620%3Fcanceledurl%3Dhttps%253A%252F%252Fstore.steampowered.com%252F%26returnhost%3Dstore.steampowered.com&canceledurl=https%3A%2F%2Fstore.steampowered.com%2F"
  }
}
```

---

The following is an example of an invalid purchase due to incorrect cost (`199` instead of `299`)

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/InitializePurchase?itemID=5116&quantity=1&cost=199&supplementalData=3759444260701602000
```

```json
{
  "Message": "Invalid Purchase",
  "Success": false
}
```