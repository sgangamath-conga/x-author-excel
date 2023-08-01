/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Apttus.XAuthor.Core
{
    public partial class WaitWindowView : Form
    {
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public WaitWindowView()
        {
            InitializeComponent();
            SetCultureData();
        }
        private void SetCultureData()
        {
            MessageLabel.Text = resourceManager.GetResource("COREWAITWINDOWVIEW_MessageLabel_Text");
        }
        public WaitWindowView(string StatusMessage, bool TopMostForm)
        {
            InitializeComponent();
            SetCultureData();
            this.MessageLabel.Text = StatusMessage;
            this.TopMost = TopMostForm;
        }

        public string Message
        {
            set
            {
                System.Action action = () =>
                    {
                        MessageLabel.Text = value;
                    };
                if (this.InvokeRequired)
                    this.BeginInvoke(action);
                else
                    MessageLabel.Text = value;
            }
        }

        public void CloseWaitWindow()
        {
            System.Action action = () =>
                {
                    this.Dispose();
                };
            if (this.InvokeRequired)
                this.BeginInvoke(action);
            else
                this.Dispose();
        }

        #region " Threaded wait window "

        //public WaitWindowView(WaitWindow parent)
        //{
        //    //
        //    // The InitializeComponent() call is required for Windows Forms designer support.
        //    //
        //    InitializeComponent();

        //    this._Parent = parent;

        //    //	Position the window in the top right of the main screen.
        //    //this.Top = Screen.PrimaryScreen.WorkingArea.Top + 32;
        //    //this.Left = Screen.PrimaryScreen.WorkingArea.Right - this.Width - 32;
        //}

        //private WaitWindow _Parent;
        //private delegate T FunctionInvoker<T>();
        //internal WaitWindowResult _Result;
        //internal Exception _Error;
        //private IAsyncResult threadResult;

        //protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        //{
        //    base.OnPaint(e);
        //    //	Paint a 3D border
        //    ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, Border3DStyle.Raised);
        //}

        //protected override void OnShown(EventArgs e)
        //{
        //    base.OnShown(e);

        //    //   Create Delegate
        //    FunctionInvoker<WaitWindowResult> threadController = new FunctionInvoker<WaitWindowResult>(this.DoWork);

        //    //   Execute on secondary thread.
        //    this.threadResult = threadController.BeginInvoke(this.WorkComplete, threadController);
        //}

        //internal WaitWindowResult DoWork()
        //{
        //    //	Invoke the worker method and return any results.
        //    WaitWindowEventArgs e = new WaitWindowEventArgs(this._Parent, this._Parent._Args);
        //    if ((this._Parent._WorkerMethod != null))
        //    {
        //        this._Parent._WorkerMethod(this, e);
        //    }
        //    return e.Result;
        //}

        //private void WorkComplete(IAsyncResult results)
        //{
        //    if (!this.IsDisposed)
        //    {
        //        if (this.InvokeRequired)
        //        {
        //            this.Invoke(new WaitWindow.MethodInvoker<IAsyncResult>(this.WorkComplete), results);
        //        }
        //        else
        //        {
        //            //	Capture the result
        //            try
        //            {
        //                this._Result = ((FunctionInvoker<WaitWindowResult>)results.AsyncState).EndInvoke(results);
        //            }
        //            catch (Exception ex)
        //            {
        //                //	Grab the Exception for rethrowing after the WaitWindow has closed.
        //                this._Error = ex;
        //            }
        //            this.Close();
        //        }
        //    }
        //}

        //public void SetMessage(string message)
        //{
        //    this.MessageLabel.Text = message;
        //}

        //public void Cancel()
        //{
        //    this.Invoke(new MethodInvoker(this.Close), null);
        //}        

        #endregion
    }
}
