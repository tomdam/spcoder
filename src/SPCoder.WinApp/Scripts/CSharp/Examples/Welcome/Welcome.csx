//Welcome to SPCoder - This file contains some basic examples with which you can get familiar with SPCoder

//You can select any part of code from this file and execute it by pressing F5 on 
//keyboard or clicking the "Execute (F5)" button

Console.WriteLine("Hello from SPCoder!");

//Note that the string "Hello from SPCoder!" is written to Output window (located on bottom-right part of the main window)

//You can change the title of the main window by executing the following line:

main.Text = "Setting title from code";

//main window is an instance of SPCoder.SPCoderForm which is a subclass of System.Windows.Forms.Form
//you can check the type by executing following line (Note that println is shortcut for Console.WriteLine()):
println(main.GetType())

//Notice that there is no ; at the end of previous line, but it was executed without an error.
//SPCoder checks the "one liners" before execution, and appends semicolon if necessary

//If you try to execute the following line, you will get an error stating
//error CS0027: Keyword 'this' is not available in the current context
//So you cannot use the this keyword in C# scripting outside a class
println(this);

//Now we will see how Explorer window can be used. Execute this line:
main.Connect("C:\\", "File system");

//After running that line, Explorer window (located on left part of the main window) 
//should contain the tree view with files and folders from C: drive
//Also, the Context window contains the directoryinfo variable, which is the root (C:\) object
println( directoryinfo.FullName);

//Now we can print all folders from root directoryinfo to output window.
var folders = directoryinfo.GetDirectories();
foreach(var f in folders)
{
    println(f.FullName);
}

//We can also view the variable folders in Properties viewer with this code
main.ShowProperties(folders, "folders");

//We can view the variable folders in Describer window with this code
main.ShowDescriber("folders");

//Or we can display the same variable in Grid Viewer:
main.SpGrid.ShowExpressionInGrid("folders");
main.ShowGridWindow();

//We can also open new code windows directly from code
//This can be useful if we want to create our own plugins
string sourceCode = "var a = 1;\nprintln(a);";
main.GenerateNewSourceTab("newCode.csx", sourceCode, null, "csx");

//You can check out the "Corona" example, for more complex code samples that involve 
//the use of multiple SPCoder connectors and external libraries

//Corona example is located in Scripts\CSharp\Examples\Corona\ folder

main.GenerateNewSourceTabsFromPath("Scripts\\CSharp\\Examples\\Corona\\Corona.csx");