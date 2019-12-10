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
using System.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UniversityTimetabling
{
    /// <summary>
    /// Interaction logic for CourseDepartmentRelAddEdit.xaml
    /// </summary>
    public partial class CourseDepartmentRelAddEdit : Window
    {
        public CourseDepartmentRelAddEdit()
        {
            InitializeComponent();
        }

        private void comboBoxCourse_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("courses", comboBoxCourse, "name", "parent_id IS NOT NULL", " name ");
        }

        private void comboBoxDepartment_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("departments", comboBoxDepartment);
        }

        private void saveCourseDepartmentRel_MouseUp(object sender, EventArgs e)
        {
			try
			{
				if (int.Parse(comboBoxCourse.SelectedIndex.ToString()) < 0)
				{
					MessageBox.Show("Please select course!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}
				if (int.Parse(comboBoxDepartment.SelectedIndex.ToString()) < 0)
				{
					MessageBox.Show("Please select department!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}

				var course_id = comboBoxCourse.SelectedValue;
				var department_id = comboBoxDepartment.SelectedValue;

				DataView data = DBConnection.Select("course_department_rel", "'*'", " course_id = '" + course_id + "' AND department_id='" + department_id + "'").DefaultView;
				if (data.Count != 0)
				{
					MessageBox.Show("This course is already assigned to the selected department!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}

				string values = "'" + course_id + "', '" + department_id + "'";

				int result = DBConnection.Create("course_department_rel", "course_id, department_id", values);
				if (result > 0)
					MessageBox.Show("Successfully created new course department assignment!", "Information", MessageBoxButton.OK);

				Close();
			} catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
			}
		}

        private void close_MouseUp(object sender, EventArgs e)
        {
            Close();
        }

        private void saveCourseDepartmentRel_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                saveCourseDepartmentRel_MouseUp(sender, e);
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