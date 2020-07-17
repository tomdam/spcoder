using SPCoder.Core.Utils;
using SPCoder.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace SPCoder.SharePoint.Client.Utils
{
    public class SharePointEditedFile : BaseEditedFile
    {
        public override bool Save(bool overwrite = false)
        {
            if (!(this.ParentContainer is Folder))
            {
                SPCoderLogging.Logger.Info("ParentContainer is not an SP Folder. Bailing out...");
            }

            FileCreationInformation fileCreation = new FileCreationInformation();
            fileCreation.Url = this.Filename;
            fileCreation.ContentStream = this.Stream;
            fileCreation.Overwrite = overwrite;


            Folder parentFolder = (Folder)this.ParentContainer;
            ClientContext ctx = parentFolder.Context as ClientContext;

            try
            {
                // If it's checked in, check it out then re-publish
                // If it's checked out to us, save it then leave it checked out
                File existingFile = parentFolder.GetFile(this.Filename);
                existingFile.EnsureProperties(f => f.CheckedOutByUser);

                bool wasCheckedOut = existingFile.CheckedOutByUser.ServerObjectIsNull == false;
                if (wasCheckedOut)
                {
                    // Make sure it's the current user that has the file checked out
                    User me = ctx.Web.CurrentUser;
                    me.EnsureProperties(m => m.Id);

                    if (me.Id != existingFile.CheckedOutByUser.Id)
                    {
                        SPCoderLogging.Logger.Error($"File is already checked out to: {existingFile.CheckedOutByUser.Title}");
                        return false;
                    }
                }
                else
                {
                    existingFile.CheckOut();
                }

                File newFile = parentFolder.Files.Add(fileCreation);
                ctx.Load(newFile);
                
                if (!wasCheckedOut)
                {
                    // Todo - allow setting what type of checkin to save with
                    newFile.CheckIn("Checked In by SPCoder", CheckinType.MajorCheckIn);
                }

                ctx.ExecuteQueryRetry();
            }
            catch (Exception ex)
            {
                SPCoderLogging.Logger.Error($"Failed to save file: {ex.ToString()}");
                throw;
            }

            return true;
        }
    }
}
