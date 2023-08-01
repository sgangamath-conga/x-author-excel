/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.AppDesigner;
using Apttus.XAuthor.Core;
using Microsoft.Office.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


public static class TaskPaneHelper
{
    public static void LoadCustomTaskPane(UserControl View, string title)
    {
        CustomTaskPane ctp = Globals.ThisAddIn.CustomTaskPanes.Add(View, title, Globals.ThisAddIn.Application.ActiveWindow);
        ctp.VisibleChanged += VisibleChangeEvent;
        ctp.Width = 640;
        ctp.Visible = true;
    }

    private static void VisibleChangeEvent(object sender, EventArgs e)
    {
        CustomTaskPane taskPane = sender as CustomTaskPane;
        if (taskPane != null)
        {
            if (taskPane.Visible == false)
            {
                Globals.ThisAddIn.CustomTaskPanes.Remove(taskPane);
                taskPane.Dispose();
                taskPane = null;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static void RemoveAllTaskPanes()
    {
        bool isObjectDisposed = false;
        int iCustomTaskPanesCount = 0;

        try
        {
            iCustomTaskPanesCount = Globals.ThisAddIn.CustomTaskPanes.Count;
        }
        catch (Exception)
        {
            isObjectDisposed = true;
        }

        if (!isObjectDisposed)
        {
            for (int i = iCustomTaskPanesCount; i > 0; i--)
            {
                CustomTaskPane ctp = Globals.ThisAddIn.CustomTaskPanes[i - 1];
                Globals.ThisAddIn.CustomTaskPanes.Remove(ctp);
            }
        }
    }

    /// <summary>
    /// Remove Custom Pane
    /// </summary>
    public static void RemoveCustomPane(string Title)
    {
        //Todo Remove hard code and move in constant
        Microsoft.Office.Tools.CustomTaskPane ctp = getTaskPaneInstance(Title);

        if (ctp != null)
        {
            Globals.ThisAddIn.CustomTaskPanes.Remove(ctp);
            ctp.Dispose();
            ctp = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Title"></param>
    /// <param name="IsVisible"></param>
    public static void Visible(string Title, bool IsVisible)
    {
        //Todo Remove hard code and move in constant
        Microsoft.Office.Tools.CustomTaskPane ctp = getTaskPaneInstance(Title);

        if (ctp != null)
            ctp.Visible = IsVisible;
    }

    /// <summary>
    /// Get TaskPane Instance
    /// </summary>
    /// <param name="sTitle">Title of the TaskPane</param>
    /// <returns></returns>
    public static CustomTaskPane getTaskPaneInstance(string Title)
    {
        CustomTaskPane taskPane = null;
        bool isObjectDisposed = false;
        int iCustomTaskPanesCount = 0;
        try
        {
            iCustomTaskPanesCount = Globals.ThisAddIn.CustomTaskPanes.Count;
        }
        catch (Exception ex)
        {
            ExceptionLogHelper.ErrorLog(ex);
            isObjectDisposed = true;
        }

        if (!isObjectDisposed)
        {
            foreach (Microsoft.Office.Tools.CustomTaskPane _ctp in Globals.ThisAddIn.CustomTaskPanes)
            {
                if (_ctp.Title.Equals(Title))
                {
                    taskPane = _ctp;
                    break;
                }
            }
        }

        return taskPane;
    }
}

