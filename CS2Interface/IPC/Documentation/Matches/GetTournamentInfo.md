# GET /Api/CS2Interface/{botNames}/GetTournamentInfo/{eventID}

## Description

Get the match information for the given tournament, `eventID`

> [!NOTE]
> Each bot can only process 1 `GetTournamentInfo` request at a time.

## Path Parameters

Name | Required | Description
--- | --- | ---
`botNames` | Yes | One or more ASF [bot names](https://github.com/JustArchiNET/ArchiSteamFarm/wiki/Commands#bots-argument)

> [!NOTE]
> Responses are not dependent on the account. You may provide multiple `botNames`, and the first available bot will be used to make the request.

## Query Parameters

None

## Response Result

The details of this result are defined by [`CMsgGCCStrike15_v2_MatchList`](https://github.com/SteamDatabase/Protobufs/blob/master/csgo/cstrike15_gcmessages.proto)

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/asf/GetTournamentInfo/24
```

```javascript
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "msgrequestid": 9146,
    "accountid": 24,
    "servertime": 1758448828,
    "matches": [
      {
        "matchid": 3759444260701602000,
        "matchtime": 1750627654,
        "watchablematchinfo": {
          "server_ip": 0,
          "tv_port": 0
        },
        "roundstats_legacy": {
          "reservation": {
            "account_ids": [],
            "game_type": 4104,
            "rankings": [],
            "party_ids": [],
            "whitelist": [],
            "tournament_event": {
              "event_id": 24,
              "event_tag": "AUS25",
              "event_name": "BLAST.tv Austin 2025 CS2 Major Championship",
              "event_time_start": 1746910800,
              "event_time_end": 1750798800,
              "event_public": 1,
              "event_stage_id": 13,
              "event_stage_name": "Match 3 of 3 | Grand Final",
              "active_section_id": 98
            },
            "tournament_teams": [
              {
                "team_id": 122,
                "team_tag": "MNGZ",
                "team_flag": "WORLD",
                "team_name": "The MongolZ",
                "players": []
              },
              {
                "team_id": 89,
                "team_tag": "VITA",
                "team_flag": "FR",
                "team_name": "Vitality",
                "players": []
              }
            ],
            "tournament_casters_account_ids": [],
            "pre_match_data": null,
            "op_var_values": [],
            "teammate_colors": []
          },
          "kills": [],
          "assists": [],
          "deaths": [],
          "scores": [],
          "pings": [],
          "match_result": 2,
          "team_scores": [
            6,
            13
          ],
          "confirm": null,
          "enemy_kills": [],
          "enemy_headshots": [],
          "enemy_3ks": [],
          "enemy_4ks": [],
          "enemy_5ks": [],
          "mvps": [],
          "enemy_kills_agg": [],
          "drop_info": null,
          "enemy_2ks": [],
          "player_spawned": [],
          "team_spawn_count": [],
          "map_id": 5
        },
        "roundstatsall": []
      },
      //...
    ],
    "streams": [],
    "tournamentinfo": {
      "sections": [
        {
          "sectionid": 98,
          "name": "Grand Final",
          "desc": "6",
          "groups": [
            {
              "groupid": 260,
              "name": "GrandFinal",
              "desc": "Final",
              "picks__deprecated": 1,
              "teams": [
                {
                  "team_id": 122,
                  "score": 0
                },
                {
                  "team_id": 89,
                  "score": 0,
                  "correctpick": true
                }
              ],
              "stage_ids": [
                11,
                12,
                13
              ],
              "pickableteams": 2,
              "points_per_pick": 7,
              "picks": [
                {
                  "pickids": [
                    89
                  ]
                }
              ]
            }
          ]
        },
        //...
      ],
      "tournament_event": {
        "event_id": 24,
        "event_tag": "AUS25",
        "event_name": "BLAST.tv Austin 2025 CS2 Major Championship",
        "event_time_start": 1746910800,
        "event_time_end": 1750798800,
        "event_public": 1,
        "active_section_id": 98
      },
      "tournament_teams": [
        {
          "team_id": 89,
          "team_tag": "VITA",
          "team_flag": "FR",
          "team_name": "Vitality",
          "players": []
        },
        {
          "team_id": 122,
          "team_tag": "MNGZ",
          "team_flag": "WORLD",
          "team_name": "The MongolZ",
          "players": []
        },
        //...
      ]
    }
  }
}
```
