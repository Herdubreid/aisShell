# Stack Application [Example](../README.md#examples)

#### See [`Server Context`](./cmd-sv.md#examples) for _e1_ connection example.

#### See [Oracle Doc's](https://docs.oracle.com/cd/E53430_01/EOTJC/perform_ais_formsvc_calls.htm#EOTJC132) for Java example.

### Create Stack Form Request
```
[e1:demo] $ sfm -c step1 fr -fn p01012_w01012b -mp 5 -rc 54|1[19,20]
New Stack Form Definition? [y/N] y
[e1:demo] sfm:step1 $ 
```

### Add Form Actions and Open Form
```
[e1:demo] sfm:step1 $ fr fa 54 SetControlValue E
[e1:demo] sfm:step1 $ fr fa 15 DoAction
[e1:demo] sfm:step1 $ o
-Responses 1.
[e1:demo] sfm:step1 $ 
```

### Display the 3rd Grid Row Values
```
[e1:demo] sfm:step1 $ r -k rowset[0] -d 0              
{
  "rowIndex": 0,
  "MOExist": false,
  "mnAddressNumber_19": {},
  "sAlphaName_20": {}
}
[e1:demo] sfm:step1 $ r -k rowset it mnAddressNumber_19.value;sAlphaName_20.value 3 3
2049	McLind, Rod	
[e1:demo] sfm:step1 $ 
```

### Create Stack Action Request
```
[e1:demo] sfm:step1 $ sfm -c step2 sa -fo w01012b -rc 28
New Stack Form Definition? [y/N] y
[e1:demo] sfm:step2 $ 
```

### Add Form Actions to the Stack Action and Execute
```
[e1:demo] sfm:step2 $ sa fa 1.3 SelectRow
[e1:demo] sfm:step2 $ sa fa 14 DoAction
[e1:demo] sfm:step2 $ e
/Responses 2.
[e1:demo] sfm:step2 $ 
```

### Display Response's Data
```
[e1:demo] sfm:step2 $ r -k data
{
  "txtAlphaName_28": {
    "id": 28,
    "internalValue": "McLind, Rod",
    "title": "Alpha Name",
    "dataType": 2,
    "staticText": "Alpha Name",
    "visible": true,
    "bsvw": true,
    "longName": "txtAlphaName_28",
    "value": "McLind, Rod",
    "editable": true
  }
}
[e1:demo] sfm:step2 $ 
```
Only control  _28_ is returned as requested with  the `-rc` option.

### Create a Stack Action Request to Update Name
```
[e1:demo] sfm:step2 $ sfm -c step3 sa -fo w01012a -rc 54|1[19,20]
New Stack Form Definition? [y/N] y
[e1:demo] sfm:step3 $ sa fa 28 SetControlValue AIS APP Stack TEST
[e1:demo] sfm:step3 $ sa fa 11 DoAction
[e1:demo] sfm:step3 $ e
\Responses 3.
[e1:demo] sfm:step3 $ 
```
The above lines create a new stack action that changes `Alpha Name` to _AIS APP Stack TEST_ and saves it with the Ok button.

### Create a Stack Action Request to Close the Form
```
[e1:demo] sfm:step3 $ sfm -c step4 sa -fo w01012a -rc 54|1[19,20]
New Stack Form Definition? [y/N] y
[e1:demo] sfm:step4 $ sa fa 12 DoAction
[e1:demo] sfm:step4 $ e
|Responses 4.
[e1:demo] sfm:step4 $ 
```

### Display Again the 3rd Grid Row Values
```
[e1:demo] sfm:step4 $ r -k rowset it mnAddressNumber_19.value;sAlphaName_20.value 3 3
2049	AIS APP Stack TEST	
[e1:demo] sfm:step4 $ 
```

### Close the Stack Form
```
e1:demo] sfm:step4 $ c
|Responses 5.
[e1:demo] sfm:step4 $ r -d 0
{
  "fs_P01012_W01012B": {},
  "stackId": 0,
  "stateId": 0,
  "rid": "",
  "currentApp": "P01012_W01012B",
  "timeStamp": "2018-10-01:16.29.40",
  "sysErrors": []
}
[e1:demo] sfm:step4 $ 
```
The `stackId`, `stateId` and `rid` values are initialised.

