//In this file you can put the code for connecting to the data source.
//Code from this file will be automatically executed during the next application startup.

//Connect to local folder
//main.Connect(@"C:\", "File system");

//Connect to github repo
//main.Connect("https://github.com/tomdam/spcoder", "Github repo")

//Connect to SharePOint online

//Here you can pass the email and password to the Connect function.
//You should never save the password in clear text. Use SPCoder encription mechanizm instead.
//main.Connect("SP-Online-site-url", "SharePoint Client O365", "sponlineemail", "sponlinepass");

//Instead of "sponlinepass", use the "Crypto helper" window to decrypt the password, and then use the encripted value, like this:
//main.Connect("SP-Online-site-url", "SharePoint Client O365", "sponlineemail", main.Decrypt("encryptedsponlinepass"));


//Connect to SharePoint online using the APP permissions. ClientId and ClientSecret can be saved in SPCoder.exe.config file or passed as parameters
//If passing as parameters, please encript them before saving to file

//main.Connect("SP-Online-site-url", "SharePoint Client O365 APP");
//or
//main.Connect("SP-Online-site-url", "SharePoint Client O365 APP", main.Decrypt("encryptedclientid"), main.Decrypt("encryptedclientsecret"));


//main.Connect(@"{{WorkingDirectory}}\Scripts\CSharp", "File system");
