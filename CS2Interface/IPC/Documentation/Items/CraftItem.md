# POST /Api/CS2Interface/{botName}/CraftItem/{recipeID}

## Description

Crafts an item for the given `botName` using the specified crafting `recipeID`

> [!NOTE]
> Each bot can only process 1 `CraftItem` request at a time.

## Path Parameters

Name | Required | Description
--- | --- | ---
`botName` | Yes | The ASF bot name that will be used to craft an item
`recipeID` | Yes | The ID for the crafting recipe

> [!NOTE]
> A list of `recipeIDs` can be found using the [Recipes](/CS2Interface/IPC/Documentation/Data/Recipes.md) API

## Query Parameters

Name | Required | Description
--- | --- | ---
`itemIDs` | Yes | A comma separated list of item IDs to be used in the crafting recipe

## Response Result

Property | Type | Description
--- | --- | ---
`recipe` | `ushort` | Should be the same as `recipeID`
`unknown` | `uint` | Unknown
`itemcount` | `ushort` | The number of items created
`itemids` | `array<ulong>` | The item IDs for each of the newly created items

> [!NOTE]
> Details for the newly crafted items can be found using the [Inventory](/CS2Interface/IPC/Documentation/Items/Inventory.md) API

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/CraftItem/0?itemIDs=41112241128,41112243329,41112241214,41112246101,41112244630,41112248299,41112243430,41112247282,41112248426,41112244626
```

```javascript
{
  "Message": "OK",
  "Success": true,
  "Result": {
    "recipe": 0,
    "unknown": 0,
    "itemcount": 1,
    "itemids": [
      41112277716
    ]
  }
}
```
