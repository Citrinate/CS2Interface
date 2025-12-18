# POST /Api/CS2Interface/{botName}/StoreItem/{crateID}/{itemID}

## Description

Stores an item `itemID` into storage unit `crateID` owned by `botName`

## Path Parameters

Name | Required | Description
--- | --- | ---
`botName` | Yes | The ASF bot name for the owner of the storage unit
`crateID` | Yes | The storage unit's item ID
`itemID` | Yes | The item's ID

## Query Parameters

None

## Response Result

None

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/StoreItem/31555237276/41020280968
```

```javascript
{
  "Message": "OK",
  "Success": true
}
```
