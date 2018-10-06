# Complex Query [Usage](../README.md#commands)
### Subcommand `qpq`
```
Usage: [fm|sfm|dt] cpq [options] [command]

Options:
  -o|--operation   And/Or Operation
  -mt|--matchType  Match Type
  -af|--autoFind   Automatically Find
  -ac|--autoClear  Clear Other Fields
  -?|-h|--help     Show help information

Commands:
  cn               Condition
```
**Note:** The `Server Context` requires the `complexQuery` capability.

## Options
- `-o|--option`  
Ether `AND` or `OR`.
- `-mt|--matchType`  
Either `MATCH_ALL` or `MATCH_ANY`.
- `-af|--autoFinde`  
Automatically find on Entry, `true` or `false`.
- `-ac|--autoClear`  
Automatically clear other Form filter fields, `true` or `false`.

## Commands
- [`cn` - Condition](./cmd-cn.md)

#### See [Oracle's Reference](https://docs.oracle.com/cd/E53430_01/EOTJC/perform_ais_formsvc_calls.htm#BABCBFDC) for more.
