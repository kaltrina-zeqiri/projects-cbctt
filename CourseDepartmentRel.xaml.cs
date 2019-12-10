using System;
using System.Collections.Generic;
using System.Data;
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

namespace UniversityTimetabling
{
    /// <summary>
    /// Interaction logic for CourseDepartmentRel.xaml
    /// </summary>
    public partial class CourseDepartmentRel : Window
    {
        public CourseDepartmentRel()
        {
            InitializeComponent();
        }

        private void courseDepartmentRel_Initialized(object sender, EventArgs e)
        {
            string Columns = " c.id AS course_id, c.name AS course_name," +
				" CASE WHEN c.detail = 'lecture' THEN 'Lecture' " +
				" WHEN c.detail = 'numeric' THEN 'Numeric Exercise' " +
				" WHEN c.detail = 'laboratory' THEN 'Laboratory Exercise'" +
				" ELSE '' END AS course_type, " +
				" d.id AS department_id, d.name AS department_name";

            string From = "courses c" +
                " LEFT JOIN course_department_rel cdr ON c.id=cdr.course_id " +
                " LEFT JOIN departments d ON cdr.department_id=d.id ";
            courseDepartmentRelGrid.ItemsSource = DBConnection.Select(From, Columns, " c.parent_id IS NOT NULL ", null, " c.name ").DefaultView;
        }

        private void addCourseDepartmentRel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CourseDepartmentRelAddEdit addeditRelation = new CourseDepartmentRelAddEdit();
            addeditRelation.Owner = this;
            addeditRelation.Closed += (s, eventarg) =>
            {
                txtFilterByName_TextChanged(s, eventarg);
            };
			DataRowView row = (DataRowView)courseDepartmentRelGrid.SelectedItem;
			if (row != null)
			{
				int id;
				bool parsed = int.TryParse(row["Department_ID"].ToString(), out id);
				if (!parsed || (parsed && (int)row["Department_ID"] <= 0))
				{
					addeditRelation.comboBoxCourse.SelectedValue = (int)row["Course_ID"];
				}
			}
			addeditRelation.ShowDialog();
        }

        private void deleteCourseDepartmentRel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)courseDepartmentRelGrid.SelectedItem;
            if (row != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to delete?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    int department_id = (int)row["Department_ID"],
                    course_id = (int)row["Course_ID"];
                    int result = DBConnection.Delete("course_department_rel", 0, " course_id = '" + course_id + "' AND department_id = '" + department_id + "'");
                    if (result > 0)
                        MessageBox.Show("Successfully deleted course department assignment!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    courseDepartmentRel_Initialized(sender, e);
                }
            }
            else
                MessageBox.Show("Please select a row to delete!", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void refreshCourseDepartmentRel_MouseUp(object sender, MouseButtonEventArgs e)
        {
			checkBox.IsChecked = false;
			txtFilterByName_TextChanged(sender, e);
        }

		private void txtFilterByName_TextChanged(object sender, EventArgs e)
		{
			string name = txtFilterByName.Text;
			string department = txtFilterByDepartment.Text;

			string Columns = " c.id AS course_id, c.name AS course_name, " +
				" CASE WHEN c.detail = 'lecture' THEN 'Lecture' " +
				" WHEN c.detail = 'numeric' THEN 'Numeric Exercise' " +
				" WHEN c.detail = 'laboratory' THEN 'Laboratory Exercise'" +
				" ELSE '' END AS course_type, " +
				"d.id AS department_id, d.name AS department_name";
			string From = "courses c" +
			   " LEFT JOIN course_department_rel cdr ON c.id=cdr.course_id " +
			   " LEFT JOIN departments d ON cdr.department_id=d.id ";
			string Conditions = " c.parent_id IS NOT NULL ";

			if (name != "")
				Conditions += " c.name LIKE '%" + name + "%' ";
			if (department != "")
				Conditions += " AND d.name LIKE '%" + department + "%'";
			if (checkBox.IsChecked == true)
				Conditions += " AND cdr.department_id IS NULL ";
			courseDepartmentRelGrid.ItemsSource = DBConnection.Select(From, Columns, Conditions, null, " c.name ").DefaultView;
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			string Columns = " c.id AS course_id, c.name AS course_name, " +
				" CASE WHEN c.detail = 'lecture' THEN 'Lecture' " +
				" WHEN c.detail = 'numeric' THEN 'Numeric Exercise' " +
				" WHEN c.detail = 'laboratory' THEN 'Laboratory Exercise'" +
				" ELSE '' END AS course_type, " +
				" CASE WHEN d.id IS NULL THEN '-' ELSE d.id END AS department_id, " +
				" CASE WHEN d.name IS NULL THEN '-' ELSE d.name END AS department_name";
			string From = "courses c" +
			   " LEFT JOIN course_department_rel cdr ON c.id=cdr.course_id " +
			   " LEFT JOIN departments d ON cdr.department_id=d.id ";
			string Conditions = " c.parent_id IS NOT NULL AND cdr.department_id IS NULL ";

			courseDepartmentRelGrid.ItemsSource = DBConnection.Select(From, Columns, Conditions, null, " c.name ").DefaultView;
		}

		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			txtFilterByName_TextChanged(sender, e);
		}

		private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			addCourseDepartmentRel_MouseUp(sender, e);
		}
	}
}