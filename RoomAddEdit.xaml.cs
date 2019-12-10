using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;

namespace UniversityTimetabling
{
    /// <summary>
    /// Interaction logic for RoomAddEdit.xaml
    /// </summary>
    public partial class RoomAddEdit : Window
    {
        public RoomAddEdit()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            int val;
            Regex regex = new Regex("[^0-9]+");
            e.Handled = (regex.IsMatch(e.Text) || !int.TryParse((((TextBox)sender).Text + e.Text), out val) || val < 1 || val > 250);
        }

        private void comboBox_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("buildings", comboBox);
        }

        private void saveRoom_MouseUp(object sender, EventArgs e)
        {
			try
			{
				if (txtName.Text == null || txtName.Text == "")
				{
					MessageBox.Show("Room name is required!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}
				if (txtSize.Text == null || txtSize.Text == "")
				{
					MessageBox.Show("Room size is required!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}
				if (int.Parse(comboBox.SelectedIndex.ToString()) < 0)
				{
					MessageBox.Show("Please select building!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}

				if (txtId.Text == "")
				{
					string code = null;
					for (int i = 1; i < 1000; i++)
					{
						code = "r" + i.ToString("000");
						DataView data = DBConnection.Select("rooms", "'*'", " code = '" + code + "'").DefaultView;
						if (data.Count == 0) break;
					}

					string values = "'" + txtName.Text +
									"', '" + Convert.ToInt32(txtSize.Text) +
									"', '" + code.ToString() +
									"', '" + comboBox.SelectedValue +
									"', '" + txtNote.Text + "'";

					int result = DBConnection.Create("rooms", "name, size, code, building_id, note", values);

					if (result > 0)
						MessageBox.Show("New room has been successfully created!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else if (Convert.ToInt32(txtId.Text) > 0)
				{
					string values = " name='" + txtName.Text +
									"', size='" + Convert.ToInt32(txtSize.Text) +
									"', code='" + txtCode.Text +
									"', building_id='" + comboBox.SelectedValue +
									"', note='" + txtNote.Text + "'";

					int result = DBConnection.Update("rooms", values, Convert.ToInt32(txtId.Text));
					if (result > 0)
						MessageBox.Show("Room has been successfully updated!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
				}

				Close();
			} catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
        }

        private void close_MouseUp(object sender, EventArgs e)
        {
           Close();
        }

        private void saveRoom_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                saveRoom_MouseUp(sender, e);
                Keyboard.ClearFocus();
            }
        }

        private void close_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                close_MouseUp(sender, e);
                Keyboard.ClearFocus();
            }
        }
    }
}
