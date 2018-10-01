# Data Definition [Usage](../README.md#commands)
### [Command](./cmd-dt.md) `dt d`
```
Usage: dt d [options]

Options:
  -n|--name               Table or View Name
  -t|--type               Target Type
  -st|--serviceType       Service Type
  -rc|--returnControlIds  Return Control IDs
  -mp|--maxPage           Max Page Size
  -ot|--outputType        Output Type
  -?|-h|--help            Show help information
```

## Options
- `-n|--name`   
The table of business view name.
- `-t|--type`  
Either `table` (default) or `view`.
- `-st|--serviceType`  
Either `BROWSE` (default), `COUNT` or `AGGREGATE`
- `-rc|--returnControlIds`   
Fields to return in the format of _table.alias_.
- `-mp|--maxPage`  
Maximum number of rows to return (defaults to 100).

## Examples

### Create F0101 Request
```
[:] $ dt -c ab d -n f0101
New Data Definition? [y/N] y
[:] dt:ab $ 
```

### Create V0101 Request
```
[:] $ dt -c abvw d -n v01010 -t view           
New Data Definition? [y/N] y
[:] dt:abvw $ 
```
