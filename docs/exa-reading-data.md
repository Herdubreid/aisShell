# Reading Data [Example](../README.md#examples)

#### See [`Server Context`](./cmd-sv.md#examples) for _e1_ connection example.

#### See [Oracle Doc's](https://docs.oracle.com/cd/E53430_01/EOTJC/perform_ais_formsvc_calls.htm#EOTJC178) for Java example.

### Create Form Request
```
[e1:demo] $ fm -c wwab d -fn p01012_w01012b -mp 30 -rc 54|1[19,20]
New Form Definition? [y/N] y
[e1:demo] fm:wwab $ 
```

### Set Form Actions and Submit
```
[e1:demo] fm:wwab $ fa 1[19] SetQBEValue >=6001
[e1:demo] fm:wwab $ fa 15 DoAction
[e1:demo] fm:wwab $ s
-Responses 1.
[e1:demo] fm:wwab $ 
```

### Response's `summary` Json Key
```
[e1:demo] fm:wwab $ r -k summary
{
  "records": 30,
  "moreRecords": true
}
[e1:demo] fm:wwab $                                
```

### Response's Grid Members
```
[e1:demo] fm:wwab $ r -k rowset[0] -d 0
{
  "rowIndex": 0,
  "MOExist": true,
  "mnAddressNumber_19": {},
  "sAlphaName_20": {}
}
[e1:demo] fm:wwab $ 
```

### Iterate Grid Member's Value
```
[e1:demo] fm:wwab $ r -k rowset it mnAddressNumber_19.value;sAlphaName_20.value
6001	Allen, Ray	
6002	Abbott, Dominique	
6015	Western Distribution Center	
6016	Hunter, Monica	
6023	Northern Distribution Center	
6026	LM Service Company	
6031	Eastern Distribution Center	
6032	Nguyen, Ellen	
6033	Donovan, Andrew	
6034	Rothchild, Douglas	
6035	EPS Distribution Center	
6040	Southern Distribution Center	
6044	Abrams, Brooke	
6046	Hunter, Max	
6050	LP Warehouse	
6055	Reardon, Lauren	
6056	Galligan, Shawn	
6058	Western Manuf/Dist Center	
6066	Northern Manuf/Dist Center	
6070	Toth, Stefan	
6071	Ishita, Narumi	
6072	Toth, Catherine	
6073	Ishita, Hiroshi	
6074	Eastern Manufacturing Plant	
6075	Ishita, Yuki	
6077	Marcheso, Dominic	
6078	Aiken, Gwen	
6079	Glass, Kendra	
6080	Flanagan, Seamus	
6082	Central Manufacturing Plant	
[e1:demo] fm:wwab $ 
```
