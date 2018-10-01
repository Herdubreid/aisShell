# Query [Usage](../README.md#commands)
### Subcommand `qry`
```
Usage: [fm|sfm|dt] qry [options] [command]

Options:
  -mt|--matchType  Match Type
  -af|--autoFind   Automatically Find
  -ac|--autoClear  Clear Other Fields
  -?|-h|--help     Show help information

Commands:
  cn               Condition
```

**Note:** The `Server Context` requires the `query` capability.

## Options
- `-mt|--matchType`  
Either `MATCH_ALL` or `MATCH_ANY`.
- `-af|--autoFinde`  
Automatically find on Entry, `true` or `false`.
- `-ac|--autoClear`  
Automatically clear other Form filter fields, `true` or `false`.

## Commands
- [`cn` - Condition](./cmd-cn.md)

#### See [Oracle's Reference](https://docs.oracle.com/cd/E53430_01/EOTJC/perform_ais_formsvc_calls.htm#EOTJC144) for more.
