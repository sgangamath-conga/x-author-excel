using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apttus.XAuthor.Core;
using System.IO;

namespace Apttus.XAuthor.AppRuntime
{
    public class PasteSourceDataActionController
    {
        public  ActionResult Result { get; private set; }
        private PasteSourceDataActionModel Model;
        private ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        private PasteHelper pasteHelper;

        public PasteSourceDataActionController(PasteSourceDataActionModel model)
        {
            Model = model;
            Result = new ActionResult();
            pasteHelper = new PasteHelper();
        }

        public ActionResult Execute()
        {
            Result.Status = ActionResultStatus.Pending_Execution;
            string tempPath = Path.GetTempPath();

            if (!Model.InputUserForFileName)
            {
                string filePath = string.Format(Constants.PASTESOURCEDATA_READY_FILEPATH, tempPath);
                string[] files = Directory.GetFiles(filePath, "*.xlsx");
                if (files.Count() != 1)
                {
                    Result.Status = ActionResultStatus.Failure;
                    return Result;
                }
                pasteHelper.SourceFile = files[0];
            }
            try
            {
                pasteHelper.StartPasteMapping(Model.InputUserForFileName);

                // Create complete directory and move source data file there
                string pasteCompleteFilePath = string.Format(Constants.PASTESOURCEDATA_COMPLETE_FILEPATH, tempPath);
                if (!Directory.Exists(pasteCompleteFilePath))
                    Directory.CreateDirectory(pasteCompleteFilePath);

                string completeFileName = Path.GetFileNameWithoutExtension(pasteHelper.SourceFile) + Constants.UNDERSCORE + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                File.Move(pasteHelper.SourceFile, pasteCompleteFilePath + completeFileName);

                Result.Status = ActionResultStatus.Success;
            }
            catch(Exception ex)
            {
                Result.Status = ActionResultStatus.Failure;
            }

            return Result;
        }
    }
}
