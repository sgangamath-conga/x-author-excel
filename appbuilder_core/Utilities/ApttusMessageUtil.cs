/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Web.Services.Protocols;

namespace Apttus.XAuthor.Core
{
    public static class ApttusMessageUtil
    {
        public static bool SuppressMessages { get; set; }
        public const TaskDialogStandardButtons Ok = TaskDialogStandardButtons.Ok;
        public const TaskDialogStandardButtons YesNo = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
        public const TaskDialogStandardButtons OkCancel = TaskDialogStandardButtons.Ok | TaskDialogStandardButtons.Cancel;

        /// <summary>
        /// Show TaskDialog
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="caption">Window title</param>
        /// <param name="instructionText">Instruction text</param>
        /// <param name="icon">Icon</param>
        /// <param name="buttons">Buttons</param>
        /// <param name="detail">Detail text</param>
        /// <returns></returns>
        public static TaskDialogResult Show(string text, string caption, string instructionText,
                                                TaskDialogStandardIcon icon, TaskDialogStandardButtons buttons, string detail,long ownerWindow =0)
        {
            if (SuppressMessages)
                return TaskDialogResult.Yes;

            TaskDialogResult res;
            using (new ActivationContextHelper(true))
            {
                TaskDialog dialog = new TaskDialog();
                dialog.Caption = caption;
                dialog.Text = text;
                dialog.Icon = icon;
                dialog.InstructionText = instructionText;
                dialog.OwnerWindowHandle = new IntPtr(ownerWindow);

                if (detail != String.Empty)
                {
                    dialog.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter;     
                    dialog.DetailsExpandedText = detail;
                }

                dialog.StandardButtons = buttons;

                dialog.Opened += taskDialog_Opened;

                res = dialog.Show();
                dialog.Dispose();
            }

            return res;
        }

        public static TaskDialogResult ShowError(string text, string caption,long ownerWindow = 0)
        {
            return ShowError(text, caption, String.Empty, String.Empty,ownerWindow);
        }

        public static TaskDialogResult ShowError(string text, string caption, string instructionText, string detail,long ownerWindow = 0)
        {
            return Show(text, caption, instructionText, TaskDialogStandardIcon.Error, TaskDialogStandardButtons.Ok, detail,ownerWindow);
        }

        public static TaskDialogResult ShowWarning(string text, string caption, TaskDialogStandardButtons buttons,long ownerWindow = 0)
        {
            return ShowWarning(text, caption, String.Empty, buttons, String.Empty,ownerWindow);
        }

        public static TaskDialogResult ShowWarning(string text, string caption, string instructionText, TaskDialogStandardButtons buttons, string detail,long ownerWindow = 0)
        {
            return Show(text, caption, instructionText, TaskDialogStandardIcon.Warning, buttons, detail,ownerWindow);
        }

        public static TaskDialogResult ShowInfo(string text, string caption,long ownerWindow = 0)
        {
            return ShowInfo(text, caption, String.Empty, String.Empty,ownerWindow);
        }

        public static TaskDialogResult ShowInfo(string text, string caption, string instructionText, string detail,long ownerWindow = 0)
        {
            return Show(text, caption, instructionText, TaskDialogStandardIcon.Information, TaskDialogStandardButtons.Ok, detail,ownerWindow);
        }

        public static TaskDialogResult ShowSoapException(SoapException e, string caption)
        {
            string msg = e.Message;

            //Remove exception class
            string removeString = "AppBuilderWS.SOAPException: ";
            int index = msg.IndexOf(removeString);
            msg = (index < 0)
                ? msg
                : msg.Remove(index, removeString.Length);

            //Remove trailing method information
            removeString = "\n\n";
            index = msg.IndexOf(removeString);
            msg = (index < 0)
                ? msg
                : msg.Substring(0, index);

            return ShowError(msg, caption,0);
        }

        /// <summary>
        /// Workaround to set icon and fix size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void taskDialog_Opened(object sender, EventArgs e)
        {
            TaskDialog taskDialog = sender as TaskDialog;
            taskDialog.Icon = taskDialog.Icon;
            taskDialog.InstructionText = taskDialog.InstructionText;
        }
    }
}
