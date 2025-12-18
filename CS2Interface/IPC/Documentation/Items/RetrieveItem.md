# POST /Api/CS2Interface/{botName}/RetrieveItem/{crateID}/{itemID}

## Description

Retrieves an item `itemID` from storage unit `crateID` owned by `botName`

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

> [!NOTE]
> This request will fail and return a message of "Request timed out" if `itemID` is not found in `crateID`

## Example Response

```
http://127.0.0.1:1242/Api/CS2Interface/Bot1/RetrieveItem/31555237276/41020280968
```

```javascript
{
  "Message": "OK",
  "Success": true
}
```
