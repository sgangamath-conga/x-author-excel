using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public partial class ucObjectField : UserControl
    {
        ApplicationDefinitionManager applicationDefinitionManager = ApplicationDefinitionManager.GetInstance;
        static string selectedNode = string.Empty;
        public ucObjectField()
        {
            InitializeComponent();
          
           
            //
            // Create two child nodes and put them in an array.
            // ... Add the third node, and specify these as its children.
            //
            TreeNode node2 = new TreeNode("Account Name");
            TreeNode node3 = new TreeNode("Account Number");
            TreeNode node4 = new TreeNode("Billing Address");
            TreeNode node5 = new TreeNode("Description");
            TreeNode node6 = new TreeNode("Employees");
             


            TreeNode[] array = new TreeNode[] { node2, node3, node4 , node5 , node6  };
            

            
            //
            // Final node.
            //
            TreeNode treeNode = new TreeNode("Account", array);
            tvObjectField.Nodes.Add(treeNode);



            TreeNode node21 = new TreeNode("Opportunity Name");
            TreeNode node31 = new TreeNode("Amount");
            TreeNode node41 = new TreeNode("Description");
            TreeNode node51 = new TreeNode("Quantity");
            TreeNode node61 = new TreeNode("Expected Revenue");
            TreeNode node71 = new TreeNode("Opportunity Owner");
            TreeNode node81 = new TreeNode("Close Date");
            TreeNode node91 = new TreeNode("Forecast Category");
            

            TreeNode[] array1 = new TreeNode[] { node21, node31, node41, node51, node61, node71, node81, node91 };
            //
            // Final node.
            //
            TreeNode treeNode1 = new TreeNode("Opportunity", array1);
            tvObjectField.Nodes.Add(treeNode1);
           

            TreeNode node211 = new TreeNode("Company");
            TreeNode node311 = new TreeNode("Campaign");
           

            TreeNode[] array11 = new TreeNode[] { node211, node311 };
            //
            // Final node.
            //
            TreeNode treeNode111 = new TreeNode("Lead", array11);
            tvObjectField.Nodes.Add(treeNode111);

            //RenderApplicationObjects("Account");


            ////// Example list.
            ////List<string[]> list = new List<string[]>();
            ////list.Add(new string[] { "Column 1", "Column 2", "Column 3" });
            ////list.Add(new string[] { "Row 2", "Row 2" });
            ////list.Add(new string[] { "Row 3" });

            ////// Convert to DataTable.
            ////DataTable table = ConvertListToDataTable(list);
            ////dataGridView1.DataSource = table;

        }


       

        
        private void tvObjectField_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                StringBuilder sbDragData = new StringBuilder();
                int itemCount = this.tvObjectField.Nodes.Count;

                // Retrieve the client coordinates of the mouse position.
                Point targetPoint = tvObjectField.PointToClient(new Point(e.X, e.Y));

                // Select the node at the mouse position.
                tvObjectField.SelectedNode = tvObjectField.GetNodeAt(targetPoint);

                //TreeNode node = tvObjectField.SelectedNode.Text;
                sbDragData.Append(selectedNode);

                this.tvObjectField.DoDragDrop(sbDragData.ToString(),
                DragDropEffects.Copy);
            }
        }
        private void tvObjectField_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Move the dragged node when the left mouse button is used. 
            if (e.Button == MouseButtons.Left)
            {
                selectedNode = ((System.Windows.Forms.TreeNode)(e.Item)).Text;
            }
        }
    }
}
