### Server Context `sv`
An AIS server definition.
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
