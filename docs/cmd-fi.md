# Form Input [Usage](../README.md)
## Subcommand `fi`
```
Usage: [fm|fr] fi [arguments] [options]

Arguments:
  Id            Id
  Value         Value

Options:
  -rm|--remove  Remove Form Input
  -?|-h|--help  Show help information
```

## Arguments
- `Id`  
  The Form Input Id.
- `Value`  
  The Form Input Value.

## Options
- `-rm|--remove`  
Remove a Form Input Id.

## Examples

### Set Form Input Id's Value
```
$ fi 4 7500
```

### Remove Form Input Id
```
$ fi 4 -rm
```
