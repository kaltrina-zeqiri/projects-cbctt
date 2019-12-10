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
using System.Windows.Shapes;

namespace UniversityTimetabling
{

    /// <summary>

    /// Interaction logic for PreferencesWindow.xaml

    /// </summary>

    public partial class PreferencesWindow : Window
    {
        List<int> teacherCourses = new List<int>();
        public PreferencesWindow(string teacherId)
        {
            InitializeComponent();
            if (teacherId != "")
            {
                initInfo(int.Parse(teacherId.ToString()));
                addRowColumnDefinitions();
                initPanels();
                initiateColors(int.Parse(teacherId.ToString()));
            }
            else
            {
                addRowColumnDefinitions();
                initPanels();
            }
        }

        public void initInfo(int tId)
        {
            comboBoxTeacher.SelectedValue = tId;
            DataTable courseData = DBConnection.Select("course_teacher_rel ", " course_id", " teacher_id=" + tId);

            if (courseData != null && courseData.Rows.Count > 0)
            {
                teacherCourses.Clear();
                foreach(DataRow course in courseData.Rows)
                {
                    teacherCourses.Add(int.Parse(course.ItemArray[0].ToString()));
                }
            }
        }

        private List<PreferencesControl> getChilds ()
        {
            var cellInfo = GridMain.Children;
            List<PreferencesControl> childs = new List<PreferencesControl>();

            foreach (PreferencesControl pControl in cellInfo)
            {
                childs.Add(pControl);
            }

            return childs;
        }

        public void initPanels()
        {
            int iRow = -1;

            foreach (RowDefinition row in GridMain.RowDefinitions)
            {
                iRow++;
                int iCol = -1;
                PreferencesControl pPanel;
                foreach (ColumnDefinition col in GridMain.ColumnDefinitions)
                {
                    iCol++;

                    if (iRow == 0)
                    {
                        pPanel = new PreferencesControl(new List<int>(), null, null, int.Parse(comboBoxTeacher.SelectedValue.ToString()));
                        switch (iCol)
                        {
                            case 1:
                                pPanel.Caption = "Monday";
                                break;
                            case 2:
                                pPanel.Caption = "Tuesday";
                                break;
                            case 3:
                                pPanel.Caption = "Wednesday";
                                break;
                            case 4:
                                pPanel.Caption = "Thursday";
                                break;
                            case 5:
                                pPanel.Caption = "Friday";
                                break;
                            case 6:
                                pPanel.Caption = "Saturday";
                                break;
                            case 7:
                                pPanel.Caption = "Sunday";
                                break;
                            default:
                                pPanel.Caption = "";
                                break;
                        }
                    }
                    else
                    {
                        switch (iCol)
                        {
                            case 0:
                                pPanel = new PreferencesControl(new List<int>(), null, null, int.Parse(comboBoxTeacher.SelectedValue.ToString()));
                                pPanel.Caption = "";

                                switch (iRow)
                                {
                                    case 1:
                                        pPanel.Caption = "08:00-09:00";
                                        break;
                                    case 2:
                                        pPanel.Caption = "09:00-10:00";
                                        break;
                                    case 3:
                                        pPanel.Caption = "10:00-11:00";
                                        break;
                                    case 4:
                                        pPanel.Caption = "11:00-12:00";
                                        break;
                                    case 5:
                                        pPanel.Caption = "12:00-13:00";
                                        break;
                                    case 6:
                                        pPanel.Caption = "13:00-14:00";
                                        break;
                                    case 7:
                                        pPanel.Caption = "14:00-15:00";
                                        break;
                                    case 8:
                                        pPanel.Caption = "15:00-16:00";
                                        break;
                                    case 9:
                                        pPanel.Caption = "16:00-17:00";
                                        break;
                                    case 10:
                                        pPanel.Caption = "17:00-18:00";
                                        break;
                                    case 11:
                                        pPanel.Caption = "18:00-19:00";
                                        break;
                                    case 12:
                                        pPanel.Caption = "19:00-20:00";
                                        break;
                                    case 13:
                                        pPanel.Caption = "20:00-21:00";
                                        break;
                                    case 14:
                                        pPanel.Caption = "21:00-22:00";
                                        break;
                                    default:
                                        pPanel.Caption = "";
                                        break;
                                }
                                break;
                            default:
                                pPanel = new PreferencesControl( teacherCourses, iRow.ToString(), iCol.ToString(), int.Parse(comboBoxTeacher.SelectedValue.ToString()));
                                pPanel.Caption = "";
                                break;
                        }
                    }

                    Grid.SetColumn(pPanel, iCol);
                    Grid.SetRow(pPanel, iRow);

                    GridMain.Children.Add(pPanel);
                }
            }
        }

        public void addRowColumnDefinitions()
        {
            Dictionary<string, string> first = new Dictionary<string, string>();

            ArrayList data = DBConnection.Get("general_info", 1);
            first = (Dictionary<string, string>)data[0];

            string nrOfDays, periodsPerDay;
            first.TryGetValue("number_of_days", out nrOfDays);
            first.TryGetValue("periods_per_day", out periodsPerDay);

            for (int i = 0; i < Convert.ToInt32(nrOfDays); i++)
            {
                GridMain.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int j = 0; j < Convert.ToInt32(periodsPerDay); j++)
            {
                GridMain.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void initiateColors(int teacherId)
        {
            foreach (PreferencesControl child in GridMain.Children)
            {
                if (child.Row != "" && child.Column != "")
                {
                    if (CoursePeriodPreferences(child, teacherId))
                    {
                        child.PanelBackground = "#5E716A";
                        child.Clicked = "true";
                    }
                    else
                    {
                        child.PanelBackground = "#EEEEEE";
                        child.Clicked = "false";
                    }
                }
            }
        }

        public bool validatePreference()
        {
            bool allow = true;
            List<PreferencesControl> childs = getChilds().Where(x => x.Caption.Length == 0).ToList();
            if ((childs.Count(x => x.Clicked == "true") + 1) > (childs.Count / 2))
                allow = false;
            return allow;
        }

        private bool CoursePeriodPreferences(PreferencesControl child, int teacherId)
        {
            int cRow = int.Parse(child.Row);
            int cColumn = int.Parse(child.Column);

            if (teacherId > 0)
            {
                DataTable teacherData = DBConnection.Select("teacher_periods_preferences ", " day, period  ", " teacher_id =" + teacherId + " AND day=" + cColumn + " AND period=" + cRow);

                if (teacherData != null && teacherData.Rows.Count > 0)
                    return true;
                else return false;
            }
            return false;          
        }

        private void comboBoxTeacher_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("teachers", comboBoxTeacher, "name,surname");
        }

        private void comboBoxTeacher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxTeacher.SelectedValue != null)
            {
                initInfo(int.Parse(comboBoxTeacher.SelectedValue.ToString()));
                initiateColors(int.Parse(comboBoxTeacher.SelectedValue.ToString()));
            }
            
        }

        private void close_MouseUp(object sender, EventArgs e)
        {
            Close();
        }

        private void close_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Close();
            }
        }
    }
}