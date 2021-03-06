# TLx2

###### *read this in HonestTrailers voice:*
### In a world full of chrome windows... where fat foxes are running slow... have you ever dreamed of transforming your html to exe without touching a single electron?
### Furthermore, compressed and with plugin capabilities to bridge the gap to the desktop's OS ?
## Well, dream no more! because I give you **Too Little Too Late** !
###### *insert youtube comercial here*

TLx2 is basically a winforms application with an somehow twiked webbrowser embedded object

For embedding your own html in it you only have to do `Tlx2.exe -add yours.html`.

This will result in a new *yours.exe* that will contain a compressed version of your html to be rendered at runtime.

If you throw in the same directory one or more .NET dlls containing the `TLx2` namespace, all the `public static` methods of all the `public static class ScriptExtensions` classes from the `TLx2` namespace in those dlls, will be available inside your html's javascript as `csharp.<mysoontobeawsomemethod>()`.

There are some examples of this in the *ScriptExtension.cs* in this project.

All the methods there are all ready to use in your own html without further .NET coding:

|name|returns|
|---|---|
|csharp.allocconsole()|void|
|csharp.freeconsole()|void|
|csharp.consolelog(string text)|void|
|csharp.consoleerror(string text)|void|
|csharp.arguments()|json|
|csharp.opts()|json|
|csharp.ismember(string groupname)|bool|
|csharp.getenv(string varname)|string|
|csharp.setenv(string varname, string value)|void|
|csharp.readfile(string path)|json|
|csharp.writefile(string path,string content)|json|
|csharp.alert(string text,(optional)string title|void|
|csharp.regread(string key,(optional)string valuename)|json|
|csharp.regwrite(string key, string valuename, string value)|json|
|csharp.run(string cmd,string arguments)|void|
|csharp.hostname()|string|

There is also a property:
- csharp.externalmethodnames - array containg all methods names form all dll plugins available

Goes without(apperently not) saying that TLx2 runs on windows...
