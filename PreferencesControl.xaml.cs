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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UniversityTimetabling
{
    /// <summary>

    /// Interaction logic for PreferencesControl.xaml

    /// </summary>

    public partial class PreferencesControl : UserControl
    {
        public List<int> CourseIds;
        public int TeacherId;

        public string Caption
        {
            get { return lblCaption.Text; }
            set { lblCaption.Text = value; }
        }

        public string Row
        {
            get { return lblRow.Text; }
            set { lblRow.Text = value; }
        }

        public string Column
        {
            get { return lblColumn.Text; }
            set { lblColumn.Text = value; }
        }

        public string Clicked
        {
            get { return lblClicked.Text; }
            set { lblClicked.Text = value; }
        }

        public string PanelBackground
        {
            get { return pnlBackground.BorderBrush.ToString(); }
            set { pnlBackground.Background = (new BrushConverter()).ConvertFromString(value) as Brush; }
        }

        public PreferencesControl(List<int> course_ids, string row = null, string column = null, int teacher_id = 0)
        {            
            InitializeComponent();
            CourseIds = course_ids;
            lblRow.Text = row;
            lblColumn.Text = column;
            lblClicked.Text = "false";
            TeacherId = teacher_id;
       }
        private void gridPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            pnlBackground.BorderThickness = new Thickness(1);
        }

        private void gridPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            pnlBackground.BorderThickness = new Thickness(0);
        }

        private void pnlBackground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var day = int.Parse(lblColumn.Text);
                var period = int.Parse(lblRow.Text);

                if (lblClicked.Text == "false")
                {
                    if((Parent.GetType() == typeof(Grid)) && ((Parent as Grid).Parent.GetType() == typeof(Grid)) && (((Parent as Grid).Parent as Grid).Parent.GetType() == typeof(PreferencesWindow))) {
                        if (!(((Parent as Grid).Parent as Grid).Parent as PreferencesWindow).validatePreference())
                        {
                            MessageBox.Show("Teacher should be available at least half of the week! In order to add other constraints, please remove some existing constraints!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                    
                    lblClicked.Text = "true";
                    PanelBackground = "#5E716A";

                    if(TeacherId > 0)
                    {
						DataTable preferences = DBConnection.Select("teacher_periods_preferences", "day, period", "teacher_id=" + TeacherId + " AND day=" + day + " AND period=" + period);
						if (preferences.Rows.Count == 0)
						{
							string values = "'" + TeacherId + "', '" + day + "', '" + period + "'";
							int result = DBConnection.Create("teacher_periods_preferences", "teacher_id, day, period", values);
						}
                    }

					if (CourseIds.Count > 0)
					{
						foreach (int cId in CourseIds)
						{
							DataView data = DBConnection.Select("course_periods_preferences", "'*'", " course_id = " + cId + " AND day=" + day + " AND period=" + period).DefaultView;
							if (data.Count == 0)
							{
								string values = "'" + cId + "', '" + day + "', '" + period + "'";
								int result = DBConnection.Create("course_periods_preferences", "course_id, day, period", values);
							}
						}
					}
                }
                else
                {
                    lblClicked.Text = "false";
                    PanelBackground = "#EEEEEE";

                    if (TeacherId > 0)
                    {
                         int result = DBConnection.Delete("teacher_periods_preferences", 0, " teacher_id=" + TeacherId + " AND day=" + day + " AND period=" + period);
                    }

					if (CourseIds.Count > 0)
					{
						foreach (int cId in CourseIds)
						{
							int result = DBConnection.Delete("course_periods_preferences", 0, " course_id=" + cId + " AND day=" + day + " AND period=" + period);
						}
					}
                }
            }           
        }
    }
}