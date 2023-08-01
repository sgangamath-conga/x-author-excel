/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Apttus.XAuthor.Core;

namespace Apttus.XAuthor.AppDesigner
{
    public interface IGenericListUIController
    {
        List<GenericListUIModel> Model { get; set; }

        void SetView();

        void Add();

        void Edit(DataGridViewSelectedRowCollection rows);

        void Delete(DataGridViewSelectedRowCollection rows);

    }
}
