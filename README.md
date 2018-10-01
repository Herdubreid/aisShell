# Celin's aisShell Usage

## Install

### Build Latest
Download zip with button above or clone with git:
```
$ git clone https://github.com/Herdubreid/aisShell.git
```
Build with [Visual Studio 2017](https://visualstudio.microsoft.com/downloads/).

### Download Binaries
Two packages are available for download.
- Fully contained Windows8/10 x64  
[v0.1 - Pre-release](https://github.com/Herdubreid/aisShell/releases/download/v0.1/win8-x64.zip)
- Framework Dependent  
[v0.1 - Pre-release](https://github.com/Herdubreid/aisShell/releases/download/v0.1/dependent.zip)

The associated source code can also be downloaded from the [Release Tab](https://github.com/Herdubreid/aisShell/releases).

## Run
Run `aisShell.exe` in the fully contained package or use `dotnet aisShell.dll` in the framework depended package.  
The framework dependent package requires installation of [Net Core 2](https://www.microsoft.com/net/download) runtime.


## Usage
The `help` command lists available commands.
```
[:] $ help
Usage: [command] [options]

Commands:
  sv                   Server Context
  fm                   Form Context
  sfm                  Stack Form Context
  dt                   Data Context
  out                  Set Output File
  help                 Show Available Commands
  clear                Clear Screen
  quit                 Quit Shell

Use [command] [-?|-h|--help] to get command help
```
Each command can have one or more subcommands and options, followed again with subcommands and options and so forth.

Any command or subcommand can be entered with a  `-?`, `-h` or `--help` option for help.

## Commands
- [`sv` - Server Context](/docs/cmd-sv.md)
- [`fm` - Form Context](/docs/cmd-fm.md)
- [`sfm` - Stack Form Context](/docs/cmd-sfm.md)
- [`dt` - Data Context](/docs/cmd-dt.md)
- `out` - Redirects the output of `exp` and `r` subcommands to file (defaults the `-of` option).

## Examples

#### [Reading Data](/docs/exa-reading-data.md)
#### [Adding Data](/docs/exa-adding-data.md)
#### [Deleting Data](/docs/exa-deleting-data.md)
#### [Stack Application](/docs/exa-application-stack.md)
