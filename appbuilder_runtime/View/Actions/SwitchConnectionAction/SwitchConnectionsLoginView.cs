/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppRuntime
{
    public partial class SwitchConnectionsLoginView : Form
    {
        public SwitchConnectionsLoginViewController controller;
        ConnectionManager connectionManager = ConnectionManager.GetInstance;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public SwitchConnectionsLoginView()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppRuntime.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.RUNTIME_PRODUCT_NAME;

            connectionManager.connectionInstances.Clear();
        }
        private void SetCultureData()
        {
            btnClose.Text = resourceManager.GetResource("SWITCHCONNVIEW_btnClose_Text");
            groupBox1.Text = resourceManager.GetResource("SWITCHCONNVIEW_groupBox1_Text");
            lblTopMessage.Text = resourceManager.GetResource("SWITCHCONNVIEW_lblTopMessage_Text");
            lblTitle.Text = resourceManager.GetResource("RIBBON_InvalConnetionCap_ErrorMsg");
        }
        public void SetController(SwitchConnectionsLoginViewController controller)
        {
            this.controller = controller;
        }

        private void SwitchConnectionsLoginView_Load(object sender, EventArgs e)
        {
            int cnt = 0;
            foreach (SwitchConnectionActionModel conn in controller.connections)
            {
                ConnectionEntry connEntry = new ConnectionEntry(conn, this);
                tblConnections.Controls.Add(connEntry, 0, cnt++);
                tblConnections.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            // Hide horizontal scroll bar
            int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
            tblConnections.Padding = new Padding(0, 0, vertScrollWidth, 0);
        }

        private void SwitchConnectionsLoginView_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = CheckConnections();
        }

        internal bool CheckConnections()
        {
            if (connectionManager.connectionInstances.Count != this.controller.connections.Count)
            {
                TaskDialogResult dResult = ApttusMessageUtil.ShowWarning(resourceManager.GetResource("SWITCHCONLOGINVIEW_CheckConnections_WarnMsg"), Constants.RUNTIME_PRODUCT_NAME, ApttusMessageUtil.YesNo);
                if (dResult == TaskDialogResult.Yes)
                    return false;
                else if (dResult == TaskDialogResult.No)
                    return true;
            }
            else if (connectionManager.connectionInstances.Count == this.controller.connections.Count)
            {
                controller.allConnectionsLoggedIn = true;
            }
            return false;
        }

    }
}
