# POST /Api/CS2Interface/{botName}/RedeemFreeReward

## Description

Redeems weekly care package reward items for the given `botName`

## Path Parameters

Name | Required | Description
--- | --- | ---
`botName` | Yes | The ASF bot name that will redeem weekly rewards

## Query Parameters

Name | Required | Description
--- | --- | ---
`itemIDs` | Yes | A comma separated list of item IDs to redeem

> [!NOTE]
> Available redeemable item IDs can be found using the [Inventory](/CS2Interface/IPC/Documentation/Items/Inventory.md) API.
> They are distinguished by having `"attributes": { "free reward status": 1 }`

## Response Result

Property | Type | Description
--- | --- | ---
`item_id` | `array<ulong>` | Item IDs related to the notification from the game coordinator
`request` | `uint` | Unknown. Should alway be 9219
`extra_data` | `array<ulong> ` | Unknown. Should alway be empty

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/RedeemFreeReward?itemIDs=50834586538,50834586537
```

```javascript
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "item_id": [
      50851441882,
      50851441883
    ],
    "request": 9219,
    "extra_data": []
  }
}
```

---

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/RedeemFreeReward?itemIDs=50834586538,50834586537
```

```
{
  "Message": "Too many weekly reward inputs given: 2, but only 1 are allowed",
  "Success": false
}
```
