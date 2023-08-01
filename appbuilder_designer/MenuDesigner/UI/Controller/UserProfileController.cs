/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
  public  class UserProfileController
    {
        private PermissionUI PermissionUI;
        public UserProfileController()
        {
            
           
        }
        public PermissionUI UI
        {
            set;
            get;

        }

        public void LoadData(List<ApttusUserProfile> Profiles)
        {
            ABSalesforceAdapter salesForceAdapter = new ABSalesforceAdapter();
            List<ApttusUserProfile> profile = salesForceAdapter.GetUserProfiles();

            ((ListBox)UI.lstPermissionUI).DataSource = profile;

            ((ListBox)UI.lstPermissionUI).DisplayMember = "UserName";
            ((ListBox)UI.lstPermissionUI).ValueMember = "USerId";

            if (Profiles != null)
            {
                for (int i = 0; i <= Profiles.Count - 1; i++)
                {
                    int index = ((ListBox)UI.lstPermissionUI).FindString(Profiles[i].UserName);
                    if (index >= 0)
                    {
                        (UI.lstPermissionUI).SetItemChecked(index, true);


                    }

                }
            }
  //chkboxlist1.Items.FindByValue(dtDataTable.Rows(i).Item  ("Name")).Selected = True
//Next
        }
        

        public void SaveCheckedtems()
        {
            List<ApttusUserProfile> SelectedItems = new List<ApttusUserProfile>();
            for (int indexChecked = 0; indexChecked < UI.lstPermissionUI.CheckedItems.Count; indexChecked ++ )
            {
                // The indexChecked variable contains the index of the item.
                ApttusUserProfile prof = UI.lstPermissionUI.CheckedItems[indexChecked] as ApttusUserProfile;
                SelectedItems.Add(prof);

            }
            UI.Profiles = SelectedItems;
        }
    }
}
