# GET /Api/CS2Interface/{botNames}/Start

## Description

Starts the CS2 interface for the given `botNames`

> [!NOTE]
> This request will run until the interface successfully starts or it encounters an error.  If attempting to start the interface for a bot that already has the interface running, nothing will happen.

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
`botName.Success` | `bool` | True if the interface is now running for `botName`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/Start
```

```json
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "Bot1": {
      "Message": "CS2 Interface started",
      "Success": true
    }
  }
}
```
