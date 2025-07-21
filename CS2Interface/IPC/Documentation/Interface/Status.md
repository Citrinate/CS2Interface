# GET /Api/CS2Interface/{botNames}/Status

## Description

Gets the CS2 interface status for the given `botNames`

## Path Parameters

Name | Required | Description
--- | --- | ---
`botNames` | Yes | One or more ASF [bot names](https://github.com/JustArchiNET/ArchiSteamFarm/wiki/Commands#bots-argument)

## Query Parameters

Name | Required | Description
--- | --- | ---
`refreshAutoStop` | No | If set to `true` the auto-stop timer will be refreshed for any of `botNames` that had their inteface started with an `autoStop` value

## Response Result

Property | Type | Description
--- | --- | ---
`botName.Connected` | `bool` | True if the interface is connected for `botName`
`botName.Connecting` | `bool` | True if the interface is attempting to connect for `botName`
`botName.InventoryLoaded` | `bool` | True if the inventory is loaded for `botName`
`botName.InventorySize` | `int` | When `InventoryLoaded`, the number of items in the inventory of `botName`
`botName.UnprotectedInventorySize` | `int` | When `InventoryLoaded`, the number of items in the inventory of `botName` that are not trade protected
`botName.AutoStopAt` | `string` | When the inteface is started with an `autoStop` value for `botName`, contains a date and time string in ISO 8601 format representing when the interface will auto-stop
`botName.Message` | `string` | A description of the status for `botName`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/Status
```

```json
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "Bot1": {
      "Connected": true,
      "Connecting": false,
      "InventoryLoaded": true,
      "InventorySize": 3,
      "UnprotectedInventorySize": 3,
      "AutoStopAt": "2025-06-09T19:25:50.3509119Z",
      "Message": "CS2 Interface is connected and will auto-stop in 14.33 minutes"
    }
  }
}
```
