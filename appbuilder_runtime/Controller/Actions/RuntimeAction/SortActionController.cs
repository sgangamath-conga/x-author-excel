/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using Apttus.XAuthor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace Apttus.XAuthor.AppRuntime
{
    public class SortActionController
    {
        ConfigurationManager configurationManager = ConfigurationManager.GetInstance;
        Excel.Range Selection = null;

        public bool bCancelSort = false;
        public string CancelReason = string.Empty;
        ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;

        public SortActionController(Excel.Range Selection)
        {
            this.Selection = Selection;
        }

        public void HandleSort()
        {
            List<string> OverlappingRanges = ExcelHelper.GetOverlapingNamedRange(Selection);
            var XAuthorRanges = (from rangeMap in configurationManager.GetRangeMaps()
                                 where OverlappingRanges.Contains(rangeMap.RangeName)
                                 select rangeMap);
            var XAuthorRangesCount = (from r in XAuthorRanges
                                      select r.RangeName).Distinct().Count();

            if (XAuthorRangesCount == 1)
            {
                RangeMap SingleRangeMap = XAuthorRanges.FirstOrDefault();

                if (SingleRangeMap.Type == ObjectType.Repeating)
                {
                    Excel.Range SingleXAuthorRange = ExcelHelper.GetRange(SingleRangeMap.RangeName);
                    // Below condition checks if the left and right of full range is selected and 
                    // the selection is contained within the X-Author Range.
                    if (((Excel.Range)Selection.Cells[1, 1]).Column == ((Excel.Range)SingleXAuthorRange.Cells[1, 1]).Column &
                        ((Excel.Range)Selection.Cells[1, Selection.Columns.Count]).Column == ((Excel.Range)SingleXAuthorRange.Cells[1, SingleXAuthorRange.Columns.Count]).Column &
                        ((Excel.Range)Selection.Cells[1, 1]).Row >= ((Excel.Range)SingleXAuthorRange.Cells[1, 1]).Row &
                        ((Excel.Range)Selection.Cells[Selection.Rows.Count, 1]).Row <= ((Excel.Range)SingleXAuthorRange.Cells[SingleXAuthorRange.Rows.Count, 1]).Row)
                    {
                        // Correct Sort Range selected.
                        bCancelSort = false;
                    }
                    // Below condition checks if the selection can be expand to include the full range.
                    else if (((Excel.Range)Selection.Cells[1, 1]).Column >= ((Excel.Range)SingleXAuthorRange.Cells[1, 1]).Column &
                        ((Excel.Range)Selection.Cells[1, Selection.Columns.Count]).Column <= ((Excel.Range)SingleXAuthorRange.Cells[1, SingleXAuthorRange.Columns.Count]).Column &
                        ((Excel.Range)Selection.Cells[1, 1]).Row >= ((Excel.Range)SingleXAuthorRange.Cells[1, 1]).Row &
                        ((Excel.Range)Selection.Cells[Selection.Rows.Count, 1]).Row <= ((Excel.Range)SingleXAuthorRange.Cells[SingleXAuthorRange.Rows.Count, 1]).Row)
                    {
                        // Auto Expand the Sort Range.
                        Excel.Worksheet oSheet = Selection.Worksheet;
                        Excel.Range topLeft = ExcelHelper.NextHorizontalCell((Excel.Range)Selection.Cells[1, 1], (-1) * SingleXAuthorRange.Cells[1, 1].Column - Selection.Cells[1, 1].Column);
                        Excel.Range bottomRight = ExcelHelper.NextHorizontalCell((Excel.Range)Selection.Cells[Selection.Rows.Count, Selection.Columns.Count], SingleXAuthorRange.Cells[1, SingleXAuthorRange.Columns.Count].Column - Selection.Cells[1, Selection.Columns.Count].Column);
                        Excel.Range NewSelection = oSheet.get_Range(topLeft, bottomRight);
                        NewSelection.Select();
                        bCancelSort = false;
                    }
                    else
                    {
                        CancelReason = resourceManager.GetResource("SORTACTIONCTL_RangeSelectionRequired_ErrorMsg");
                        bCancelSort = true;
                    }
                }
                else
                {
                    // Independent and CrossTab
                    CancelReason = resourceManager.GetResource("SORTACTIONCTL_RangeCannotSort_ErrorMsg");
                    bCancelSort = true;
                }
            }
            else if (XAuthorRangesCount == 0)
            {
                bCancelSort = false;
            }
            else
            {
                CancelReason = resourceManager.GetResource("SORTACTIONCTL_MoreCellAffected_ErrorMsg");
                bCancelSort = true;
            }

            if (bCancelSort)
                ApttusMessageUtil.ShowError(string.Format(resourceManager.GetResource("SORTACTIONCTL_SortCancel_ErrorMsg"), CancelReason), resourceManager.GetResource("SORTACTIONCTL_SortCancelCap_ErrorMsg"));

        }
    }
}
