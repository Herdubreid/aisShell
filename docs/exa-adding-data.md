# Adding Data [Example](../README.md#examples)

#### See [`Server Context`](./cmd-sv.md#examples) for _e1_ connection example.

#### See [Oracle Doc's](https://docs.oracle.com/cd/E53430_01/EOTJC/perform_ais_formsvc_calls.htm#EOTJC180) for Java example.

### Create Form Request
```
[e1:demo] $ fm -c addPhone d -fn p0115_w0115a -fs u
New Form Definition? [y/N] y
[e1:demo] fm:addPhone $ exp
```

### Add Form Input Values
```
[e1:demo] fm:addPhone $ fi 4 7500  
[e1:demo] fm:addPhone $ fi 5 0
[e1:demo] fm:addPhone $ 
```
The values _7500_ represents the Address Number and _0_ the Who's Who line number.

### Add Grid Insert Values
```
[e1:demo] fm:addPhone $ gi -g 1 -r 1
[e1:demo] fm:addPhone $ gi 27 SetGridCellValue HOM
[e1:demo] fm:addPhone $ gi 28 SetGridCellValue 303
[e1:demo] fm:addPhone $ gi 29 SetGridCellValue 123-456
[e1:demo] fm:addPhone $ 
```
The first line creates a grid insert event and the subsequent lines set the value of columns _27_, _28_ and _29_.

### Press the Ok Button
```
[e1:demo] fm:addPhone $ fa 4 DoAction
[e1:demo] fm:addPhone $ 
```

### Review the Request
```
[e1:demo] fm:addPhone $ exp
{
  "formServiceAction": "U",
  "formInputs": [
    {
      "id": "4",
      "value": "7500"
    },
    {
      "id": "5",
      "value": "0"
    }
  ],
  "formActions": [
    {
      "gridAction": {
        "gridRowInsertEvents": [
          {
            "rowNumber": 1,
            "gridColumnEvents": [
              {
                "command": "SetGridCellValue",
                "value": "HOM",
                "columnID": "27"
              },
              {
                "command": "SetGridCellValue",
                "value": "303",
                "columnID": "28"
              },
              {
                "command": "SetGridCellValue",
                "value": "123-456",
                "columnID": "29"
              }
            ]
          }
        ],
        "gridID": "1"
      }
    },
    {
      "controlID": "4",
      "command": "DoAction",
      "value": ""
    }
  ],
  "formName": "P0115_W0115A",
  "maxPageSize": "10",
  "aliasNaming": false
}
[e1:demo] fm:addPhone $ 
```

### Submit the Form Request
```
[e1:demo] fm:addPhone $ s
\Responses 1.
[e1:demo] fm:addPhone $
```

### Iterate the Response for the Column Values
```
[e1:demo] fm:addPhone $ r -k rowset -d 0
{
  "rowIndex": 0,
  "MOExist": false,
  "sPrefix_28": {},
  "sPhoneNumber_29": {},
  "sPhoneType_27": {},
  "sPhoneTypeDescription_66": {},
  "mnLineNumber_26": {}
}
[e1:demo] fm:addPhone $ r -k rowset it sPrefix_28.value;sPhoneNumber_29.value;sPhoneType_27.value
303	123-456	HOM	
[e1:demo] fm:addPhone $ 
```
