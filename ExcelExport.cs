using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ccbt;
using ccbt.Models;
using System.Windows;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace UniversityTimetabling
{
    class ExcelExport
    {
        public void main()
        {
        }

        public void exportToExcel(System.Windows.Controls.DataGrid grid, string fileName = "Excel")
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Documents (*.xls)|*.xls";
                sfd.FileName = fileName + ".xls";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Copy DataGridView results to clipboard
                    copyAlltoClipboard(grid);

                    object misValue = System.Reflection.Missing.Value;
                    Microsoft.Office.Interop.Excel.Application xlexcel = new Microsoft.Office.Interop.Excel.Application();

                    xlexcel.DisplayAlerts = false; // Without this you will get two confirm overwrite prompts
                    Microsoft.Office.Interop.Excel.Workbook xlWorkBook = xlexcel.Workbooks.Add(misValue);
                    Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    // Format column D as text before pasting results, this was required for my data
                    Microsoft.Office.Interop.Excel.Range rng = xlWorkSheet.get_Range("D:D").Cells;
                    rng.NumberFormat = "@";

                    // Paste clipboard results to worksheet range
                    Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, 1];
                    CR.Select();
                    xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);

                    // For some reason column A is always blank in the worksheet. ¯\_(ツ)_/¯
                    // Delete blank column A and select cell A1
                    Microsoft.Office.Interop.Excel.Range delRng = xlWorkSheet.get_Range("A:A").Cells;
                    delRng.Delete(Type.Missing);
                    xlWorkSheet.get_Range("A1").Select();

                    // Save the excel file under the captured location from the SaveFileDialog
                    xlWorkBook.SaveAs(sfd.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlexcel.DisplayAlerts = true;
                    xlWorkBook.Close(true, misValue, misValue);
                    xlexcel.Quit();

                    releaseObject(xlWorkSheet);
                    releaseObject(xlWorkBook);
                    releaseObject(xlexcel);

                    // Clear Clipboard and DataGridView selection
                    System.Windows.Clipboard.Clear();
                    // roomsGrid.ClearSelection();

                    // Open the newly saved excel file
                    if (File.Exists(sfd.FileName))
                        System.Diagnostics.Process.Start(sfd.FileName);
                }

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
        }

        private void copyAlltoClipboard(System.Windows.Controls.DataGrid grid)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                if (grid.Columns[i].Visibility != Visibility.Hidden)
                {
                    if (i > 0)
                        builder.Append("\t");
                    builder.Append(grid.Columns[i].Header);
                }
            }
            builder.Append(Environment.NewLine);

            if (grid.Items is IList)
            {
                for (int i = 0; i < grid.Items.Count; i++)
                {
                    System.Data.DataRowView item = (System.Data.DataRowView)grid.Items[i];
                    Type objectType = item.GetType();
                    for (int j = 0; j < grid.Columns.Count; j++)
                    {
                        if (grid.Columns[j].Visibility != Visibility.Hidden)
                        {
                            string colName = grid.Columns[j].Header.ToString().Replace(" ", "_");

                            if (j > 0)
                                builder.Append("\t");
                            builder.Append(item[colName]);
                        }
                    }
                    builder.Append(Environment.NewLine);
                }
            }
            else
                throw new NotSupportedException();

            System.Windows.Clipboard.SetDataObject(builder.ToString(), true);
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                System.Windows.MessageBox.Show("Exception Occurred while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
