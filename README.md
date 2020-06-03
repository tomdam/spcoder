**SPCoder** is the desktop application for _writing_ and _executing_ **C#** code in real time, without the need for other tools like Visual Studio. It uses [Roslyn](https://github.com/dotnet/roslyn) for analysis and execution of the C# code.

The built-in code editor is available, with syntax highlighting and autocomplete features.
SPCoder also contains various useful features like Grid viewer, Properties window, Describer window, which can be used for inspecting the variables.

SPCoder can be extended by creating new _modules_, _plugins_ or _autorun scripts_. You can find more about those in the documentation.

Some of the standard cases for using SPCoder are:
* 	Testing and learning C# language features
* 	Creating project prototypes
* 	Retreiving the data from various data sources
* 	Cleaning the data
* 	Doing data migrations
* 	...

SPCoder supports execution of C# expressions. 
That means you can write something like this: 
`	Console.WriteLine("Hello world")`
select it and press F5 or Execute button and it will write the string "`Hello world`" to the output window, without the need for creating the class and the Main() method.
It also supports all the other features of C# language, so you can create classes, instantiate objects, call methods, invoke delegates, etc.

![](https://github.com/tomdam/spcoderdocs/blob/master/imgs/main.PNG)


The main motivation for creating the tool like SPCoder was the need for a tool that could directly call SharePoint's object model (thus the name **SPCoder**) in a language different than PowerShell. The first version of SPCoder used IronPython instead of C#.

SPCoder contains different connectors (modules), so you can easily connect to the following "endpoints":
* SharePoint on-premise (Server side object model)
* SharePoint on-premise (Client side object model) - Windows and FBA auth
* SharePoint online (Client side object model) - Regular user and APP auth
* Filesystem - get the content of a folder or a drive
* Github repo - get the content of the repo master branch
* Web page - download the page and parse it's content

Other connectors can be created and added to SPCoder. For instance one could easily create the Facebook connector using Facebook .net sdk, which could be used to connect to Facebook account, get the list of friends, posts, images, etc.

Best way to learn more about SPCoder is through our examples which you can find in wiki pages.
