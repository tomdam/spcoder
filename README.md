**SPCoder** is the desktop application for _writing_ and _executing_ **C#** code in real time, without the need for other tools like Visual Studio. It uses [Roslyn](https://github.com/dotnet/roslyn) for analysis and execution of the C# code.

The built-in code editor is available, with syntax highlighting and autocomplete features.
SPCoder also contains various useful features like Grid viewer, Properties window, Describer window, which can be used for inspecting variables.

SPCoder can be extended by creating new _connectors_, _plugins_ or _autorun scripts_. You can find more about those in the [wiki documentation](https://github.com/tomdam/spcoder/wiki).

Some of the standard cases for using SPCoder are:
* 	Testing and learning C# language features
* 	Creating project prototypes
* 	Retreiving and updating the data from various data sources
* 	Cleaning the data
* 	Doing data migrations
* 	...

SPCoder supports execution of C# expressions. 
That means you can write something like this: 
`	Console.WriteLine("Hello world")`
select it and press F5 on Keyboard or click the Execute button and it will write the string `Hello world` to the output window, without the need for creating a class and the Main() method.
It also supports all the other features of C# language, so you can create classes, instantiate objects, call methods, invoke delegates, etc.

![](https://github.com/tomdam/spcoderdocs/blob/master/imgs/main0.PNG)


The initial motivation for creating the application like SPCoder was the need for a tool that could directly call SharePoint's object model (hence the name **SP**Coder) in a language different than PowerShell. The first version of SPCoder used IronPython instead of C#. 

SPCoder contains different connectors (modules), so you can easily connect to the following data sources (endpoints) :
* Filesystem - get the content of a folder or a drive
* Github repo - get the content of the Github repo master branch
* Web page - download the web page and parse it's content
* SharePoint on-premise (Server side object model) - manage SharePoint using SSOM
* SharePoint on-premise (Client side object model) - Windows and FBA auth (manage SharePoint using CSOM)
* SharePoint online (Client side object model) - Regular user and APP auth (manage SharePoint using CSOM)

Other connectors can be created and added to SPCoder. For instance one could easily create the Facebook connector using Facebook .net sdk, which could be used to connect to a Facebook account, get the list of friends, posts, images, etc.

SPCoder has been successfully used in different scenarios, to list some:
* Migration of large corporate Mediawiki site to SharePoint online
* Migration of various File shares to SharePoint online
* Migration of content from Alfresco to SharePoint
* Migration of content from SharePoint 2010/2013 to SharePoint 2016/online
* Executing various post migration updates and checks
* Creating complete structures of SharePoint sites, lists, pages
* Implementing service for transfering data from IBM Informix db to external REST API
* ...

Best way to learn more about SPCoder is through our examples. Once you start SPCoder, the [Welcome example](https://github.com/tomdam/spcoder/wiki/Welcome-example) will be displayed, and for more complex stuff you should check the [Corona example](https://github.com/tomdam/spcoder/wiki/Corona-example).

Please check [Wiki](https://github.com/tomdam/spcoder/wiki) pages for more detailed documentation.
