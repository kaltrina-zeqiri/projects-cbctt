using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Office.Interop;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace UniversityTimetabling
{
    /// <summary>
    /// Interaction logic for Rooms.xaml
    /// </summary>
    public partial class Rooms : Window
    {
        public Rooms()
        {
            InitializeComponent();
        }

        private void rooms_Initialized(object sender, EventArgs e)
        {
            string Columns = " r.id AS id, r.name AS name, r.size AS size, r.code AS code, b.name AS building, r.note AS note ";
            string Table = " rooms r JOIN buildings b ON r.building_id = b.id";
            roomsGrid.ItemsSource = DBConnection.Select(Table, Columns, null, null, " r.name ").DefaultView;
        }
        

private void btnExportToExcel_Click(object sender, EventArgs e)
    {
        SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "Excel Documents (*.xls)|*.xls";
        sfd.FileName = "Inventory_Adjustment_Export.xls";
        if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            // Copy DataGridView results to clipboard
            copyAlltoClipboard();

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

    private void copyAlltoClipboard()
    {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < roomsGrid.Columns.Count; i++)
            {
                if (i > 0)
                    builder.Append("\t");
                builder.Append(roomsGrid.Columns[i].Header);
            }
            builder.Append(Environment.NewLine);

            if (roomsGrid.Items is IList)
            {
                for (int i = 0; i < roomsGrid.Items.Count; i++)
                {
                    object item = roomsGrid.Items[i];
                    Type objectType = item.GetType();
                    for (int j = 0; j < roomsGrid.Columns.Count; j++)
                    {
                        string colName = roomsGrid.Columns[j].Header.ToString();
                        PropertyInfo propInfo = objectType.GetProperty(colName);
                        if (propInfo == null)
                            continue;
                        object propertyValue = propInfo.GetValue(item, null);
                        if (j > 0)
                            builder.Append("\t");
                        builder.Append(propertyValue.ToString());
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

    private void editRoom_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)roomsGrid.SelectedItem;
            if (row != null)
            {
                int id = (int)row["ID"];
                string name = (string)row["Name"];
                int size = (int)row["Size"];
                string code = (string)row["Code"];
                string building = (string)row["Building"];
                string note = (string)row["Note"];

                RoomAddEdit addEdit = new RoomAddEdit();
                addEdit.Closed += (s, eventarg) =>
                {
                    rooms_Initialized(s, eventarg);
                };
                addEdit.Owner = this;
                addEdit.txtId.Text = id.ToString();
                addEdit.txtName.Text = name;
                addEdit.txtSize.Text = size.ToString();
                addEdit.txtCode.Text = code;
                addEdit.txtNote.Text = note;
                addEdit.comboBox.Text = building;
                addEdit.ShowDialog();
            }
            else
                System.Windows.MessageBox.Show("Please select a row to edit!", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void deleteRoom_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.btnExportToExcel_Click(sender, e);
          /*  DataRowView row = (DataRowView)roomsGrid.SelectedItem;
            if (row != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to delete this room?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    int id = (int)row["ID"];
                    int result = DBConnection.Delete("rooms", id);
                    if (result > 0)
                        MessageBox.Show("Room has been successfully deleted!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    rooms_Initialized(sender, e);
                }
            }
            else
                MessageBox.Show("Please select a row to delete!", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
    */   
    }

        private void addRoom_MouseUp(object sender, MouseButtonEventArgs e)
        {
            RoomAddEdit addEdit = new RoomAddEdit();
            addEdit.Closed += (s, eventarg) =>
            {
                rooms_Initialized(s, eventarg);
            };
            addEdit.Owner = this;
            addEdit.ShowDialog();
        }

        private void rooms_DataChanged(object sender, EventArgs e)
        {
            rooms_Initialized(sender, e);
        }

        private void refreshRooms_MouseUp(object sender, MouseButtonEventArgs e)
        {
            rooms_Initialized(sender, e);
        }

        private void comboBoxBuildings_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("buildings", comboBoxBuildings);
        }

        private void clearFilterRoom_MouseUp(object sender, MouseButtonEventArgs e)
        {
            rooms_Initialized(sender, e);
            comboBoxBuildings.SelectedIndex = -1;
            txtFilterByName.Text = null;
            txtFilterBySize.Text = null;
        }

        private void txtFilterByName_TextChanged(object sender, TextChangedEventArgs e)
        {
            filter();
        }

        private void txtFilterByName_TextChanged(object sender, SelectionChangedEventArgs e)
        {
            filter();
        }

        private void filter()
        {
            string name = txtFilterByName.Text;
            int size = (txtFilterBySize.Text != "") ? Convert.ToInt32(txtFilterBySize.Text) : 0;
            int building = (comboBoxBuildings.SelectedIndex >= 0) ? Convert.ToInt32(comboBoxBuildings.SelectedValue.ToString()) : 0;

            string Columns = " r.id AS id, r.name AS name, r.size AS size, r.code AS code, b.name AS building, r.note AS note ";
            string Table = " rooms r JOIN buildings b ON r.building_id = b.id";
            string Conditions = null;

            if (name != "") Conditions = " r.name LIKE '%" + name + "%' ";
            if (size > 0) Conditions += ((Conditions != null) ? " AND " : " ") + "r.size >= '" + size + "' ";
            if (building > 0) Conditions += ((Conditions != null) ? " AND " : " ") + "r.building_id = '" + building + "' ";

            roomsGrid.ItemsSource = DBConnection.Select(Table, Columns, Conditions).DefaultView;
        }

        private void roomsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            editRoom_MouseUp(sender, e);
        }
    }
}
