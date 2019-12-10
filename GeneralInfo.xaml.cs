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
    /// Interaction logic for GeneralInfo.xaml
    /// </summary>
    public partial class GeneralInfo : Window
    {
		public GeneralInfo()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> first = new Dictionary<string, string>();

            ArrayList data =  DBConnection.Get("general_info", 1);
            first = (Dictionary < string, string> )data[0];

            string nrOfDays, periodsPerDay, minNrOfLectures, maxNrOfLectures, editor;
            first.TryGetValue("number_of_days", out nrOfDays);
            first.TryGetValue("periods_per_day", out periodsPerDay);
            first.TryGetValue("min_daily_lectures", out minNrOfLectures);
            first.TryGetValue("max_daily_lectures", out maxNrOfLectures);
            first.TryGetValue("editor", out editor);

            comboBoxNrDays.SelectedValue = int.Parse(nrOfDays);
            cmbPeriodsperDay.SelectedValue = int.Parse(periodsPerDay);
            cmbMinNrLecturesperDay.SelectedValue = int.Parse(minNrOfLectures);
            cmbMaxNrLecturesperDay.SelectedValue = int.Parse(maxNrOfLectures);
			txtEditor.Text = editor;
        }

        private void saveGeneralInfo_MouseUp(object sender, EventArgs e)
        {
            int id = 1;
            if(comboBoxNrDays.SelectedIndex < 0) MessageBox.Show("Number of working days is required!", "Information", MessageBoxButton.OK, MessageBoxImage.Warning );
            if(cmbPeriodsperDay.SelectedIndex < 0) MessageBox.Show("Number of periods/hours per day is required!", "Information", MessageBoxButton.OK, MessageBoxImage.Warning );
            if(cmbMinNrLecturesperDay.SelectedIndex < 0) MessageBox.Show("Number of minimum periods/hours per day is required!", "Information", MessageBoxButton.OK, MessageBoxImage.Warning );
            if(cmbMaxNrLecturesperDay.SelectedIndex < 0) MessageBox.Show("Number of maximum periods/hours per day is required!", "Information", MessageBoxButton.OK, MessageBoxImage.Warning );

            string values = " number_of_days='" + comboBoxNrDays.SelectedValue +
                               "', periods_per_day='" + cmbPeriodsperDay.SelectedValue +
                               "', min_daily_lectures='" + cmbMinNrLecturesperDay.SelectedValue +
                               "', max_daily_lectures='" + cmbMaxNrLecturesperDay.SelectedValue +
                               "', editor='" + txtEditor.Text + "'";

            int result = DBConnection.Update("general_info", values, id);
            if (result > 0)
                MessageBox.Show("Successfully updated Info!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void comboBoxNrDays_Initialized(object sender, EventArgs e)
        {
            comboBoxNrDays.SelectedValuePath = "id";
            comboBoxNrDays.DisplayMemberPath = "name";
            comboBoxNrDays.Items.Add(new { id = 3, name = 3 });
            comboBoxNrDays.Items.Add(new { id = 4, name = 4 });
            comboBoxNrDays.Items.Add(new { id = 5, name = 5 });
            comboBoxNrDays.Items.Add(new { id = 6, name = 6 });
        }

        private void cmbPeriodsperDay_Initialized(object sender, EventArgs e)
        {
            cmbPeriodsperDay.SelectedValuePath = "id";
            cmbPeriodsperDay.DisplayMemberPath = "name";
            cmbPeriodsperDay.Items.Add(new { id = 8, name = 8 });
            cmbPeriodsperDay.Items.Add(new { id = 9, name = 9 });
            cmbPeriodsperDay.Items.Add(new { id = 10, name = 10 });
            cmbPeriodsperDay.Items.Add(new { id = 11, name = 11 });
            cmbPeriodsperDay.Items.Add(new { id = 12, name = 12 });
            cmbPeriodsperDay.Items.Add(new { id = 13, name = 13 });
        }

        private void cmbMinNrLecturesperDay_Initialized(object sender, EventArgs e)
        {
            cmbMinNrLecturesperDay.SelectedValuePath = "id";
            cmbMinNrLecturesperDay.DisplayMemberPath = "name";
            cmbMinNrLecturesperDay.Items.Add(new { id = 1, name = 1 });
            cmbMinNrLecturesperDay.Items.Add(new { id = 2, name = 2 });
            cmbMinNrLecturesperDay.Items.Add(new { id = 3, name = 3 });
            cmbMinNrLecturesperDay.Items.Add(new { id = 4, name = 4 });
            cmbMinNrLecturesperDay.Items.Add(new { id = 5, name = 5 });
            cmbMinNrLecturesperDay.Items.Add(new { id = 6, name = 6 });
            cmbMinNrLecturesperDay.Items.Add(new { id = 7, name = 7 });
            cmbMinNrLecturesperDay.Items.Add(new { id = 8, name = 8 });
            cmbMinNrLecturesperDay.Items.Add(new { id = 9, name = 9 });
            cmbMinNrLecturesperDay.Items.Add(new { id = 10, name = 10 });
            cmbMinNrLecturesperDay.Items.Add(new { id = 11, name = 11 });
            cmbMinNrLecturesperDay.Items.Add(new { id = 12, name = 12 });
            cmbMinNrLecturesperDay.Items.Add(new { id = 13, name = 13 });
        }

        private void cmbMaxNrLecturesperDay_Initialized(object sender, EventArgs e)
        {
            cmbMaxNrLecturesperDay.SelectedValuePath = "id";
            cmbMaxNrLecturesperDay.DisplayMemberPath = "name";
            cmbMaxNrLecturesperDay.Items.Add(new { id = 1, name = 1 });
            cmbMaxNrLecturesperDay.Items.Add(new { id = 2, name = 2 });
            cmbMaxNrLecturesperDay.Items.Add(new { id = 3, name = 3 });
            cmbMaxNrLecturesperDay.Items.Add(new { id = 4, name = 4 });
            cmbMaxNrLecturesperDay.Items.Add(new { id = 5, name = 5 });
            cmbMaxNrLecturesperDay.Items.Add(new { id = 6, name = 6 });
            cmbMaxNrLecturesperDay.Items.Add(new { id = 7, name = 7 });
            cmbMaxNrLecturesperDay.Items.Add(new { id = 8, name = 8 });
            cmbMaxNrLecturesperDay.Items.Add(new { id = 9, name = 9 });
            cmbMaxNrLecturesperDay.Items.Add(new { id = 10, name = 10 });
            cmbMaxNrLecturesperDay.Items.Add(new { id = 11, name = 11 });
            cmbMaxNrLecturesperDay.Items.Add(new { id = 12, name = 12 });
            cmbMaxNrLecturesperDay.Items.Add(new { id = 13, name = 13 });
        }

        private void closeGeneralInfo_MouseUp(object sender, EventArgs e)
        {
            Close();
        }

        private void saveGeneralInfo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                saveGeneralInfo_MouseUp(sender, e);
                Keyboard.ClearFocus();
            }
        }

        private void closeGeneralInfo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                closeGeneralInfo_MouseUp(sender, e);
                Keyboard.ClearFocus();
            }
        }
    }
}
