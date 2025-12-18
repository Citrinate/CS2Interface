# POST /Api/CS2Interface/{botNames}/Stop

## Description

Stops the CS2 interface for the given `botNames`

## Path Parameters

Name | Required | Description
--- | --- | ---
`botNames` | Yes | One or more ASF [bot names](https://github.com/JustArchiNET/ArchiSteamFarm/wiki/Commands#bots-argument)

## Query Parameters

None

## Response Result

Property | Type | Description
--- | --- | ---
`botName.Message` | `string` | A description of whatever just happened to `botName`
`botName.Success` | `bool` | True if the interface is now not running for `botName`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/Stop
```

```json
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "Bot1": {
      "Message": "CS2 Interface successfully stopped",
      "Success": true
    }
  }
}
```
