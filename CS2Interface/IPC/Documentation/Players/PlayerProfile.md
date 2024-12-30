# GET /Api/CS2Interface/{botName}/PlayerProfile/{steamID}

## Description

Get the CS2 player profile for `steamID`, a friend of `botName`

## Path Parameters

Name | Required | Description
--- | --- | ---
`botName` | Yes | An ASF bot name
`steamID` | No | A 64-bit SteamID for an account on the friends list of `botName`.  If missing, the player profile for `botName` will be returned instead

## Query Parameters

None

## Response Result

The details of this result are defined by [`CMsgGCCStrike15_v2_PlayersProfile`](https://github.com/SteamDatabase/Protobufs/blob/master/csgo/cstrike15_gcmessages.proto)

> [!NOTE]
> This request will fail and return a message of "Request timed out" if the account with `steamID` is not a friend of `botName`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/PlayerProfile/76561197960287930
```

```json
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "account_profiles": [
      {
        "account_id": 22202,
        "ongoingmatch": null,
        "global_stats": null,
        "ranking": {
          "account_id": 22202,
          "rank_id": 0,
          "wins": 562,
          "rank_type_id": 6,
          "per_map_rank": []
        },
        "commendation": {
          "cmd_friendly": 66,
          "cmd_teaching": 50,
          "cmd_leader": 55
        },
        "medals": {
          "display_items_defidx": [
            6020,
            4906,
            996,
            6018,
            1376,
            970,
            6014,
            6016,
            1357,
            922,
            6009,
            1336,
            1024,
            1028,
            1331,
            1327
          ],
          "featured_display_item_defidx": 6020
        },
        "my_current_event": null,
        "my_current_event_teams": [],
        "my_current_team": null,
        "my_current_event_stages": [],
        "activity": null,
        "player_level": 21,
        "player_cur_xp": 327682418,
        "rankings": [
          {
            "account_id": 22202,
            "rank_id": 0,
            "wins": 2,
            "rank_type_id": 7,
            "per_map_rank": []
          },
          {
            "account_id": 22202,
            "rank_id": 0,
            "wins": 2,
            "rank_type_id": 10,
            "per_map_rank": []
          },
          {
            "account_id": 22202,
            "rank_id": 0,
            "wins": 3,
            "rank_type_id": 11,
            "rank_window_stats": 16777488,
            "per_map_rank": []
          }
        ]
      }
    ]
  }
}
```
