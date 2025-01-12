# GET /Api/CS2Interface/{botNames}/Status

## Description

Gets the CS2 interface status for the given `botNames`

## Path Parameters

Name | Required | Description
--- | --- | ---
`botNames` | Yes | One or more ASF [bot names](https://github.com/JustArchiNET/ArchiSteamFarm/wiki/Commands#bots-argument)

## Query Parameters

None

## Response Result

Property | Type | Description
--- | --- | ---
`botName.Connected` | `bool` | True if the interface is connected for `botName`
`botName.Connecting` | `bool` | True if the interface is attempting to connect for `botName`
`botName.Ready` | `bool` | True if the interface is currently able to handle a new request for `botName`
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
      "Ready": true,
      "Message": "Ready"
    }
  }
}
```