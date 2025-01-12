# GET /Api/CS2Interface/Recipes

## Description

Get a list of crafting recipes currently in the game

## Path Parameters

None

## Query Parameters

Name | Required | Description
--- | --- | ---
`showDefs` | No | If set to true, the response will include a `def` property containing additional game data

## Response Result

Property | Type | Description
--- | --- | ---
`ClientVersion` | `uint` | The version of CS2 that this data comes from
`Data.id` | `ushort` | The recipe ID
`Data.name` | `string` | The name of the crafting recipe
`Data.inputs` | `string` | A description of the inputs
`Data.outputs` | `string` | A description of the outputs
`Data.quality` | `string` | The quality of the inputs and outputs (Unique or StatTrak™)
`Data.def`| `object` | Related game data found in [items_game.txt](/CS2Interface/IPC/Documentation/Data/ItemsGameTxt.md) under `recipes`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/Recipes
```

```json
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "ClientVersion": 2000468,
    "Data": [
      {
        "id": 0,
        "name": "Trade Up Consumer-Grade Weapons",
        "inputs": "Requires: 10 Consumer-Grade Weapons",
        "outputs": "Produces: 1 Industrial-Grade Weapon",
        "quality": "Unique"
      },
      {
        "id": 1,
        "name": "Trade Up Industrial-Grade Weapons",
        "inputs": "Requires: 10 Industrial-Grade Weapons",
        "outputs": "Produces: 1 Mil-Spec Weapon",
        "quality": "Unique"
      },
      {
        "id": 2,
        "name": "Trade Up Mil-Spec Weapons",
        "inputs": "Requires: 10 Mil-Spec Weapons",
        "outputs": "Produces: 1 Restricted Weapon",
        "quality": "Unique"
      },
      {
        "id": 3,
        "name": "Trade Up Restricted Weapons",
        "inputs": "Requires: 10 Restricted Weapons",
        "outputs": "Produces: 1 Classified Weapon",
        "quality": "Unique"
      },
      {
        "id": 4,
        "name": "Trade Up Classified Weapons",
        "inputs": "Requires: 10 Classified Weapons",
        "outputs": "Produces: 1 Covert Weapon",
        "quality": "Unique"
      },
      {
        "id": 10,
        "name": "Trade Up Consumer-Grade Weapons",
        "inputs": "Requires: 10 Consumer-Grade Weapons",
        "outputs": "Produces: 1 Industrial-Grade Weapon",
        "quality": "StatTrak™"
      },
      {
        "id": 11,
        "name": "Trade Up Industrial-Grade Weapons",
        "inputs": "Requires: 10 Industrial-Grade Weapons",
        "outputs": "Produces: 1 Mil-Spec Weapon",
        "quality": "StatTrak™"
      },
      {
        "id": 12,
        "name": "Trade Up Mil-Spec Weapons",
        "inputs": "Requires: 10 Mil-Spec Weapons",
        "outputs": "Produces: 1 Restricted Weapon",
        "quality": "StatTrak™"
      },
      {
        "id": 13,
        "name": "Trade Up Restricted Weapons",
        "inputs": "Requires: 10 Restricted Weapons",
        "outputs": "Produces: 1 Classified Weapon",
        "quality": "StatTrak™"
      },
      {
        "id": 14,
        "name": "Trade Up Classified Weapons",
        "inputs": "Requires: 10 Classified Weapons",
        "outputs": "Produces: 1 Covert Weapon",
        "quality": "StatTrak™"
      }
    ]
  }
}
```
