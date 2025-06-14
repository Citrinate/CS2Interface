# GET /Api/CS2Interface/csgo_english.txt

## Description

Get the contents of the CS2 game file: [`resource/csgo_english.txt`](https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/game/csgo/pak01_dir/resource/csgo_english.txt)

## Path Parameters

None

## Query Parameters

None

## Response Result

Property | Type | Description
--- | --- | ---
`ClientVersion` | `uint` | The version of CS2 that this data comes from
`Data` | `object` | The contents of `resource/csgo_english.txt`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/csgo_english.txt
```

```javascript
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "ClientVersion": 2000468,
    "Data": {
      "GameUI_MainMenuMovieScene": "Main Menu Background Scenery",
      "GameUI_MainMenuMovieScene_Vanity": "Change Background Scenery",
      "GameUI_MainMenuMovieScene_Tooltip": "This setting allows you to customize the main menu background scenery which sets the mood for visual experience of your entire game.",
      //...
    }
  }
}
```
