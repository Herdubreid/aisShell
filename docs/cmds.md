# Command Usage
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

- [`sv` - Server Context](./cmd-sv.md)
- [`sv` - Form Context](./cmd-fm.md)
- [`sfm` - Stack Form Context](./cmd-sfm.md)
- [`dt` - Data Context](./cmd-dt.md)
- `out` - Set Output File
Directs the output of `exp` and `r` to file (defaults the `-of` parameter).
