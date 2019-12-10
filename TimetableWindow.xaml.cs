using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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

    /// Interaction logic for TimetableWindow.xaml

    /// </summary>

    public partial class TimetableWindow : Window
    {
        private string red = "#FF0000";
        private string green = "#008000";
        private string yellow = "#FFFF00";
        private string orange = "#FFA500";
        private string gray = "#A9A9A9";
        int RoomPenalty = 0;
        List<UControl> childs;

        public TimetableWindow(object[] row, Thread splashWindowThread)
        {
			try
			{
				InitializeComponent();

				initInfo(row);
				fillRooms();
				addRowColumnDefinitions();
				initPanels();
				childs = getChilds();
				initiateColors();
				if( splashWindowThread.IsAlive) splashWindowThread.Abort();
			} catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
        }

        public void initInfo(object[] row)
        {
            txtClassId.Text = row[0].ToString();
            txtClass.Text = row[1].ToString();
            txtTeacherId.Text = row[3].ToString();
            txtTeacher.Text = row[4].ToString();
            txbDepartments.Text = row[8].ToString();
			if(row[5].ToString() != "-" && row[6].ToString() != "-" && row[7].ToString() != "-")
			{
				txbInfo.Text = row[5].ToString().Replace(" ", "") + " " + row[7].ToString() + ", " + row[6].ToString();
			}
			else
			{
				txbInfo.Visibility = Visibility.Hidden;
				labelInfo.Visibility = Visibility.Hidden;
			}

            Dictionary<string, string> first = new Dictionary<string, string>();

            ArrayList data = DBConnection.Get("courses", int.Parse(row[0].ToString()));
            first = (Dictionary<string, string>)data[0];

            string nrOfStudents, nrOfLectures, detail, code, departmentIds = "", departments = "", semester, lectureGroup = "", numericGroup = "", laboratoryGroup = "";
            first.TryGetValue("number_of_students", out nrOfStudents);
            first.TryGetValue("number_of_lectures", out nrOfLectures);
            first.TryGetValue("detail", out detail);
            first.TryGetValue("code", out code);
            first.TryGetValue("semester_id", out semester);
            first.TryGetValue("lecture_group", out lectureGroup);
            first.TryGetValue("numeric_group", out numericGroup);
            first.TryGetValue("laboratory_group", out laboratoryGroup);

            txtCode.Text = code;
            txtSemester.Text = semester;
            txtLectureGroup.Text = lectureGroup;
            txtNumericGroup.Text = numericGroup;
            txtLaboratoryGroup.Text = laboratoryGroup;
            txtNumberofStudents.Text = nrOfStudents;
			txtDetail.Text = detail;
            var std = nrOfLectures.Split('+');
            txtNumberofLectures.Text = detail == "lecture" ? std[0].ToString() : (detail == "numeric" ? std[1].ToString() : std[2].ToString());

            string Columns = " DISTINCT(d.id) AS departmentId, d.name AS departmentName";
            string From = " departments d" +
               " JOIN course_department_rel cd ON d.id=cd.department_id ";
            string Conditions = " cd.course_id =" + Int32.Parse(row[0].ToString());

            DataTable res = DBConnection.Select(From, Columns, Conditions);


            ArrayList result = new ArrayList();
            foreach (DataRow r in res.Rows)
            {
                departments += r.ItemArray[1].ToString() + "\n";
                departmentIds += departmentIds != "" ? (", " + r.ItemArray[0].ToString()) : r.ItemArray[0].ToString();
            }

            txbDepartments.Text = departments;
            txtDepartmentIds.Text = departmentIds;
        }

        public List<UControl> getChilds ()
        {
            var cellInfo = GridMain.Children;
            List<UControl> childs = new List<UControl>();

            foreach (UControl uControl in cellInfo)
            {
                Panel panelContent = (Panel)uControl.Content;

                var contentChilds = panelContent.Children;

                foreach (var finalChilds in contentChilds)
                {
                    if (finalChilds.GetType().Name == "UControl")
                        childs.Add((UControl)finalChilds);
                }
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
                UControl ucPanel;
                foreach (ColumnDefinition col in GridMain.ColumnDefinitions)
                {
                    iCol++;

                    if (iRow == 0)
                    {
                        ucPanel = new UControl(false);
                        switch (iCol)
                        {
                            case 1:
                                ucPanel.Caption = "Monday";
                                break;
                            case 2:
                                ucPanel.Caption = "Tuesday";
                                break;
                            case 3:
                                ucPanel.Caption = "Wednesday";
                                break;
                            case 4:
                                ucPanel.Caption = "Thursday";
                                break;
                            case 5:
                                ucPanel.Caption = "Friday";
                                break;
                            case 6:
                                ucPanel.Caption = "Saturday";
                                break;
                            case 7:
                                ucPanel.Caption = "Sunday";
                                break;
                            default:
                                ucPanel.Caption = "";
                                break;
                        }
                    }
                    else
                    {
                        switch (iCol)
                        {
                            case 0:
                                ucPanel = new UControl(false);
                                ucPanel.Caption = "";

                                switch (iRow)
                                {
                                    case 1:
                                        ucPanel.Caption = "08:00-09:00";
                                        break;
                                    case 2:
                                        ucPanel.Caption = "09:00-10:00";
                                        break;
                                    case 3:
                                        ucPanel.Caption = "10:00-11:00";
                                        break;
                                    case 4:
                                        ucPanel.Caption = "11:00-12:00";
                                        break;
                                    case 5:
                                        ucPanel.Caption = "12:00-13:00";
                                        break;
                                    case 6:
                                        ucPanel.Caption = "13:00-14:00";
                                        break;
                                    case 7:
                                        ucPanel.Caption = "14:00-15:00";
                                        break;
                                    case 8:
                                        ucPanel.Caption = "15:00-16:00";
                                        break;
                                    case 9:
                                        ucPanel.Caption = "16:00-17:00";
                                        break;
                                    case 10:
                                        ucPanel.Caption = "17:00-18:00";
                                        break;
                                    case 11:
                                        ucPanel.Caption = "18:00-19:00";
                                        break;
                                    case 12:
                                        ucPanel.Caption = "19:00-20:00";
                                        break;
                                    case 13:
                                        ucPanel.Caption = "20:00-21:00";
                                        break;
                                    case 14:
                                        ucPanel.Caption = "21:00-22:00";
                                        break;
                                    default:
                                        ucPanel.Caption = "";
                                        break;
                                }
                                break;
                            default:
                                ucPanel = new UControl(true, iRow.ToString(), iCol.ToString());
                                ucPanel.Caption = "";
                                break;
                        }
                    }

                    Grid.SetColumn(ucPanel, iCol);
                    Grid.SetRow(ucPanel, iRow);

                    GridMain.Children.Add(ucPanel);
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


        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                foreach (UControl child in childs)
                {
                    if (child.Clicked == "true")
                    {
                        if (comboBoxRooms.SelectedIndex != -1)
                        {
                            if (child.ControlAssigned == "false" && child.BlockedAssigned != "true")
                            {
                                //check if clicked period is a red position
                                string cCode = txtCode.Text;
                                int day = int.Parse(child.Column) - 1;
                                int startPeriod = int.Parse(child.Row) - 1;
                                string rCode = "";

                                string query = " SELECT code FROM rooms WHERE id=" + comboBoxRooms.SelectedValue;
                                DataTable res = DBConnection.RAW_Select(query);

                                ArrayList result = new ArrayList();
                                foreach (DataRow r in res.Rows)
                                {
                                    rCode = r.ItemArray[0].ToString();
                                }

                                string values = "'" + cCode + "', '" + rCode + "', '" + day + "', '" + startPeriod + "'";

                                string table = "timetable_" + getCurrentSemester();

                                DataView data = DBConnection.Select(table, "'*'", " course_code = '" + cCode + "'").DefaultView;
                                if (data.Count == 0)
                                {
                                    int createdId = DBConnection.Create(table, "course_code, room_code, day, start_period", values);
                                }
                                else
                                {
                                    string vals = "room_code='" + rCode + "', day='" + day + "', start_period='" + startPeriod + "'";
                                    int createdId = DBConnection.Update(table, vals, 0, " course_code='" + cCode + "'");
                                }
                                MessageBox.Show("Class was placed successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                Close();
                                break;
                            }
                            else {
                                MessageBox.Show("Class can not be placed at this position!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                child.Clicked = "false";
                                break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please select a room first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            child.Clicked = "false";
                            break;
                        }
                    }
                }
            }
        }

        private void fillRooms()
        {
            comboBoxRooms.SelectedValuePath = "id";
            comboBoxRooms.DisplayMemberPath = "name";

            string query = " SELECT id, name, code FROM rooms WHERE id NOT IN ( SELECT room_id FROM courses_rooms_rel WHERE course_id = " + Int32.Parse(txtClassId.Text) + " )";

            DataTable res = DBConnection.RAW_Select(query);

            ArrayList result = new ArrayList();
            foreach (DataRow r in res.Rows)
            {
                comboBoxRooms.Items.Add(new { id = r.ItemArray[0], name = r.ItemArray[1]});
               
            }
        }

        private void comboboxRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			comboBoxRooms.IsDropDownOpen = false;
			Thread newThread = new Thread(new ThreadStart(ThreadStartingPoint));
			newThread.SetApartmentState(ApartmentState.STA);
			newThread.IsBackground = true;
			newThread.Start();
			RoomPenalty = checkRoomCapacity();
			initiateColors();
			newThread.Abort();
		}

		private void ThreadStartingPoint()
		{
			LoadingPanel.LoadingPanel bar = new LoadingPanel.LoadingPanel();
			bar.Show();
			bar.Topmost = true;
			System.Windows.Threading.Dispatcher.Run();
		}

		private string getCurrentSemester()
        {
            Dictionary<string, string> first = new Dictionary<string, string>();
            ArrayList data = DBConnection.Get("currentSemester", 1);
            first = (Dictionary<string, string>)data[0];

            string name;
            first.TryGetValue("name", out name);

            return name.ToLower();
        }

        private void initiateColors()
        {
            List<Assignment> assignments = getAssignments();
            foreach (UControl child in childs)
            {
                child.BlockedAssigned = "false";
                if (SlotHardConstraints(child, assignments))
                {
                    child.PanelBackground = red;
                    child.BackgroundOpacity = "0.9";
                    child.ControlAssigned = "true";
                }
                else
                {
                    child.ControlAssigned = "false";
                }
            }

            List<UControl> leftChilds = childs.FindAll(x => x.ControlAssigned == "false");
            foreach (UControl ch in leftChilds)
            {
                string message = "Class will be assigned from " + getCurrentPeriod(ch, int.Parse(txtClassId.Text));
                int penalty = RoomPenalty + GetWindowViolation(ch, assignments, ref message);
                if (RoomPenalty > 0) message += ",\nthe room does not have enough space for this class.";
                if (penalty == 0)
                {
                    ch.PanelBackground = green;
                    ch.BackgroundOpacity = "0.7";
                    ch.gridPanel.ToolTip = message;
                }
                else if(penalty > 0 && penalty < 4)
                {
                    ch.PanelBackground = yellow;
                    ch.BackgroundOpacity = "0.7";
                    ch.gridPanel.ToolTip = message;
                }
                else if(penalty >= 4 && penalty < 100)
                {
                    ch.PanelBackground = orange;
                    ch.BackgroundOpacity = "0.7";
                    ch.gridPanel.ToolTip = message;
                }
                else
                {
                    ch.PanelBackground = gray;
                    ch.BackgroundOpacity = "0.8";
                    ch.BlockedAssigned = "true";
                    ch.gridPanel.ToolTip = "Not enough space to assign this course!\n" + message;
                }
            }
        }

        private string getCurrentPeriod(UControl position, int courseId)
        {
            string returnText = "";

            Dictionary<string, string> first = new Dictionary<string, string>();
            string name;
            ArrayList rec = DBConnection.Get("subPeriods", int.Parse(position.Row));
            first = (Dictionary<string, string>)rec[0];
            first.TryGetValue("name", out name);

            returnText = name + " to " + TimetablingResultDisplay.getEndingPeriod(name, courseId);

            return returnText;
        }

        private int checkRoomCapacity()
        {
            int penalty = 0;
            int roomId = int.Parse(comboBoxRooms.SelectedValue.ToString());
            Dictionary<string, string> first = new Dictionary<string, string>();

            ArrayList data = DBConnection.Get("rooms", roomId);
            first = (Dictionary<string, string>)data[0];

            string size;
            first.TryGetValue("size", out size);

            int difference = int.Parse(txtNumberofStudents.Text) - int.Parse(size);
            if (difference > 0)
            {
                penalty = difference;
            }
            return penalty;
        }

        private int GetWindowViolation(UControl child, List<Assignment> assignments, ref string message)
        {
            //Each time window in a curriculum counts as many violation as its length (in periods).
            int window = 0;

            int row = int.Parse(child.Row) -1;
            int column = int.Parse(child.Column) -1;

            int breaks = 0;
            switch (int.Parse(txtNumberofLectures.Text) % 2)
            {
                case 0:
                    breaks = (int.Parse(txtNumberofLectures.Text) - 2) / 2;
                    break;
                case 1:
                    breaks = (int.Parse(txtNumberofLectures.Text) - 1) / 2;
                    break;
            }

            int lecturePeriods = 3 * int.Parse(txtNumberofLectures.Text);
            int endPeriod = int.Parse(child.Row) + (3 * int.Parse(txtNumberofLectures.Text)) + breaks - 1;

            string columns = " c.id, c.code ";
            string from = "courses c" +
                                   " LEFT JOIN course_department_rel cdr ON c.id = cdr.course_id ";
			string conditions = "";
			switch (txtDetail.Text)
			{
				case "lecture":
					conditions = "c.parent_id IS NOT null AND c.semester_id=" + int.Parse(txtSemester.Text) + " AND cdr.department_id IN (" + txtDepartmentIds.Text + ") AND c.lecture_group='" + txtLectureGroup.Text +"' ";
					break;
				case "numeric":
					conditions = "c.parent_id IS NOT null AND c.semester_id=" + int.Parse(txtSemester.Text) + " AND cdr.department_id IN (" + txtDepartmentIds.Text + ") AND c.lecture_group='" + txtLectureGroup.Text +
										"' AND c.id IN (SELECT c1.id FROM courses c1 WHERE (c1.numeric_group='" + txtNumericGroup.Text +
										"' OR c1.numeric_group IS NULL) AND c1.lecture_group = c.lecture_group) ";
					break;
				case "laboratory":
					conditions = "c.parent_id IS NOT null AND c.semester_id=" + int.Parse(txtSemester.Text) + " AND cdr.department_id IN (" + txtDepartmentIds.Text + ") AND c.lecture_group='" + txtLectureGroup.Text +
										"' AND c.id IN (SELECT c1.id FROM courses c1 WHERE (c1.laboratory_group='" + txtLaboratoryGroup.Text + "' OR c1.laboratory_group IS NULL) AND(c1.numeric_group='" + txtNumericGroup.Text +
										"' OR c1.numeric_group IS NULL) AND c1.lecture_group = c.lecture_group) ";
					break;
			}

            DataTable curricula = DBConnection.Select(from, columns, conditions);
            List<Assignment> beforeAssign = new List<Assignment>();
            List<Assignment> afterAssign = new List<Assignment>();
            if (curricula != null && curricula.Rows.Count > 0)
            {
                foreach (DataRow c in curricula.Rows)
                {
                    //check if students are busy on that slot
                    var before = assignments.FindAll(x => x.Course_code == c.ItemArray[1].ToString() && x.Day == column && x.Ending_period < int.Parse(child.Row)).FirstOrDefault();
                    if(before != null)
                    {
                        beforeAssign.Add(before);
                    }
                  
                    var after = assignments.FindAll(x => x.Course_code == c.ItemArray[1].ToString() && x.Day == column && x.Starting_period > row).FirstOrDefault();
                    if(after != null)
                    {
                        afterAssign.Add(after);
                    }
               
                }
                Assignment beforeAssign1;
                if (beforeAssign.Count > 0)
                {
                    beforeAssign1 = beforeAssign.OrderByDescending(x => x.Ending_period).FirstOrDefault();
                    window += (row - beforeAssign1.Ending_period);
                }

                Assignment afterAssign1;
                if (afterAssign.Count > 0)
                {
                    afterAssign1 = afterAssign.OrderBy(x => x.Starting_period).FirstOrDefault();

                    int difference = (afterAssign1.Starting_period - endPeriod);
                    if (difference < 0) //for same period result = 0
                        window += 100;
                    else
                        window += difference;

                }
                else
                { 
                    UControl otherAfterItem = childs.FindAll(x => x.Column == child.Column && int.Parse(x.Row) > row && x.ControlAssigned == "true").OrderBy(x => int.Parse(x.Row)).FirstOrDefault();
                    List<UControl> dayItems = childs.FindAll(x => x.Column == child.Column);
                    int assignedDiff = 0;
                    if(otherAfterItem != null)
                    {
                        assignedDiff = (int.Parse(otherAfterItem.Row) - (endPeriod + 1));
                    }

                    int difference = (dayItems.Count - endPeriod);  
                    if ((difference < 0) || assignedDiff < 0)
                        window += 100;
                }

				// Calculate room busy periods
				if (comboBoxRooms.SelectedIndex >= 0)
				{
					Dictionary<string, string> first = new Dictionary<string, string>();

					ArrayList data = DBConnection.Get("rooms", int.Parse(comboBoxRooms.SelectedValue.ToString()));
					first = (Dictionary<string, string>)data[0];

					string roomCode;
					first.TryGetValue("code", out roomCode);
					List<Assignment> roomAssign = assignments.FindAll(x => x.Room_code == roomCode && x.Day == column && (x.Starting_period < endPeriod && x.Starting_period > row));
					if (roomAssign.Count > 0)
					{
						window += 100;
					}
				}

				// Calculate teacher busy periods
				if (txtTeacherId.Text != "" && txtTeacherId.Text != null)
				{
					List<Assignment> teacherAssign = assignments.FindAll(x => x.Teacher_id == int.Parse(txtTeacherId.Text) && x.Day == column && (x.Starting_period < endPeriod && x.Starting_period > row));
					if (teacherAssign.Count > 0)
					{
						window += 100;
					}
				}
			}
            if (window > 0 && window < 100)
            {
                int minutes = window * 15;
                TimeSpan ts = TimeSpan.FromMinutes(minutes);
                var res = (((int)ts.TotalHours > 0) ? ((int)ts.TotalHours + "h ") : "") + ((ts.Minutes > 0) ? (ts.Minutes + "mins") : "");
                message += ", \nbut students will wait for " + res;
            }
            else if (window >= 100) message = " \n(Class " + txtClass.Text.Replace("_", ", ") + " \nfrom " + getCurrentPeriod(child, int.Parse(txtClassId.Text)) + ")";
            return window;
        }

        private bool SlotHardConstraints(UControl child, List<Assignment> assignments)
        {
            bool ContraintViolated = true;
            string message = "";
            if (comboBoxRooms.SelectedIndex >= 0)
            {
                ContraintViolated = (SlotTeacherConstraint(child, assignments, ref message) || SlotCurriculaConstraint(child, assignments, ref message) || SlotRoomConstraint(child, assignments, ref message) || CoursePeriodPreferences(child, ref message));
                if (message != "")
                    child.gridPanel.ToolTip = message;
            }
            else
            {
                ContraintViolated = (SlotTeacherConstraint(child, assignments, ref message) || SlotCurriculaConstraint(child, assignments, ref message) || CoursePeriodPreferences(child, ref message));
                if (message != "")
                    child.gridPanel.ToolTip = message;
            }
            return ContraintViolated;
        }

        private bool SlotTeacherConstraint(UControl child, List<Assignment> assignments, ref string msg)
        {
            bool TeacherContraintViolated = true;

			if (txtTeacherId.Text == "" || txtTeacherId.Text == null)
				return false;
            int row = int.Parse(child.Row) - 1;
            int column = int.Parse(child.Column) - 1;

            //check if teacher is busy on that slot
            List<Assignment> allBusyPeriods = assignments.FindAll(x => x.Teacher_id == int.Parse(txtTeacherId.Text)).ToList();
            if (allBusyPeriods == null)
            {
                TeacherContraintViolated = false;

            }
            else
            {
                var checkGivenPeriod = allBusyPeriods.Find(x => x.Day == column && x.Starting_period <= row && x.Ending_period > row);
                if (checkGivenPeriod == null)
                {
                    TeacherContraintViolated = false;

                }
                else
                {
                    msg += ((msg == "") ? "" : ", ") + " Teacher is busy \n(Class " + checkGivenPeriod.Course_name.Replace("_", ", ")
                        + " \nfrom " +  checkGivenPeriod.getCourseTextPeriod() + ")";
                    TeacherContraintViolated = true;
                }
            }
               
            return TeacherContraintViolated;
        }

        private bool SlotRoomConstraint(UControl child, List<Assignment> assignments, ref string msg)
        {
            bool RoomContraintViolated = true;

            int roomId = int.Parse(comboBoxRooms.SelectedValue.ToString());
            Dictionary<string, string> first = new Dictionary<string, string>();

            ArrayList data = DBConnection.Get("rooms", roomId);
            first = (Dictionary<string, string>)data[0];

            string code;
            first.TryGetValue("code", out code);

            int row = int.Parse(child.Row) - 1;
            int column = int.Parse(child.Column) - 1;

            //check if room is busy on that slot
            List<Assignment> allBusyPeriods = assignments.FindAll(x => x.Room_code == code).ToList();
            if (allBusyPeriods == null)
            {
                RoomContraintViolated = false;
            }
            else
            {
                var checkGivenPeriod = allBusyPeriods.Find(x => x.Day == column && x.Starting_period <= row && x.Ending_period > row);
                if (checkGivenPeriod == null)
                {
                    RoomContraintViolated = false;

                }
                else
                {
                    msg += ((msg == "") ? "" : ", ") + " Room is busy\n (Class " + checkGivenPeriod.Course_name.Replace("_", ", ") +
                                " \nfrom " + checkGivenPeriod.getCourseTextPeriod() + ")";
                    RoomContraintViolated = true;
                }
            }

            return RoomContraintViolated;
        }

        private bool SlotCurriculaConstraint(UControl child, List<Assignment> assignments, ref string msg)
        {
            bool CurriculaContraintViolated = true;

            string columns = " c.id, c.code ";
            string from = "courses c" +
                                   " LEFT JOIN course_department_rel cdr ON c.id = cdr.course_id ";
			string conditions = "";
			switch (txtDetail.Text)
			{
				case "lecture":
					conditions = "c.parent_id IS NOT null AND c.semester_id=" + int.Parse(txtSemester.Text) + " AND cdr.department_id IN (" + txtDepartmentIds.Text + ") AND c.lecture_group='" + txtLectureGroup.Text + "' ";
					break;
				case "numeric":
					conditions = "c.parent_id IS NOT null AND c.semester_id=" + int.Parse(txtSemester.Text) + " AND cdr.department_id IN (" + txtDepartmentIds.Text + ") AND c.lecture_group='" + txtLectureGroup.Text +
										"' AND c.id IN (SELECT c1.id FROM courses c1 WHERE (c1.numeric_group='" + txtNumericGroup.Text +
										"' OR c1.numeric_group IS NULL) AND c1.lecture_group = c.lecture_group) ";
					break;
				case "laboratory":
					conditions = "c.parent_id IS NOT null AND c.semester_id=" + int.Parse(txtSemester.Text) + " AND cdr.department_id IN (" + txtDepartmentIds.Text + ") AND c.lecture_group='" + txtLectureGroup.Text +
										"' AND c.id IN (SELECT c1.id FROM courses c1 WHERE (c1.laboratory_group='" + txtLaboratoryGroup.Text + "' OR c1.laboratory_group IS NULL) AND(c1.numeric_group='" + txtNumericGroup.Text +
										"' OR c1.numeric_group IS NULL) AND c1.lecture_group = c.lecture_group) ";
					break;
			}

			DataTable curricula = DBConnection.Select(from, columns, conditions);
            if (curricula != null && curricula.Rows.Count > 0)
            {
                int row = int.Parse(child.Row) - 1;
                int column = int.Parse(child.Column) - 1;

                foreach (DataRow c in curricula.Rows)
                {
                    //check if students are busy on that slot
                    List<Assignment> allBusyPeriods = assignments.FindAll(x => x.Course_code == c.ItemArray[1].ToString()).ToList();
                    if (allBusyPeriods == null)
                    {
                        CurriculaContraintViolated = false;

                    }
                    else
                    {
                        var checkGivenPeriod = allBusyPeriods.Find(x => x.Day == column && x.Starting_period <= row && x.Ending_period > row);
                        if (checkGivenPeriod == null)
                        {
                            CurriculaContraintViolated = false;

                        }
                        else
                        {
                            msg += ((msg == "") ? "" : ", ") + " Students are busy\n (Class " + checkGivenPeriod.Course_name.Replace("_", ", ") +
                                " \nfrom " + checkGivenPeriod.getCourseTextPeriod() + ")";
                            CurriculaContraintViolated = true;
                            break;
                        }
                    }
                }
            }
            else CurriculaContraintViolated = false;
                
            return CurriculaContraintViolated;
        }

        private bool CoursePeriodPreferences(UControl child, ref string msg)
        {
            int cRow = int.Parse(child.Row);
            int cColumn = int.Parse(child.Column);

            int parentPeriod = (( cRow - 1 ) / 4) + 1;

            bool PeriodContraintViolated = true;

            DataTable courseData = DBConnection.Select("course_periods_preferences ", " day, period  ", " course_id=" + int.Parse(txtClassId.Text) + " AND day=" + cColumn + " AND period=" + parentPeriod);

            if (courseData != null && courseData.Rows.Count > 0)
                PeriodContraintViolated = true;
            else PeriodContraintViolated = false;

            if (PeriodContraintViolated)
                msg += ((msg == "") ? "" : ", ") + " Teacher doesn't teach at this time";
            return PeriodContraintViolated;
        }

        private List<Assignment> getAssignments()
        {
            List<Assignment> assignments = new List<Assignment>();
          
            string columns = " course_code, room_code, day, start_period";
            string from = " timetable_" + getCurrentSemester();

            DataTable allAssinedCourses = DBConnection.Select(from, columns);
            if (allAssinedCourses.Rows != null && allAssinedCourses.Rows.Count > 0)
            {
                foreach (DataRow course in allAssinedCourses.Rows)
                {
                    assignments.Add(new Assignment(course.ItemArray[0].ToString(), course.ItemArray[1].ToString(), int.Parse(course.ItemArray[2].ToString()), int.Parse(course.ItemArray[3].ToString())));
                }
                }
            return assignments;
        }
    }
}