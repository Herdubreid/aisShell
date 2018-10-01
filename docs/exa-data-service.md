# Data Service [Example](../README.md#examples)

#### See [`Server Context`](./cmd-sv.md#examples) for _e1_ connection example.

#### See [Oracle Doc's](https://docs.oracle.com/cd/E53430_01/EOTJC/perform_ais_formsvc_calls.htm#EOTJC222) for Java example.

### Create Data Request
```
[e1:demo] $ dt -c ab d -n f0101 -rc F0101.AN8|F0101.ALPH|F0101.AT1 -mp 10
New Data Definition? [y/N] y
[e1:demo] dt:ab $ 
```

### Add Query and Submit
```
[e1:demo] dt:ab $ qry -mt match_all -af true
[e1:demo] dt:ab $ qry cn F0101.AN8 greater literal 7000
[e1:demo] dt:ab $ s
/Responses 1.
[e1:demo] dt:ab $ 
```

### Iterate the Response
```
[e1:demo] dt:ab $ r -k rowset[0] -d 0
{
  "rowIndex": 0,
  "MOExist": false,
  "F0101_AN8": {},
  "F0101_ALPH": {},
  "F0101_AT1": {}
}
[e1:demo] dt:ab $ r -k rowset it F0101_AN8.value;F0101_ALPH.value;F0101_AT1.value
7004	European Motors	V	
7272	Good Real	C	
7339	Fujimori Shosha	V	
7372	Auditor for Expense Reimbursement	M	
7373	Expense Report Approvals	M	
7384	Purchasing Department	M	
7392	Accounts Receivable Department	M	
7405	Engineering	M	
7410	Maintenance Management-Alerts	M	
7500	McDougle, Cathy	E	
[e1:demo] dt:ab $ 
```

