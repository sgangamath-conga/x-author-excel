/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */

namespace Apttus.XAuthor.Core
{
    public class AddCustomRowsController
    {
        private AddCustomRowsView view;
        public int result { get; set; }

        public AddCustomRowsController(AddCustomRowsView view)
        {
            result = 0;
            this.view = view;
            this.view.SetController(this);
        }

        public int GetCustomRows()
        {
            view.ShowDialog();
            return result;
        
        }
    }
}
