using System;
using System.Collections;
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

namespace UniversityTimetabling
{
    /// <summary>
    /// Interaction logic for CoursePeriodPreferenceAddEdit.xaml
    /// </summary>
    public partial class CoursePeriodPreferenceAddEdit : Window
    {
        private ArrayList data = DBConnection.Get("general_info", 1);
       
        public CoursePeriodPreferenceAddEdit()
        {
            InitializeComponent();
        }

        private void comboBoxCourse_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("courses", comboBoxCourse, "name", "parent_id IS NOT NULL");
        }

        private void comboBoxDay_Initialized(object sender, EventArgs e)
        {         
            var first = (Dictionary<string, string>)data[0];
            string nrOfDays;
            first.TryGetValue("number_of_days", out nrOfDays);
            DBConnection.FillCombo("days", comboBoxDay, "name", "id <=" + nrOfDays);
        }

        private void comboBoxPeriod_Initialized(object sender, EventArgs e)
        {
            var first = (Dictionary<string, string>)data[0];
            string periodsPerDay;
            first.TryGetValue("periods_per_day", out periodsPerDay);
           
            DBConnection.FillCombo("periods", comboBoxPeriod, "name", "id <=" + periodsPerDay);
        }

        private void saveCoursePeriodPreferences_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var course_id = comboBoxCourse.SelectedValue;
            var day = comboBoxDay.SelectedValue;
            var period = comboBoxPeriod.SelectedValue;

            string values = "'" + course_id + "', '" + day + "', '" + period + "'";

            int result = DBConnection.Create("course_periods_preferences", "course_id, day, period", values);
            if (result > 0)
                MessageBox.Show("Successfully created new Course Period Preferences!", "Information", MessageBoxButton.OK);

            Close();
        }

        private void close_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
