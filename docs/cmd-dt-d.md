# Data Definition [Usage](../README.md#commands)
### [Command](./cmd-dt.md) `dt d`
```
Usage: dt d [arguments] [options]

Arguments:
  TargetName              Table or View Name
  TargetType              Target Type
  ServiceType             Service Type

Options:
  -rc|--returnControlIds  Return Control IDs
  -mp|--maxPage           Max Page Size
  -ot|--outputType        Output Type
  -?|-h|--help            Show help information
```

## Arguments
- `TargetName`   
The table of business view name.
- `TargetType`  
Either `table` (default) or `view`.
- `ServiceType`  
Either `BROWSE` (default), `COUNT` or `AGGREGATE`

## Options
- `-rc|--returnControlIds`   
Fields to return in the format of _table.alias_.
- `-mp|--maxPage`  
Maximum number of rows to return (defaults to 100).

## Examples

### Create F0101 Request
```
[:] $ dt -c ab d f0101
New Data Definition? [y/N] y
[:] dt:ab $ 
```

### Create V0101 Request
```
[:] $ dt -c abvw d v01010 view           
New Data Definition? [y/N] y
[:] dt:abvw $ 
```