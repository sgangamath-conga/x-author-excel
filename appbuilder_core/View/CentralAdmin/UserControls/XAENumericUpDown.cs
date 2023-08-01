using System;
using System.Windows.Forms;

namespace Apttus.XAuthor.Core
{
    public partial class XAENumericUpDown: UserControl
    {
        private CentralUserControls centralUserControl;
        public event EventHandler NumericUpDownEventHandler;
        public XAENumericUpDown(CentralUserControls centralUserControl)
        {
            InitializeComponent();
            this.centralUserControl = centralUserControl;
        }

        private void XAENumericUpDown_Load(object sender, EventArgs e)
        {
            if (centralUserControl.Equals(CentralUserControls.AppSettings))
            {
                numericUpDown.Maximum = 100;
            }
        }

        public decimal GetValue()
        {
            return numericUpDown.Value;
        }
        public void SetValue(decimal Value)
        {
            numericUpDown.Value = Value;
        }

        private void numericUpDown_ValueChanged(object sender, System.EventArgs e)
        {
            NumericUpDownEventHandler?.Invoke(this, new EventArgs());
        }
    }
}
