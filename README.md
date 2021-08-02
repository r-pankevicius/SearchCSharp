# SearchCSharp

Search for substrings in string literals in *.cs files.

Not user friendly version, look for "param" in comments in Program.cs to find out different options.

Or search for all strings and pipe output to grep.

## Examples

`SearchCSharp.exe "Usage"`

Searches .cs files recursively starting from current folder and finds string literals containing substring "Usage".

`SearchCSharp.exe "Usage" c:\dev`

Searches .cs files recursively starting from c:\dev folder and finds string literals containing substring "Usage".
