using SPCoder.Core.Utils;
using SPCoder.FileSystem.Utils.Nodes;
using SPCoder.Utils;
using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace SPCoder.FileSystem.Utils
{

    public class FSConnector : BaseConnector
    {
        DirectoryInfo Folder { get; set; }
        public FSConnector(string connectorType)
        { }

        public FSConnector()
        { }

        string RootNodeTitle = "Filesystem: ";

        public override SPCoder.Utils.Nodes.BaseNode GetSPStructure(string siteUrl)
        {
            Folder = new DirectoryInfo(siteUrl);
            BaseNode rootNode = new FolderNode(Folder);
            rootNode.Title = RootNodeTitle + Folder.Name;
            rootNode.NodeConnector = this;
            rootNode.OMType = ObjectModelType.LOCAL;

            TraverseFileSystem(Folder, rootNode, rootNode);

            return rootNode;
        }

        public override BaseNode ExpandNode(BaseNode node, bool doIfLoaded = false)
        {
            SPCoderLogger.Logger.LogInfo("Expanding " + node.Url);
            //Ako je folder node
            if (node is FolderNode)
            {
                //Ako nije ucitan
                if (!node.LoadedData)
                {
                    if (node.ParentNode.Children != null && node.ParentNode.Children.Contains(node))
                    {
                        node.ParentNode.Children.Remove(node);
                    }

                    node = TraverseFileSystem((DirectoryInfo)node.SPObject, node.ParentNode, node.RootNode);
                }
            }
            return node;
        }


        private BaseNode TraverseFileSystem(DirectoryInfo folder, BaseNode parentNode, BaseNode rootNode)
        {
            try
            {
                //if (myNode == null)
                BaseNode myNode = new FolderNode(folder);
                myNode.LoadedData = true;

                if (parentNode != null)
                {
                    parentNode.Children.Add(myNode);
                    myNode.ParentNode = parentNode;
                }
                myNode.RootNode = rootNode;
                
                try
                {
                    foreach (DirectoryInfo d in folder.GetDirectories())
                    {
                        //TraverseFileSystem(d, myNode, rootNode);
                        //do not go to next level, just add the first level under the current
                        BaseNode myFolderNode = new FolderNode(d);
                        myNode.Children.Add(myFolderNode);
                        myFolderNode.ParentNode = myNode;
                        myFolderNode.RootNode = rootNode;
                    }
                }
                catch (Exception)
                {
                    //SPCoderForm.MainForm.LogException(exc);
                }


                foreach (FileInfo file in folder.GetFiles())
                {                    
                    BaseNode myFileNode = new FileNode(file);
                    myNode.Children.Add(myFileNode);
                    myFileNode.ParentNode = myNode;
                    myFileNode.RootNode = rootNode;
                    try
                    {
                        //Icon icon = Icon.ExtractAssociatedIcon(file.FullName);
                        Icon icon = ShellIcon.GetSmallIconFromExtension(file.Extension);
                        myFileNode.IconObject = icon.ToBitmap();
                        if (myFileNode.IconObject.Width != 16)
                        {
                            myFileNode.IconObject = new Bitmap(myFileNode.IconObject, 16, 16);
                        }/**/
                    }
                    catch (Exception)
                    {
                        //skip if exception happens here... the default icon will be shown
                    }                  
                }
                return myNode;
            }
            catch (Exception)
            {
                return null;
                //SPCoderForm.MainForm.LogException(exc);
            }
        }

        public override string ImagesPath
        {
            get
            {
                return null;
            }
            set
            {}
        }

        public override bool IsImagesPathLocal
        {
            get
            {
                return true;
            }
            set
            {}
        }

        public override List<object> AutoAddToContext()
        {
            List<object> objects = new List<object>();
            objects.Add(this.Folder);

            return objects;
        }
    }
}
