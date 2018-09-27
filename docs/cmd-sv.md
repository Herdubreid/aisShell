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

### Options
- [`-c|--context and -|--listContexts`](./opt-context-and-list.md) usage.

### Commands
#### `d` - Define
```
Usage: sv d [options]

Options:
  -b|--baseUrl                Base Url
  -d|--device                 Device
  -rc|--requiredCapabilities  Required Capabilities
  -?|-h|--help                Show help information
``` 
- `-b|--baseUrl`  
The AIS Url with a trailing '/'.
- `-d|--device`  
The device name making the request.  Defaults to 'celin'.
- `-rc|--requiredCapabilities`  
Required capabilities in subsequent requests.  See your `<AIS Url>/defaultconfig` for available capabilities.  Defaults to 'grid,processingOption'.

### Examples
#### Create a new `Server Context` 'e1':
```csh
[:] $ sv -c e1 d -b http://e1.celin.io:9300/jderest/
Server Context 'e1' not found!
New Server Definition? [Y/n] y
[e1:] $ 
```
The 'e1' context is now the default `Server Context` and displayed in the prompt.

#### Connect / Authenticate
```csh
[e1:] fm:wwab $ sv c
User name: demo
Password: *******
..
Signon success!
[e1:demo] fm:wwab $ 
```
After a successful connection, the authenticated user id follows the default `Server Context` indicating an active connection.
