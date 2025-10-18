# GET /Api/CS2Interface/{botName}/NameItem

## Description

Adds a name to an item

> [!NOTE]
> Each bot can only process 1 `NameItem` request at a time.

## Path Parameters

Name | Required | Description
--- | --- | ---
`botName` | Yes | The ASF bot name for the owner of the item you want to name

## Query Parameters

Name | Required | Description
--- | --- | ---
`itemID` | Yes | The ID of the item you want to name
`name` | Yes | The name you want to apply to the item
`nameTagID` | No | The ID of the name tag you want to use.  Not needed for items that don't use a Name Tag item to change their name (ex: Storage Units)

> [!IMPORTANT]
> The plugin won't prevent you from using a `name` that would be impossible to enter into the in-game interface.  Do so at your own risk.  Using such names won't trigger VAC bans as the plugin isn't interacting with the game client, but Valve could decide to take manual action against such accounts.

## Response Result

None

## Example Response

```
http://127.0.0.1:1243/Api/CS2Interface/Bot1/NameItem?itemID=31555237276&name=Stickers
```

```javascript
{
  "Message": "OK",
  "Success": true
}
```
