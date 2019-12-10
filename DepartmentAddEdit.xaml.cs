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
    /// Interaction logic for DepartmentAddEdit.xaml
    /// </summary>
    public partial class DepartmentAddEdit : Window
    {
        public DepartmentAddEdit()
        {
            InitializeComponent();
        }

        private void comboBox_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("faculties", comboBox);
        }

		private void saveDepartment_MouseUp(object sender, EventArgs e)
		{
			try
			{
				if (txtName.Text == null || txtName.Text == "")
				{
					MessageBox.Show("Study program name is required!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}
				if (int.Parse(comboBox.SelectedIndex.ToString()) < 0)
				{
					MessageBox.Show("Please select study program's faculty!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}

				if (txtId.Text == "")
				{
					string values = "'" + txtName.Text +
									"', '" + comboBox.SelectedValue + "'";

					int result = DBConnection.Create("departments", "name, faculty_id", values);

					if (result > 0)
						MessageBox.Show("Successfully created new study program!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else if (Convert.ToInt32(txtId.Text) > 0)
				{
					string values = " name='" + txtName.Text +
									"', faculty_id='" + comboBox.SelectedValue + "'";

					int result = DBConnection.Update("departments", values, Convert.ToInt32(txtId.Text));
					if (result > 0)
						MessageBox.Show("Successfully updated study program!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
				}

				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void close_MouseUp(object sender, EventArgs e)
        {
           Close();
        }

        private void saveDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                saveDepartment_MouseUp(sender, e);
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
