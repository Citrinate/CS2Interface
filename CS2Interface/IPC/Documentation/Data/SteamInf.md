# GET /Api/CS2Interface/steam.inf

## Description

Get the contents of the CS2 game file: [`game/csgo/steam.inf`](https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/steam.inf)

## Path Parameters

None

## Query Parameters

None

## Response Result

Property | Type | Description
--- | --- | ---
`ClientVersion` | `uint` | The version of CS2 that this data comes from
`Data` | `object` | The contents of `game/csgo/steam.inf`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/steam.inf
```

```json
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "ClientVersion": 2000468,
    "Data": {
      "ClientVersion": "2000468",
      "ServerVersion": "2000468",
      "PatchVersion": "1.40.5.9",
      "ProductName": "cs2",
      "appID": "730",
      "ServerAppID": "2347773",
      "SourceRevision": "9449802",
      "VersionDate": "Jan 09 2025",
      "VersionTime": "16:42:11"
    }
  }
}
```
