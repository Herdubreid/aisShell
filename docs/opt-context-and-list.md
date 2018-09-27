# Usage
### Options `-c|--context <id>` and `-l|--contextList`
The shell keeps a repository of defined contexts with a unique alpha-numeric context id.  
Request commands are executed in the selected or default context against the default `Server Context`.
## Create a new Context
To create a new context, use a non existing id as the parameter, followed by the definition command `d`:
```
[:] $ sv -c e1 d
Server Context 'e1' not found!
New Server Definition? [y/N] 
```
A prompt will indicate that the context doesn't exist and creates it with 'y' response (defaults to 'N').
## Select a Context
If the context id exist, then it's selected in any subcommands and becomes default:
```
[e1:] fm:wwab $ fm -c ab
[e1:] fm:ab $
```
Note that the `fm` command is reduntant in the above example since the default request context is a `Form Context`.
## Default Context
The last created or selected context is defaulted and displayed in the prompt:
```
[e1:] fm:wwab $
```
In the above prompt, the server context is 'e1' with 'wwab' the default form context denoted by 'fm:'.
## List Contexts
The `-l` option lists existing contexts:
```
[e1:] fm:ab $ -l
wwab
ab
[e1:] fm:ab $ 
```
Note the `fm` command is not needed above since we want a list of `Form Context`'s.
