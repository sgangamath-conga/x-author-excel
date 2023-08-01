using System;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace Apttus.XAuthor.Core
{
    public interface IXAuthorApplicationController
    {
        bool CreateApplication(Core.Application NewApp);
        bool AppNameExists(string AppName);
        ApplicationObject LoadApplication(string appId, string appUniqueId);
        bool Save(byte[] xlsxTemplate, string TemplateName, byte[] jsonSchema, byte[] googleSheetSchema, string Edition, bool IsMacroExists = false, string CurrentExtension = "");
        string GetAppIdByUniqueId(string appUniqueId);
        bool DeleteTemplate(string appId);
        bool CloneApp(Application application, string CloneWorkbookName);
        bool OverrideApplication(Application application, string templateNameWithPath, string appName, string AppId, string uniqueId, Byte[] schema = null);
        bool CreateAppAndSaveAttachments(Application application, string templateNameWithPath, string appName, byte[] schema = null);
        bool SaveAttachments(Application application, string templateNameWithPath, byte[] schema = null);
        ApttusDataSet GetAppbyNameOrUniqueId(string appImportName, string uniqueId);
        string GetApplicationObjectUniqueIdAttribute();
        string GetApplicationObjectIdAttribute();
        string GetApplicationObjectNameAttribute();
        bool IsAppActivated(string AppId);
    }

    abstract public class BaseApplicationController : IXAuthorApplicationController
    {
        protected ObjectManager objectManager = ObjectManager.GetInstance;
        protected ConfigurationManager configManager = ConfigurationManager.GetInstance;
        protected ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        protected ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        virtual public bool CreateApplication(Core.Application NewApp)
        {
            throw new NotImplementedException();
        }

        virtual public bool AppNameExists(string AppName)
        {
            throw new NotImplementedException();
        }

        virtual public ApplicationObject LoadApplication(string appId, string appUniqueId)
        {
            throw new NotImplementedException();
        }

        virtual public bool Save(byte[] xlsxTemplate, string TemplateName, byte[] jsonSchema, byte[] googleSheetSchema, string Edition, bool IsMacroExists = false, string CurrentExtension = "")
        {
            throw new NotImplementedException();
        }

        virtual public string GetAppIdByUniqueId(string appUniqueId)
        {
            throw new NotImplementedException();
        }

        virtual public bool DeleteTemplate(string appId)
        {
            throw new NotImplementedException();
        }

        virtual public bool CloneApp(Core.Application application, string CloneWorkbookName)
        {
            throw new NotImplementedException();
        }

        virtual public bool OverrideApplication(Core.Application application, string templateNameWithPath, string appName, string AppId, string uniqueId, byte[] schema = null)
        {
            throw new NotImplementedException();
        }

        virtual public bool CreateAppAndSaveAttachments(Core.Application application, string templateNameWithPath, string appName, byte[] schema = null)
        {
            throw new NotImplementedException();
        }

        virtual public bool SaveAttachments(Core.Application application, string templateNameWithPath, byte[] schema = null)
        {
            throw new NotImplementedException();
        }

        public bool ExportApp(string ExportPath, string ExportFileName)
        {
            bool result = false;

            string[] filenames = Directory.GetFiles(ExportPath);
            try
            {
                // 'using' statements guarantee the stream is closed properly which is a big source
                // of problems otherwise.  Its exception safe as well which is great.
                using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(ExportFileName)))
                {
                    zipStream.SetLevel(9); // 0 - store only to 9 - means best compression
                    byte[] buffer = new byte[4096];

                    foreach (string file in filenames)
                    {
                        // Using GetFileName makes the result compatible with XP
                        // as the resulting path is not absolute.
                        ZipEntry templateEntry = new ZipEntry(Path.GetFileName(file));

                        // Setup the entry data as required.

                        // Crc and size are handled by the library for seakable streams
                        // so no need to do them here.

                        // Could also use the last write time or similar for the file.
                        templateEntry.DateTime = DateTime.Now;
                        zipStream.PutNextEntry(templateEntry);

                        using (FileStream fs = File.OpenRead(file))
                        {
                            // Using a fixed size buffer here makes no noticeable difference for output
                            // but keeps a lid on memory usage.
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                zipStream.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }

                    }
                    // Finish/Close arent needed strictly as the using statement does this automatically

                    // Finish is important to ensure trailing information for a Zip file is appended.  Without this
                    // the created file would be invalid.
                    zipStream.Finish();

                    // Close is important to wrap things up and unlock the file.
                    zipStream.Close();

                    result = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(ex.ToString(), resourceManager.GetResource("COREAPPCONTL_LoadCaption_ErrorMsg"));
            }

            return result;
        }

        public bool ExtractApp(string AppFileName, string ImportDirectory)
        {
            bool result = false;
            try
            {
                //Unzip the File and extract both Application Excel Template and Application Config XML File.
                byte[] buffer = new byte[4096];
                using (FileStream fileStream = new FileStream(AppFileName, FileMode.Open))
                {
                    using (ZipInputStream zipStream = new ZipInputStream(fileStream))
                    {
                        ZipEntry zipEntry;
                        while ((zipEntry = zipStream.GetNextEntry()) != null)
                        {
                            if (zipEntry.CanDecompress && zipEntry.IsFile)
                            {
                                string FileName = ImportDirectory + Path.DirectorySeparatorChar + zipEntry.Name;
                                using (FileStream file = File.Create(FileName))
                                {
                                    int readBytes = 0;
                                    do
                                    {
                                        readBytes = zipStream.Read(buffer, 0, buffer.Length);
                                        file.Write(buffer, 0, readBytes);
                                    }
                                    while (readBytes > 0);
                                }
                                result = true;
                            }
                        }
                        zipStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                ExceptionLogHelper.ErrorLog(ex);
                ApttusMessageUtil.ShowError(ex.Message, resourceManager.GetResource("COREAPPCONTL_ExtractCaption_ErrorMsg"));
            }
            return result;
        }

        virtual public ApttusDataSet GetAppbyNameOrUniqueId(string appImportName, string uniqueId)
        {
            throw new NotImplementedException();
        }

        public virtual string GetApplicationObjectUniqueIdAttribute()
        {
            throw new NotImplementedException();
        }

        public virtual string GetApplicationObjectIdAttribute()
        {
            throw new NotImplementedException();
        }

        public virtual string GetApplicationObjectNameAttribute()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsAppActivated(string AppId)
        {
            throw new NotImplementedException();
        }
    }
}
