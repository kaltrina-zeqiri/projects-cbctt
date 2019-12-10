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
    /// Interaction logic for CourseTeacherRelation.xaml
    /// </summary>
    public partial class CourseTeacherRelation : Window
    {
        public CourseTeacherRelation()
        {
            InitializeComponent();
        }

        private void courseTeacherRel_Initialized(object sender, EventArgs e)
        {
            string Columns = " c.id AS course_id, c.name AS course_name," +
				" CASE WHEN c.detail = 'lecture' THEN 'Lecture' " +
				" WHEN c.detail = 'numeric' THEN 'Numeric Exercise' " +
				" WHEN c.detail = 'laboratory' THEN 'Laboratory Exercise'" +
				" ELSE '' END AS course_type, " +
				"t.id AS teacher_id, concat(t.name, ' ', t.surname) AS teacher_name";

            string From = "courses c" +
                " LEFT JOIN course_teacher_rel ctr ON c.id=ctr.course_id " +
                            " JOIN teachers t ON ctr.teacher_id=t.id ";
            courseteacherGrid.ItemsSource = DBConnection.Select(From, Columns, null, null, " c.name ").DefaultView;
        }

        private void addCourseTeacherRel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CourseTeacherRelAddEdit addeditRelation = new CourseTeacherRelAddEdit();
            addeditRelation.Owner = this;
            addeditRelation.Closed += (s, eventarg) =>
            {
				txtFilterByName_TextChanged(s, eventarg);
            };
			DataRowView row = (DataRowView)courseteacherGrid.SelectedItem;
			if (row != null)
			{
				int id;
				bool parsed = int.TryParse(row["Teacher_ID"].ToString(), out id);
				if (!parsed || (parsed && (int)row["Teacher_ID"] <= 0))
				{
					addeditRelation.comboBoxCourse.SelectedValue = (int)row["Course_ID"];
				}
			}
			addeditRelation.ShowDialog();
        }
       
        private void deleteCourseTeacherRel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)courseteacherGrid.SelectedItem;
            if (row != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to delete?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    int teacher_id = (int)row["Teacher_ID"],
                    course_id = (int)row["Course_ID"];
                    int result = DBConnection.Delete("course_teacher_rel", 0, " course_id = '" + course_id + "' AND teacher_id = '" + teacher_id + "'");
                    if (result > 0)
                        MessageBox.Show("Successfully deleted Course Teacher Relation!", "Information", MessageBoxButton.OK);

                    courseTeacherRel_Initialized(sender, e);
                }
            }
            else
                MessageBox.Show("Please select a row to delete!", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void refreshCourseTeacherRel_MouseUp(object sender, MouseButtonEventArgs e)
        {
			checkBox.IsChecked = false;
			txtFilterByName_TextChanged(sender, e);
        }

        private void txtFilterByName_TextChanged(object sender, EventArgs e)
        {
			string name = txtFilterByName.Text;
            string teacher = txtFilterByTeacher.Text;

            string Columns = " c.id AS course_id, c.name AS course_name," +
				" CASE WHEN c.detail = 'lecture' THEN 'Lecture' " +
				" WHEN c.detail = 'numeric' THEN 'Numeric Exercise' " +
				" WHEN c.detail = 'laboratory' THEN 'Laboratory Exercise'" +
				" ELSE '' END AS course_type, " +
				" t.id AS teacher_id, concat(t.name, ' ', t.surname) AS teacher_name";
            string From = "courses c" +
                " LEFT JOIN course_teacher_rel ctr ON c.id=ctr.course_id " +
                            " LEFT JOIN teachers t ON ctr.teacher_id=t.id ";
            string Conditions = " c.parent_id IS NOT NULL ";

            if (name != "")
                Conditions += " AND c.name LIKE '%" + name + "%' ";
            if (teacher != "")
                Conditions +=  " AND concat(t.name, ' ', t.surname) LIKE '%" + teacher + "%'";
			if (checkBox.IsChecked == true)
				Conditions += " AND ctr.teacher_id IS NULL ";

			courseteacherGrid.ItemsSource = DBConnection.Select(From, Columns, Conditions, null, " c.name ").DefaultView;
        }

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			string Columns = " c.id AS course_id, c.name AS course_name, " +
				" CASE WHEN c.detail = 'lecture' THEN 'Lecture' " +
				" WHEN c.detail = 'numeric' THEN 'Numeric Exercise' " +
				" WHEN c.detail = 'laboratory' THEN 'Laboratory Exercise'" +
				" ELSE '' END AS course_type, " +
				" CASE WHEN t.id IS NULL THEN '-' ELSE t.id END AS teacher_id, " +
				" CASE WHEN t.name IS NULL THEN '-' ELSE concat(t.name, ' ', t.surname) END AS teacher_name";
			string From = "courses c" +
				" LEFT JOIN course_teacher_rel ctr ON c.id=ctr.course_id " +
							" LEFT JOIN teachers t ON ctr.teacher_id=t.id ";
			string Conditions = " c.parent_id IS NOT NULL AND ctr.teacher_id IS NULL ";

			courseteacherGrid.ItemsSource = DBConnection.Select(From, Columns, Conditions, null, " c.name ").DefaultView;
		}

		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			txtFilterByName_TextChanged(sender, e);
		}

		private void CourseteacherGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			addCourseTeacherRel_MouseUp(sender, e);
		}
	}
}