# Usage
### Command `sv`
```
[:] $ sv -h
Server Context

Usage: sv [options] [command]

Options:
  -c|--context       Server Context
  -l|--listContexts  List Contexts
  -?|-h|--help       Show help information

Commands:
  c                  Connect
  d                  Define
  exp                Export Servers
  lo                 Logout
  load               Load Definitions
  save               Save Definitions

Run 'sv [command] --help' for more information about a command.
```
Before any AIS requests can be made, a server context must be defined and connected.

## Options
- [`-c|--context and -|--listContexts`](./opt-context-and-list.md) usage.

## Commands
Example:
```csh
[:] $ sv -c e1 d -b http://e1.celin.io:9300/jderest/
Server Context 'e1' not found!
New Server Definition? [Y/n] y
[e1:] $ 
```
Where:
- `-c` is Context Id `e1`.  If it doesn't exist then the user is prompted for a new definition.
- `d` Context definition.
- `-b` is the the AIS Url (note the trailing '/').

Command usage help is displayed with trailing `-?` or `-h`.  For example `sv -h` and `sv d -?`.

The current server context is displayed in the prompt `[e1:] $`.
