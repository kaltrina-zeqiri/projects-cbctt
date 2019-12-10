using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UniversityTimetabling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
		}   

        private void general_info_MouseUp(object sender, MouseButtonEventArgs e)
        {
            GeneralInfo info = new GeneralInfo();
            info.Owner = this;
            info.ShowDialog();
        }

        private void departments_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Departments departments = new Departments();
            departments.Owner = this;
            departments.ShowDialog();
        }

        private void rooms_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Rooms rooms = new Rooms();
            rooms.Owner = this;
            rooms.ShowDialog();
        }

        private void teachers_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Teachers teachers = new Teachers();
            teachers.Owner = this;
            teachers.ShowDialog();
        }

        private void courses_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Courses courses = new Courses();
            courses.Owner = this;
            courses.ShowDialog();
        }

        private void courseTeacherRel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CourseTeacherRelation relationGrid = new CourseTeacherRelation();
            relationGrid.Owner = this;
            relationGrid.ShowDialog();
        }

        private void courseDepartmentRel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CourseDepartmentRel relationGrid = new CourseDepartmentRel();
            relationGrid.Owner = this;
            relationGrid.ShowDialog();
        }

        private void generateTimetabling_MouseUp(object sender, MouseButtonEventArgs e)
        {
            LoadingPanel.LoadingPanel bar = new LoadingPanel.LoadingPanel();
            bar.Owner = this;
			bar.Topmost = true;
            bar.Show();
			validateTimetableOpen(bar);
        }

		private void validateTimetableOpen(LoadingPanel.LoadingPanel bar)
		{
			string Columns = " c.id AS course_id";
			string From = "courses c" +
				" LEFT JOIN course_teacher_rel ctr ON c.id=ctr.course_id ";
			string Conditions = " c.parent_id IS NOT NULL AND ctr.teacher_id IS NULL ";

			DataTable unasignedCoursesT = DBConnection.Select(From, Columns, Conditions);
			if (unasignedCoursesT.Rows.Count != 0)
			{
				MessageBox.Show("Every course should have a teacher assigned! In order to open Timetable window, you have to assign teachers to these courses first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				CourseTeacherRelation relationGrid = new CourseTeacherRelation();
				relationGrid.Owner = this;
				relationGrid.checkBox.IsChecked = true;
				bar.Close();
				relationGrid.ShowDialog();
			} else
			{
				string ColumnsD = " c.id AS course_id";
				string FromD = "courses c" +
					" LEFT JOIN course_department_rel cdr ON c.id=cdr.course_id ";
				string ConditionsD = " c.parent_id IS NOT NULL AND cdr.department_id IS NULL ";

				DataTable unasignedCoursesD = DBConnection.Select(FromD, ColumnsD, ConditionsD);
				if (unasignedCoursesD.Rows.Count != 0)
				{
					MessageBox.Show("Every course should have at least a study program assigned! In order to open Timetable window, you have to assign study programs to these courses first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
					CourseDepartmentRel relationGrid = new CourseDepartmentRel();
					relationGrid.Owner = this;
					relationGrid.checkBox.IsChecked = true;
					bar.Close();
					relationGrid.ShowDialog();
				} else
				{
					TimetablingResultDisplay timetabling = new TimetablingResultDisplay();
					timetabling.Owner = this;
					bar.Close();
					timetabling.ShowDialog();
				}
			}
		}
    }
}
