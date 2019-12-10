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
    /// Interaction logic for CoursePeriodPreferences.xaml
    /// </summary>
    public partial class CoursePeriodPreferences : Window
    {
        public CoursePeriodPreferences()
        {
            InitializeComponent();
        }

        private void coursePeriodPreferences_Initialized(object sender, EventArgs e)
        {
            string Columns = " cp.id AS id, c.id AS course_id, c.name AS course_name, t.id as teacher_id, concat(t.name, ' ', t.surname)  AS teacher, d.name AS day, p.name AS period";

            string From = "course_periods_preferences cp" +
                " JOIN courses c ON cp.course_id=c.id " +
                " JOIN days d ON cp.day=d.id " +
                " JOIN periods p ON cp.period=p.id " +
                " LEFT JOIN course_teacher_rel ctr ON c.id=ctr.course_id " +
                " LEFT JOIN teachers t ON ctr.teacher_id=t.id ";
            courseperiodGrid.ItemsSource = DBConnection.Select(From, Columns).DefaultView;
        }

        private void addCoursePeriodPreference_MouseUp(object sender, MouseButtonEventArgs e)
        {
           // PreferencesWindow win = new PreferencesWindow();
           // win.Show();
        }

        private void deleteCoursePeriodPreference_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)courseperiodGrid.SelectedItem;
            if (row != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to delete?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    int id = (int)row["ID"];
                    int result = DBConnection.Delete("course_periods_preferences", id);
                    if (result > 0)
                        MessageBox.Show("Successfully deleted Course Period Preferences!", "Information", MessageBoxButton.OK);

                    coursePeriodPreferences_Initialized(sender, e);
                }
            }
            else
                MessageBox.Show("Please select a row to delete!", "Information");
        }

        private void refreshCoursePeriodPreferences_MouseUp(object sender, MouseButtonEventArgs e)
        {
            coursePeriodPreferences_Initialized(sender, e);
        }

        private void comboBoxDays_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("days", comboBoxDays);
        }

        private void filterPeriodPreferences()
        {
            string name = txtFilterByName.Text;
            string teacher = txtFilterByTeacher.Text;
            int day = (comboBoxDays.SelectedIndex >= 0) ? Convert.ToInt32(comboBoxDays.SelectedValue.ToString()) : 0;

            string Columns = " cp.id AS id, c.id AS course_id, c.name AS course_name, t.id as teacher_id, concat(t.name, ' ', t.surname)  AS teacher, d.name AS day, p.name AS period";
            string From = "course_periods_preferences cp" +
                " JOIN courses c ON cp.course_id=c.id " +
                " JOIN days d ON cp.day=d.id " +
                " JOIN periods p ON cp.period=p.id " +
                " LEFT JOIN course_teacher_rel ctr ON c.id=ctr.course_id " +
                " LEFT JOIN teachers t ON ctr.teacher_id=t.id ";   
            string Conditions = null;

            if (name != "")
                Conditions = " c.name LIKE '%" + name + "%' ";
            if (teacher != "")
                Conditions += ((Conditions != null) ? " AND " : "") + " concat(t.name, ' ', t.surname) LIKE '%" + teacher + "%'";
            if (day > 0)
                Conditions += ((Conditions != null) ? " AND " : "") + " cp.day= '" + day + "'";

            courseperiodGrid.ItemsSource = DBConnection.Select(From, Columns, Conditions).DefaultView;
        }

        private void clearFilterPeriodPreferences_MouseUp(object sender, MouseButtonEventArgs e)
        {
            coursePeriodPreferences_Initialized(sender, e);
            comboBoxDays.SelectedIndex = -1;
            txtFilterByName.Text = null;
            txtFilterByTeacher.Text = null;
        }

        private void txtFilterByName_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterPeriodPreferences();
        }

        private void comboBoxDays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterPeriodPreferences();
        }

        private void editCoursePeriodPreference_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)courseperiodGrid.SelectedItem;
            if (row != null)
            {
              //  PreferencesWindow win = new PreferencesWindow(row.Row.ItemArray);
              //  win.Show();
            }
       }

        private void courseperiodGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)courseperiodGrid.SelectedItem;
            if (row != null)
            {
              //  PreferencesWindow win = new PreferencesWindow(row.Row.ItemArray);
              //  win.Show();
            }
        }
    }
}
