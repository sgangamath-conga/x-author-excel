using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class DataMigrationHelp : Form
    {
        DataMigrationModel Model;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        public DataMigrationHelp()
        {
            InitializeComponent();
            SetCultureData();
            this.Icon = global::Apttus.XAuthor.AppDesigner.Properties.Resources.XA_AppBuilder_Icon;
            this.Text = Constants.DESIGNER_PRODUCT_NAME;
        }

        private void SetCultureData()
        {
            //lblSettings.Text = resourceManager.GetResource("COMMON_Settings_Text");
            btnOK.Text = resourceManager.GetResource("MATRIXFIELDVIEW_btnOK_Text");
            richTextBox1.Text = resourceManager.GetResource("DM_Help_Text"); 
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DataMigrationSettings_Load(object sender, EventArgs e)
        {

        }
    }
}
