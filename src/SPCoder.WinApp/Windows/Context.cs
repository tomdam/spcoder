using SPCoder.Context;
using SPCoder.Describer;
using SPCoder.HelperWindows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SPCoder.Windows
{
    public partial class Context : DockContent
    {
        public Context()
        {
            //for debug purposes
            //ListBox.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        
        public void AddToContext(ContextItem item)
        {
            SPCoderForm.MainForm.MyContext.AddItem(item);
            //SPCoderForm.MainForm.CSharpContext.AddItem(item);

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    lbContext.Items.Add(item);
                });
            }
            else
            {
                lbContext.Items.Add(item);
            }
        }

        public void AddToContext(object item)
        {
            try
            {
                ContextItem contextItem = new ContextItem { Data = item, Type = item.GetType().ToString() };
                AddToContext(contextItem);
            }
            catch (Exception ex)
            {
                SPCoderForm.MainForm.LogException(ex);
            }
        }

        public void DoRenameContextItem(object item, string newName)
        {
            ContextItem myItem = (ContextItem)item;
            int ind = lbContext.Items.IndexOf(myItem);
            string oldName = myItem.Name;

            if (SPCoderForm.MainForm.MyContext.GetContext.ContainsKey(newName))
            {
                string err = string.Format("The variable '{0}' already exists in context", newName);
                
                SPCoderForm.MainForm.AppendToLog(err);
                throw new Exception(err);
            }

            SPCoderForm.MainForm.MyContext.RenameItem(myItem, newName);
            //SPCoderForm.MainForm.CSharpContext.RenameItem(myItem, newName);

            lbContext.Items.Remove(myItem);
            if (ind != -1)
            {
                lbContext.Items.Insert(ind, myItem);
            }
            else
            {
                lbContext.Items.Add(myItem);
            }
            
            SPCoderForm.MainForm.AppendToLog(string.Format("Object '{0}' renamed to '{1}'", oldName, newName));
        }

        private void tsDescribe_Click(object sender, EventArgs e)
        {
            if (lbContext.SelectedItem != null)
            {
                ContextItem item = (ContextItem)lbContext.SelectedItem;
                SPCoderForm.MainForm.Describe(item);
                //DescriberForm describerForm = new DescriberForm(item);
                //describerForm.Show(this);
            }
        }

        private void tEstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //pgEditor
            if (lbContext.SelectedItem != null)
            {
                ContextItem item = (ContextItem)lbContext.SelectedItem;
                SPCoderForm.MainForm.ShowProperties(item.Data, item.Name);
            } 
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                object[] items = new object[lbContext.SelectedItems.Count];
                int i = 0;
                foreach (object item in lbContext.SelectedItems)
                {
                    items[i++] = item;

                }
                foreach (object item in items)
                {
                    SPCoderForm.MainForm.MyContext.RemoveItem((ContextItem)item);

                    //SPCoderForm.MainForm.CSharpContext.RemoveItem((ContextItem)item);

                    lbContext.Items.Remove(item);
                }
            }
            catch (Exception)
            {
            }
        }

        public void RemoveDataItemFromContext(object itemToRemove)
        {
            try
            {
                object[] items = new object[lbContext.Items.Count];
                int i = 0;
                foreach (object item in lbContext.Items)
                {
                    items[i++] = item;

                }
                foreach (object item in items)
                {
                    var itm = ((ContextItem)item).Data;
                    if (itm.Equals(itemToRemove))
                    {
                        SPCoderForm.MainForm.MyContext.RemoveItem((ContextItem)item);
                        lbContext.Items.Remove(item);
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Rename the object in the context
            try
            {
                object item = lbContext.SelectedItem;
                ContextItemRenamer renamer = new ContextItemRenamer { Item = item };
                renamer.Rename += DoRenameContextItem;
                renamer.ShowDialog(this);
            }
            catch (Exception)
            {
            }
        }

        private void pluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void lbContext_DragDrop(object sender, DragEventArgs e)
        {
            //AddToContext(sourceNode.Tag);
            AddToContext(SPCoderForm.MainForm.DragedBaseNode.SPObject);
            //lbContext.Items.Add(SPCoderForm.MainForm.DragedBaseNode.SPObject);
        }

        private void lbContext_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
    }
}
