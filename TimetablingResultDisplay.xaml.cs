using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;
using System.Threading;
using ccbt;
using ccbt.Models;

namespace UniversityTimetabling
{
    /// <summary>
    /// Interaction logic for TimetablingResultDisplay.xaml
    /// </summary>
    public partial class TimetablingResultDisplay : System.Windows.Window
    {
		public bool isLoading = true;
		public TimetablingResultDisplay()
        {
            InitializeComponent();
			isLoading = false;
		}

        private void Window_Initialized(object sender, EventArgs e)
        {
            string currentSemester = getSemester();
            string degree = (comboBoxDegree.SelectedIndex >= 0) ? comboBoxDegree.SelectedValue.ToString() : "";
            int department = (comboBoxDepartment.SelectedIndex >= 0) ? int.Parse(comboBoxDepartment.SelectedValue.ToString()) : 0;
            int semester = (comboBoxSemester.SelectedIndex >= 0) ? int.Parse(comboBoxSemester.SelectedValue.ToString()) : 0;
            string name = txtFilterByCourse.Text;
            string teacher = txtFilterByTeacher.Text;
            string room = txtFilterByRoom.Text;
            int day = (comboBoxDays.SelectedIndex >= 0) ? int.Parse(comboBoxDays.SelectedValue.ToString()) : 0;

            string conditions = "";
            if (degree != "") conditions += " AND c.degree LIKE '%" + degree + "%' ";
            if (department > 0) conditions += " AND  d.id=" + department;
            conditions += (semester > 0) ? " AND c.semester_id=" + semester : (currentSemester == "spring" ? " AND (c.semester_id % 2) = 0 " : " AND (c.semester_id % 2) = 1 ");

            if (name != "") conditions += " AND c.name LIKE '%" + name + "%' ";
            if (teacher != "") conditions += " AND concat(t.name, ' ', t.surname) LIKE '%" + teacher + "%'";
            if (room != "") conditions += " AND r.name LIKE '%" + room + "%' ";
            if (day > 0) conditions += " AND day.id=" + day;

            string query = " WITH res as (SELECT c.id AS course_id,  c.name AS course_name, " +
								   " CASE WHEN c.detail = 'lecture' THEN 'Lecture' " +
								   " WHEN c.detail = 'numeric' THEN 'Numeric Exercise' " +
								   " WHEN c.detail = 'laboratory' THEN 'Laboratory Exercise'" +
								   " ELSE '' END AS course_type, " +
								   " t.id AS teacher_id, concat(t.name, ' ', t.surname)  AS teacher_name, " +
                                   " CASE WHEN day.id IS NULL THEN '-' ELSE day.name END AS day, " +
                                   " CASE WHEN r.id IS NULL THEN '-' ELSE r.name END AS room_name, " +
                                   " CASE WHEN p.id IS NULL THEN '-' ELSE p.name END AS period " +
                                   " FROM courses c " +
                                   " LEFT JOIN course_teacher_rel ctr ON c.id=ctr.course_id " +
                                   " LEFT JOIN teachers t ON ctr.teacher_id=t.id " +
                                   " LEFT JOIN timetable_" + currentSemester + " tt ON c.code = tt.course_code" +
                                   " LEFT JOIN days day ON day.id=(tt.day + 1) " +
                                   " LEFT JOIN subPeriods p ON p.id=(tt.start_period + 1) " +
                                   " LEFT JOIN rooms r ON tt.room_code=r.code " +
                                   " LEFT JOIN course_department_rel cdr ON cdr.course_id = c.id " +
                                   " LEFT JOIN departments d ON cdr.department_id = d.id " +
                                   " JOIN semesters s ON c.semester_id = s.id " +
                                   " WHERE c.parent_id IS NOT NULL " + conditions;


            query += ") " +
			  " SELECT MAX(res.course_id) as course_id, MAX(res.course_name) as course_name, MAX(res.course_type) as course_type, MAX(res.teacher_id) as teacher_id, " +
                  " MAX(res.teacher_name) as teacher_name, MAX(res.day) as day, MAX(res.room_name) as room_name, MAX(res.period) as period, depts = " +
                  " STUFF((SELECT DISTINCT ', ' + CAST(d.name AS VARCHAR(MAX)) " +
                      " FROM departments d " +
                            " JOIN course_department_rel cdr ON cdr.department_id = d.id " +
                          " WHERE cdr.course_id = MAX(res.course_id)  FOR XMl PATH('')), 1, 1, ''  ) " +
             " FROM res " +
             "GROUP BY res.course_id ORDER BY res.course_id ; ";

            System.Data.DataTable res = DBConnection.RAW_Select(query);
            ArrayList result = new ArrayList();
            foreach (DataRow r in res.Rows)
            {
                if (r.ItemArray[7].ToString() != "-")
                    r[7] = r.ItemArray[7] + "-" + getEndingPeriod(r.ItemArray[7].ToString(), int.Parse(r.ItemArray[0].ToString()));
            }
            courseperiodGrid.ItemsSource = res.DefaultView;

        }

        private string getSemester()
        {
            Dictionary<string, string> first = new Dictionary<string, string>();
            ArrayList data = DBConnection.Get("currentSemester", 1);
            first = (Dictionary<string, string>)data[0];

            string name;
            first.TryGetValue("name", out name);

            return name.ToLower();
        }

        private void removeAssignment_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)courseperiodGrid.SelectedItem;
            if (row != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to remove this assign?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    int courseId = (int)row["course_id"];
                    Dictionary<string, string> first = new Dictionary<string, string>();
                    string code;
                    ArrayList rec = DBConnection.Get("courses", courseId);
                    first = (Dictionary<string, string>)rec[0];
                    first.TryGetValue("code", out code);

                    int res = DBConnection.Delete("timetable_" + getSemester(), 0, " course_code='" + code + "'");
                    MessageBox.Show("Assignment was successfully deleted!", "Information");

                    Window_Initialized(sender, e);
                }
            }
            else
                MessageBox.Show("Please select a row to remove assign!", "Information");
        }

        private void refreshCoursePeriodPreferences_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Window_Initialized(sender, e);
        }

        private void comboBoxSemester_Initialized(object sender, EventArgs e)
        {
			comboBoxSemester.Items.Clear();
			if (int.Parse(comboBoxAcademicTerm.SelectedIndex.ToString()) >= 0) { 
			var semester_id = comboBoxAcademicTerm.SelectedValue.ToString();
		
			string conditions = "";
			if (int.Parse(semester_id) % 2 == 0)
				conditions = " (id % 2) = 0 ";
			else
				conditions = " (id % 2) = 1 ";
			DBConnection.FillCombo("semesters", comboBoxSemester, "name", conditions);	
			}
        }

        private void comboBoxDepartment_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("departments", comboBoxDepartment);
        }

        private void comboBoxDays_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("days", comboBoxDays);
        }

        private void comboBoxDegree_Initialized(object sender, EventArgs e)
        {
            comboBoxDegree.SelectedValuePath = "id";
            comboBoxDegree.DisplayMemberPath = "name";
            comboBoxDegree.Items.Add(new { id = "bachelor", name = "Bachelor" });
            comboBoxDegree.Items.Add(new { id = "master", name = "Master" });
        }

		private void comboBoxAcademicTerm_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!isLoading)
			{
				int id = 1;
				string values = "";
				var semester_id = comboBoxAcademicTerm.SelectedValue;
				if (Convert.ToInt32(semester_id) % 2 == 0)
					values = " name='Spring', module = '0'";
				else
					values = " name='Autumn', module = '1'";

				int result = DBConnection.Update("currentSemester", values, id);
				comboBoxSemester_Initialized(sender, e);
				Window_Initialized(sender, e);
				if (result > 0)
					MessageBox.Show("Current semester has been updated!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void comboBoxAcademicTerm_Initialized(object sender, EventArgs e)
		{
			comboBoxAcademicTerm.SelectedValuePath = "id";
			comboBoxAcademicTerm.DisplayMemberPath = "name";
			comboBoxAcademicTerm.Items.Add(new { id = 0, name = "Spring" });
			comboBoxAcademicTerm.Items.Add(new { id = 1, name = "Autumn" });
			Dictionary<string, string> first = new Dictionary<string, string>();
			ArrayList data = DBConnection.Get("currentSemester", 1);
			first = (Dictionary<string, string>)data[0];

			string index;
			first.TryGetValue("module", out index);

			comboBoxAcademicTerm.SelectedValue = Convert.ToInt32(index);
			comboBoxSemester_Initialized(sender, e);
		}

		private void courseperiodGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRowGrid.Items.Clear();
            DataRowView row = (DataRowView)courseperiodGrid.SelectedItem;
            if (row != null) selectedRowGrid.Items.Add(row);
        }

        private void courseperiodGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
			try
			{
				DataRowView row = (DataRowView)courseperiodGrid.SelectedItem;
				Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
				newWindowThread.SetApartmentState(ApartmentState.STA);
				newWindowThread.IsBackground = true;
				newWindowThread.Start();
				TimetableWindow win = new TimetableWindow(row.Row.ItemArray, newWindowThread);
				win.Owner = this;
				win.Closed += (s, eventarg) =>
				{
					Window_Initialized(s, eventarg);
				};
				win.ShowDialog();
			} catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
        }

        private void ThreadStartingPoint()
        {
            LoadingPanel.LoadingPanel bar = new LoadingPanel.LoadingPanel();
            bar.Show();
            System.Windows.Threading.Dispatcher.Run();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Window_Initialized(sender, e);
        }

        private void txtFilterByText_TextChanged(object sender, TextChangedEventArgs e)
        {
            Window_Initialized(sender, e);
        }

        public static string getEndingPeriod(string startPeriod, int courseId)
        {
            int subPeriodId = 0;
            string query = " SELECT id FROM subPeriods WHERE name='" + startPeriod + "'";
            System.Data.DataTable res = DBConnection.RAW_Select(query);

            ArrayList result = new ArrayList();
            foreach (DataRow r in res.Rows)
            {
                subPeriodId = int.Parse(r.ItemArray[0].ToString());
            }

            Dictionary<string, string> first = new Dictionary<string, string>();
            string detail, numberOfLectures;
            ArrayList rec = DBConnection.Get("courses", courseId);
            first = (Dictionary<string, string>)rec[0];
            first.TryGetValue("detail", out detail);
            first.TryGetValue("number_of_lectures", out numberOfLectures);

            var lectures = numberOfLectures.Split('+');
            switch (detail)
            {
                case "lecture":
                    numberOfLectures = lectures[0];
                    break;
                case "numeric":
                    numberOfLectures = lectures[1];
                    break;
                case "laboratory":
                    numberOfLectures = lectures[2];
                    break;
                default:
                    numberOfLectures = "";
                    break;
            }

            int breaks = 0;

            switch (int.Parse(numberOfLectures) % 2)
            {
                case 0:
                    breaks = (int.Parse(numberOfLectures) - 2) / 2;
                    break;
                case 1:
                    breaks = (int.Parse(numberOfLectures) - 1) / 2;
                    break;
            }

            int lecturePeriods = 3 * int.Parse(numberOfLectures);
            int endPeriod = subPeriodId + (3 * int.Parse(numberOfLectures)) + breaks;

            Dictionary<string, string> period = new Dictionary<string, string>();
            string name = "";
            ArrayList subPeriod = DBConnection.Get("subPeriods", endPeriod);
            if (subPeriod.Count > 0)
            {
                period = (Dictionary<string, string>)subPeriod[0];
                period.TryGetValue("name", out name);
            }
       
            return name;
        }

        private void generatePartialSolution()
        {
			List<string> rowsToInsert = new List<string>();
            string currentSemester = getSemester();
            string conditions = currentSemester == "spring" ? " AND (c.semester_id % 2) = 0 " : " AND (c.semester_id % 2) = 1 ";

            string query = " WITH res as (SELECT c.code AS course_code, c.detail, c.number_of_lectures AS lectures, " +
                                  " CONCAT((day.id-1), '') AS day, " +
                                  " r.code AS room_code, " +
                                  " CONCAT((p.id-1), '') AS period " +
                                  " FROM courses c " +
                                  " JOIN timetable_" + currentSemester + " tt ON c.code = tt.course_code" +
                                  " JOIN days day ON day.id=(tt.day + 1) " +
                                  " JOIN subPeriods p ON p.id=(tt.start_period + 1)" +
                                  " JOIN rooms r ON tt.room_code=r.code " +
                                  " WHERE c.parent_id IS NOT NULL " + conditions;


            query += ") " +
              " SELECT MAX(res.course_code) as course_code, MAX(res.detail) as detail, MAX(res.lectures) as lectures, " +
                  " MAX(res.day) as day, MAX(res.room_code) as room_code, MAX(res.period) as period " +
             " FROM res " +
             "GROUP BY res.course_code ORDER BY res.course_code ; ";

            System.Data.DataTable res = DBConnection.RAW_Select(query);
            foreach (DataRow r in res.Rows)
            {

                switch (r.ItemArray[1].ToString())
                {
                    case "lecture":
                        if (int.Parse(r.ItemArray[2].ToString().Split('+')[0]) > 0)
                        {
                            rowsToInsert.Add("" + r.ItemArray[0].ToString() + " " + r.ItemArray[4].ToString() + " " + r.ItemArray[3].ToString() + " " + r.ItemArray[5].ToString());
                        }
                        break;
                    case "numeric":
                        if (int.Parse(r.ItemArray[2].ToString().Split('+')[1]) > 0)
                        {
                            rowsToInsert.Add("" + r.ItemArray[0].ToString() + " " + r.ItemArray[4].ToString() + " " + r.ItemArray[3].ToString() + " " + r.ItemArray[5].ToString());
                        }
                        break;
                    case "laboratory":
                        if (int.Parse(r.ItemArray[2].ToString().Split('+')[2]) > 0)
                        {
                            rowsToInsert.Add("" + r.ItemArray[0].ToString() + " " + r.ItemArray[4].ToString() + " " + r.ItemArray[3].ToString() + " " + r.ItemArray[5].ToString());
                        }
                        break;

                }
            }

            string _basePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string fileName = _basePath + "\\Input\\PartialSolution.txt";

            try
            {
				FileStream fs1;
				if (File.Exists(fileName))
					fs1 = new FileStream(fileName, FileMode.Truncate, FileAccess.Write);
				else
					fs1 = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fs1);

				foreach (string line in rowsToInsert)
                    writer.WriteLine(line);
                writer.Close();
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

		private bool allCoursesAssigned(List<DataRowView> items)
		{
			List<DataRowView> unAssigned = items.FindAll(x => 
			(x.Row.ItemArray[5].ToString() == "-" || x.Row.ItemArray[5].ToString() == "") && 
			(x.Row.ItemArray[6].ToString() == "-" || x.Row.ItemArray[6].ToString() == "") && 
			(x.Row.ItemArray[7].ToString() == "-" || x.Row.ItemArray[7].ToString() == ""));

			if (unAssigned.Count > 0)
				return false;

			return true;
		}

		private void exportToWord_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (courseperiodGrid.Items.Count != 0)
            {				
                List<DataRowView> items = new List<DataRowView>();
                var gridItems = courseperiodGrid.Items;
                foreach (DataRowView item in gridItems)
                    items.Add(item);

				if (allCoursesAssigned(items))
				{
					List<DataRowView> assignedItems = items.FindAll(x => x.Row.ItemArray[5].ToString() != "-" && x.Row.ItemArray[6].ToString() != "-" && x.Row.ItemArray[7].ToString() != "-");

					if (assignedItems.Count > 0)
					{
						System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();

						sfd.Filter = "Word Documents (*.docx)|*.docx";

						sfd.FileName = getExportDocName() + ".docx";

						if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							DataRowView row = (DataRowView)courseperiodGrid.SelectedItem;
							Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
							newWindowThread.SetApartmentState(ApartmentState.STA);
							newWindowThread.IsBackground = true;
							newWindowThread.Start();
							Export_Data_To_Word(assignedItems, sfd.FileName, newWindowThread);
						}
					}
					else MessageBox.Show("No assigned classes!", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
				else MessageBox.Show("In order to export to word, all courses have to be assigned!", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

        private string getExportDocName()
        {
            string name = "Orari per ";
            if ((comboBoxDegree.SelectedIndex) >= 0)
            {
                name += "nivelin " + comboBoxDegree.SelectedValue + ", ";
            }
            if (comboBoxDepartment.SelectedIndex < 0 && comboBoxSemester.SelectedIndex < 0)
            {
                Dictionary<string, string> first = new Dictionary<string, string>();
                ArrayList data = DBConnection.Get("faculties", 1);
                first = (Dictionary<string, string>)data[0];

                string faculty;
                first.TryGetValue("abbreviation", out faculty);

                name += " " + faculty.Replace(" ", "");
            }
            else
            {
                if (comboBoxDepartment.SelectedIndex >= 0)
                {
                    name += " " + comboBoxDepartment.Text;
                }

                if (comboBoxSemester.SelectedIndex >= 0)
                {
                    name += ", Semestri " + comboBoxSemester.Text;
                }
            }
            name += ", " + ((getSemester() == "autumn") ? "vjeshte" : "pranvere");
            return name;
        }

        public void Export_Data_To_Word(List<DataRowView> items, string filename, Thread thread)
        {
            string actualSemester = getSemester();
            ArrayList semesterIds = GenerateInstance.getSemesters(" (id % 2) =" + ((actualSemester == "autumn") ? 1 : 0));
            ArrayList departmentIds = (comboBoxDepartment.SelectedIndex >= 0) ? (GenerateInstance.getDepartments(int.Parse(comboBoxDepartment.SelectedValue.ToString()))) : GenerateInstance.getDepartments();

            Dictionary<string, string> row = new Dictionary<string, string>();

            ArrayList data = DBConnection.Get("general_info", 1);
            row = (Dictionary<string, string>)data[0];

            string nrOfDays, periodsPerDay, editor;
            row.TryGetValue("number_of_days", out nrOfDays);
            row.TryGetValue("periods_per_day", out periodsPerDay);
            row.TryGetValue("editor", out editor);

			ArrayList subPeriodsNames = new ArrayList();
            System.Data.DataTable subPeriods = DBConnection.Select(" subPeriods ", " name ", " period_id <=" + (int.Parse(periodsPerDay) + 1), null, " id ");
            foreach (DataRow subP in subPeriods.Rows)
            {
                subPeriodsNames.Add(subP.ItemArray[0]);
            }

            object objMiss = System.Reflection.Missing.Value;
            object objEndOfDocFlag = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

            //Start Word and create a new document.
            _Application objApp;
            _Document objDoc;
            objApp = new Microsoft.Office.Interop.Word.Application();
            objDoc = objApp.Documents.Add(ref objMiss, ref objMiss, ref objMiss, ref objMiss);

            objDoc.PageSetup.PaperSize = WdPaperSize.wdPaperA4;
            objDoc.PageSetup.LeftMargin = 15;
            objDoc.PageSetup.RightMargin = 15;
            objDoc.PageSetup.BottomMargin = 15;
            objDoc.PageSetup.FooterDistance = 10;
            objDoc.PageSetup.TopMargin = 15;
            objDoc.PageSetup.HeaderDistance = 10;

            foreach (Microsoft.Office.Interop.Word.Section section in objDoc.Sections)
            {
                //Get the header range and add the header details.
                Range headerRange = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;

                string _basePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
               // string _basePath = Directory.GetParent(Directory.GetParent(_filePath).FullName).FullName;
                string file_name = _basePath + "\\upLogo.png";
                var shape = headerRange.InlineShapes.AddPicture(file_name, false, true);
                var convertedShape = shape.ConvertToShape();
                convertedShape.Height = 50;
                convertedShape.Width = 50;
                convertedShape.Top = -30;
                convertedShape.Left = -10;

                headerRange.Font.Size = 11;
                headerRange.Font.Name = "Times New Roman";
                headerRange.Collapse();
                headerRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                headerRange.Font.ColorIndex = WdColorIndex.wdBlack;
                headerRange.InsertAfter(getHeaderText());
            }

            //Get faculty Abbreviation
            Dictionary<string, string> faculty = new Dictionary<string, string>();
            
            ArrayList facultyData = DBConnection.Get("faculties", 1);
            faculty = (Dictionary<string, string>)facultyData[0];
            
            string abbr;
            faculty.TryGetValue("abbreviation", out abbr);
            
            #region
            foreach (int semesterId in semesterIds)
            {
                foreach (int departmentId in departmentIds)
                {
                    if ((semesterId == 1 || semesterId == 2 ) && abbr.Contains("FIEK"))
                    {
                        //GetGroups
                        string from = "courses c" +
                                   " LEFT JOIN course_department_rel cdr ON c.id = cdr.course_id ";
                        string conditions = " c.parent_id IS NOT NULL AND c.type='obl' AND c.semester_id= " + semesterId + " AND cdr.department_id= " + departmentId;

                        string actualGroup = null;
                        System.Data.DataTable lectureGrs = DBConnection.Select(from, " DISTINCT c.lecture_group ", conditions);
                        if (lectureGrs != null)
                        {
                            foreach (DataRow group in lectureGrs.Rows)
                            {
                                actualGroup = group.ItemArray[0].ToString();
                                List<WordAssignedData> lectureGroups = getAssignedClasses(semesterId, departmentId, "bachelor", items, actualGroup);
                                if (lectureGroups.Count > 0)
                                    generateTable(objDoc, objMiss, objEndOfDocFlag, lectureGroups, subPeriodsNames, nrOfDays, periodsPerDay, departmentId, semesterId, "bachelor", actualGroup);
                            }
                        }
                    }
                    else
                    {
                        List<WordAssignedData> lectureGroups = getAssignedClasses(semesterId, departmentId, "bachelor", items);
                        if (lectureGroups.Count > 0)
                           generateTable(objDoc, objMiss, objEndOfDocFlag, lectureGroups, subPeriodsNames, nrOfDays, periodsPerDay, departmentId, semesterId, "bachelor");
                         
                    }
                }
            }
            #endregion

            #region
            foreach (int semesterId in semesterIds)
            {
                foreach (int departmentId in departmentIds)
                {
                    List<WordAssignedData> lectureGroups = getAssignedClasses(semesterId, departmentId, "master", items);
                    if (lectureGroups.Count > 0)
                        generateTable(objDoc, objMiss, objEndOfDocFlag, lectureGroups, subPeriodsNames, nrOfDays, periodsPerDay, departmentId, semesterId, "master");
                }
            }
			#endregion

			//Insert footer
			foreach (Microsoft.Office.Interop.Word.Section section in objDoc.Sections)
			{
				//Get the footer range and add the footer details.
				Range footerRange = section.Footers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;			

				footerRange.Font.Size = 11;
				footerRange.Font.Name = "Times New Roman";
				footerRange.Collapse();
				footerRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
				footerRange.Font.ColorIndex = WdColorIndex.wdBlack;
				footerRange.InsertAfter("Përpiluar nga: " + editor + " _______________");
			}

			objDoc.SaveAs(filename);
			if(thread.IsAlive)
				thread.Abort();
            objApp.Visible = true;
        }

        private string getHeaderText()
        {
            Dictionary<string, string> first = new Dictionary<string, string>();
            ArrayList data = DBConnection.Get("faculties", 1);
            first = (Dictionary<string, string>)data[0];

            string name, university;
            first.TryGetValue("name", out name);
            first.TryGetValue("university", out university);

            string year = "";
            int actualYear = DateTime.Today.Year;
            int month = DateTime.Today.Month;
            if (month > 9)
                year = DateTime.Today.Year + " - " + (++actualYear);
            else
                year = (--actualYear) + " - " + DateTime.Today.Year;
            string other = "Orari i mësimit për vitin akademik " + year;

            string returnText = university + "\n" + name + "\n" + other;
            return returnText;
        }

        private void insertFooterText(
            List<Footer> footerSubjectData,
            List<Footer> footerTeacherData,
            List<Footer> footerRoomData,
            List<Footer> footerOtherData,
            _Document objDoc,
            object objEndOfDocFlag
            )
        {
            Microsoft.Office.Interop.Word.Paragraph objPara; //define paragraph object
            object oRng = objDoc.Bookmarks.get_Item(ref objEndOfDocFlag).Range; //go to end of the page
            objPara = objDoc.Content.Paragraphs.Add(ref oRng); //add paragraph at end of document
            objPara.Range.Font.Color = WdColor.wdColorGray50;
            objPara.Range.Font.Name = "Times New Roman";
			objPara.Range.Font.Size = 8;
         
            if (footerSubjectData.Count > 0)
            {
                // Insert subject data
                objPara.Range.Text = "\nShkurtesat e lëndëve: ";

                string txt = "";
                foreach (Footer subj in footerSubjectData)
                    txt += subj.key + " - " + subj.value + (subj.Equals(footerSubjectData.Last()) ? ";" : ", ");
               
                objPara.Range.InsertAfter(txt);
                objPara.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            }

            if (footerTeacherData.Count > 0)
            {
                // Insert teacher data
                objPara.Range.Text += "\nInicialet e profesorëve/asistentëve: ";

                string txt = "";
                foreach (Footer t in footerTeacherData)
                    txt += t.key + " - " + t.value + (t.Equals(footerTeacherData.Last()) ? ";" : ", ");

                objPara.Range.InsertAfter(txt);
                objPara.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            }

            if (footerRoomData.Count > 0)
            {
                // Insert room data
                objPara.Range.Text += "\nLlojet e hapësirave të mësimit: ";

                string txt = "";
                foreach (Footer r in footerRoomData)
                    txt += r.key + " - " + r.value + (r.Equals(footerRoomData.Last()) ? ";" : ", ");

                objPara.Range.InsertAfter(txt);
                objPara.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            }

            if (footerOtherData.Count > 0)
            {
                // Insert other data
                objPara.Range.Text += "\nShkurtesat tjera: ";

                string txt = "";
                foreach (Footer o in footerOtherData)
                    txt += o.key + " - " + o.value + (o.Equals(footerOtherData.Last()) ? ";" : ", ");

                objPara.Range.InsertAfter(txt);
                objPara.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            }
        }

        private List<WordAssignedData> getAssignedClasses(int semester_id, int department_id, string degree, List<DataRowView> items, string actualGroup = null)
        {
            string cIds = "";
            foreach (DataRowView cId in items)
            {
                cIds += (cIds == "") ? cId.Row.ItemArray[0].ToString() : (", " + cId.Row.ItemArray[0].ToString());
            }
            List<WordAssignedData> wordAssignedData = new List<WordAssignedData>();
            List<string> parentCodes = new List<string>();

            string columns = " c.id, c.name, c.lecture_group, c.numeric_group, c.laboratory_group, concat(t.name, ' ', t.surname) AS teacher, r.name as room, tt.day, tt.start_period, c.type, c.ects, c.detail, c.number_of_lectures, c.base_parent_code, p.name";
            string from = " courses c" +
                           " JOIN courses p ON p.code = c.base_parent_code " +
                           " LEFT JOIN course_department_rel cdr ON c.id = cdr.course_id " +
                           " LEFT JOIN course_teacher_rel ctr ON c.id=ctr.course_id" +
                           " LEFT JOIN teachers t ON ctr.teacher_id=t.id" +
                           " JOIN timetable_" + getSemester() + " tt ON tt.course_code=c.code" +
                           " JOIN rooms r on r.code = tt.room_code ";
            string conditions = " c.parent_id IS NOT NULL AND c.semester_id= " + semester_id + " AND cdr.department_id= " + department_id + " AND c.degree='" + degree + "' AND c.id IN (" + cIds + ")";
            if (actualGroup != null) conditions += " AND c.lecture_group='" + actualGroup + "'";

            System.Data.DataTable assignedLectures = DBConnection.Select(from, columns, conditions);

            wordAssignedData = processAssignedData(assignedLectures, ref parentCodes);
            if (assignedLectures != null && assignedLectures.Rows.Count > 0 && actualGroup != null)
            {
                //check if result contains all parent codes
                string fromQ = "courses c" +
                                    " LEFT JOIN course_department_rel cdr ON c.id = cdr.course_id ";
                string conditionsQ = " c.parent_id IS NOT NULL AND c.semester_id= " + semester_id + " AND cdr.department_id= " + department_id;
                //get Distinct groups of all lectures
                System.Data.DataTable lectureGroups = DBConnection.Select(fromQ, " DISTINCT c.base_parent_code ", conditionsQ);
                List<string> allCodes = new List<string>();
                if (assignedLectures != null && assignedLectures.Rows.Count > 0)
                {
                    foreach (DataRow row in assignedLectures.Rows)
                    {
                        allCodes.Add(row.ItemArray[0].ToString());
                    }
                }

                IEnumerable<string> differenceCodes = allCodes.Except(parentCodes);
                foreach (string code in differenceCodes)
                {
                    //Select randomly a group on this parent code
                    System.Data.DataTable unelectedGroups = DBConnection.RAW_Select("SELECT TOP 1 lecture_group FROM courses WHERE base_parent_code='" + code + "' ORDER BY RAND()");
                    if (unelectedGroups != null && unelectedGroups.Rows.Count > 0)
                    {
                        string gr = unelectedGroups.Rows[0].ItemArray[0].ToString();

                        string newConditions = " c.parent_id IS NOT NULL AND c.lecture_group='" + gr + "' AND c.detail IN ( 'lecture', 'numeric', 'laboratory') AND c.base_parent_code='" + code + "' AND c.id IN (" + cIds + ")";

                        System.Data.DataTable otherLectures = DBConnection.Select(from, columns, newConditions);
                        List<WordAssignedData> otherUnelectedData = processAssignedData(otherLectures, ref parentCodes);

                        wordAssignedData.AddRange(otherUnelectedData);
                    }
                }
            }

            return wordAssignedData;
        }

        private List<WordAssignedData> processAssignedData(System.Data.DataTable data, ref List<string> parentCodes)
        {
            List<WordAssignedData> wordAssignedData = new List<WordAssignedData>();
            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow row in data.Rows)
                {
                    if (!parentCodes.Contains(row.ItemArray[13].ToString())) parentCodes.Add(row.ItemArray[13].ToString());
                    int numberOfLectures = 0;
                    string detail = row.ItemArray[11].ToString();
                    var lectures = row.ItemArray[12].ToString().Split('+');
                    string classType = "";

                    switch (detail)
                    {
                        case "lecture":
                            numberOfLectures = int.Parse(lectures[0]);
                            classType = "L";
                            break;
                        case "numeric":
                            numberOfLectures = int.Parse(lectures[1]);
                            classType = "UN";
                            break;
                        case "laboratory":
                            numberOfLectures = int.Parse(lectures[2]);
                            classType = "UL";
                            break;
                        default:
                            numberOfLectures = 0;
                            break;
                    }

                    int breaks = 0;
                    switch (numberOfLectures % 2)
                    {
                        case 0:
                            breaks = (numberOfLectures - 2) / 2;
                            break;
                        case 1:
                            breaks = (numberOfLectures - 1) / 2;
                            break;
                    }

                    int lecturePeriods = 3 * numberOfLectures;
                    int endPeriod = int.Parse(row.ItemArray[8].ToString()) + lecturePeriods + breaks;

                    wordAssignedData.Add(new WordAssignedData()
                    {
                        id = int.Parse(row.ItemArray[0].ToString()),
                        name = row.ItemArray[1].ToString(),
                        parentName = row.ItemArray[14].ToString(),
                        lectureGroup = row.ItemArray[2].ToString(),
                        numericGroup = row.ItemArray[3].ToString(),
                        laboratoryGroup = row.ItemArray[4].ToString(),
                        teacher = row.ItemArray[5].ToString(),
                        room = row.ItemArray[6].ToString(),
                        day = int.Parse(row.ItemArray[7].ToString()),
                        startPeriod = int.Parse(row.ItemArray[8].ToString()),
                        type = row.ItemArray[9].ToString(),
                        ects = row.ItemArray[10].ToString(),
                        detail = detail,
                        numberOfLectures = numberOfLectures,
                        classType = classType,
                        endPeriod = endPeriod
                    });
                }
            }
                return wordAssignedData;
            }

        private string getTableInfo(int department, int semester, string dergree, string group = null)
        {
            //Programi i studimeve në nivelin bachelor “Inxhinieri Elektrike dhe Kompjuterike”, Vitit I, Semestri I, Grupi 1
            Dictionary<string, string> dept = new Dictionary<string, string>();
            ArrayList data = DBConnection.Get("departments", department);
            dept = (Dictionary<string, string>)data[0];

            string deptName;
            dept.TryGetValue("name", out deptName);

            Dictionary<string, string> sem = new Dictionary<string, string>();
            ArrayList semData = DBConnection.Get("semesters", semester);
            sem = (Dictionary<string, string>)semData[0];

            List<string> romanNr = new List<string> { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
            string year = romanNr[DivideRoundingUp(semester, 2) - 1];

            string semName;
            sem.TryGetValue("name", out semName);

            return "Programi i studimeve në nivelin " + dergree + " \"" + deptName + "\", Viti " + year + ", Semestri " + semName + (group != null ? ", Grupi " + int.Parse(Regex.Match(group, @"\d+$").Value) : "");
        }

        public static int DivideRoundingUp(int x, int y)
        {
            int remainder;
            int quotient = Math.DivRem(x, y, out remainder);
            return remainder == 0 ? quotient : quotient + 1;
        }

        private int findOverlapings(List<WordAssignedData> items)
        {
            int overlapings = 0;
            foreach (WordAssignedData item in items)
            {
                int nr = items.FindAll(a => a.id != item.id && ((a.startPeriod < item.endPeriod && a.startPeriod >= item.startPeriod) || (item.startPeriod > a.endPeriod && item.startPeriod <= a.startPeriod))).Count;

                if (nr > overlapings)
                    overlapings = nr;
            }

            return overlapings;
        }

        private string getAbbreviation(string text, ref List<Footer> footerData, bool nameInitials = false, bool roomAbbr = false)
        {
            string result = "";
            List<string> digraphs = new List<string>() { "dh", "gj", "ll", "nj", "rr", "sh", "th", "xh", "zh" };
            List<string> connecters = new List<string>() { "ku", "tek", "nga", "kudo", "ngado", "tekdo", "deri", "që", "qe", "kurdoherë", "se", "posa",
                                                           "kur", "qysh", "sapo", "sepse", "si", "pasi", "derisa", "gjersa", "përderisa", "meqenëse", "i",
                                                           "meqë", "ngaqë", "ngase", "me", "të", "te", "e", "sa", "aq", "sesa", "siç", "ashtu", "sikurse",
                                                           "në", "ne", "po", "nëse", "sikur", "është", "saqë", "megjithëse", "megjithëqe", "ndonëse",
                                                           "sado", "sido", "edhe", "sikur", "as", "dhe", "a", "apo", "o", "qofte", "kurse", "mirepo",
                                                           "ndersa", "teksa", "andaj", "ndaj", "pa", "pra", "prandaj", "domethene"};
            //Replace ë with e or ç with q

            if (nameInitials)
            {
                char[] splitChars = { ' ', '\t', '\n', '-' };
                string[] c = text.Split(splitChars);
                for (int i = 0; i < c.Length; i++)
                {
                    if (c[i].Length > 2 && digraphs.Contains(c[i].Substring(0, 2).ToLower()))
                        result += c[i].ToUpper().Substring(0, 1) + c[i].ToLower().Substring(1, 1) + ".";
                    else
                        result += c[i].Substring(0, 1) + ".";
                }
                // add initials to footerData
                if (footerData.FindAll(x => x.key == result && x.value == text).FirstOrDefault() == null)
                    footerData.Add(new Footer() { key = result, value = text });
            }
            else
            {
                char[] splitChars = { ' ', '\t', '\n' };
                string[] c;
                c = text.Split(splitChars);
                int temp;
                if (c.Length == 1 || (c.Length == 2 && (int.TryParse(c[1], out temp) || roomAbbr)))
                {
                    if (roomAbbr) result = c[0].Substring(0, 1) + ".";
                    else result = c[0].Substring(0, 3);
                    if (c.Length == 2)
                        result += " " + c[1];

                    return result;
                }
                //TryParse(numString, out number3);
                for (int i = 0; i < c.Length; i++)
                {
                    if (int.TryParse(c[i], out temp))
                        result += " " + c[i];
                    else if (c[i].Length > 2 && digraphs.Contains(c[i].Substring(0, 2).ToLower()) && !connecters.Contains(c[i]))
                        result += c[i].ToUpper().Substring(0, 1) + c[i].ToLower().Substring(1, 1);
                    else if (c[i].Length > 2 && (int.TryParse(c[i].Substring(0, 1), out temp) || (c[i].Substring(0, 3) == "Gr.")))
                        result += " " + c[i];
                    else if (c[i].Length >= 2 && (int.TryParse(c[i].Substring(0, 1), out temp)))
                         result += " " + c[i];
                    else if (!connecters.Contains(c[i]))
                        result += c[i].ToUpper().Substring(0, 1);
                }

                // Add data for footer
                if (!roomAbbr && footerData.FindAll(x => x.key == result && x.value == text).FirstOrDefault() == null)
                    footerData.Add(new Footer() { key = result, value = text });
            }
            return result;
        }

        private void generateTable(_Document objDoc, object objMiss, object objEndOfDocFlag, List<WordAssignedData> lectureGroups, ArrayList subPeriodsNames, string nrOfDays, string periodsPerDay, int departmentId, int semesterId, string degree, string group = null)
        {
            List<Footer> footerSubjectData = new List<Footer>();
            List<Footer> footerTeacherData = new List<Footer>();
            List<Footer> footerRoomData = new List<Footer>() { new Footer() { key = "A", value = "Amfiteatër" }, new Footer() { key = "S", value = "Sallë" }, new Footer() { key = "L", value = "Laborator" } };
            List<Footer> footerOtherData = new List<Footer>() { new Footer() {key = "L", value = "Ligjërata" }, new Footer() { key = "UN", value = "Ushtrime numerike" }, new Footer() { key = "UL", value = "Ushtrime laboratorike" },
                            new Footer() {key = "O", value = "Lëndë e obliguar" }, new Footer() { key = "Z", value = "Lëndë zgjedhore" }, new Footer() { key = "h", value = "Numri i orëve mësimore" }, new Footer() { key = "k", value = "Numri i kredive ECTS" }};

            //let only used items to footerOtherData
            List<int> indexesToRemove = new List<int>();
            foreach (Footer item in footerOtherData)
            {
                if (item.key != "h" && item.key != "k")
                {
                    if (lectureGroups.FindAll(x => x.classType == item.key || x.type.ToCharArray()[0].ToString().ToUpper() == item.key).FirstOrDefault() == null)
                        indexesToRemove.Add(footerOtherData.IndexOf(item));
                }
            }
            if (indexesToRemove.Count > 0)
                foreach (int index in indexesToRemove.OrderByDescending(x => x))
                    footerOtherData.RemoveAt(index);

            //let only used items to footerRoomData 
            List<int> roomIndexesToRemove = new List<int>();
            foreach (Footer item in footerRoomData)
                if (lectureGroups.FindAll(x => x.room.StartsWith(item.key)).FirstOrDefault() == null)
                    roomIndexesToRemove.Add(footerRoomData.IndexOf(item));

            if (roomIndexesToRemove.Count > 0)
                foreach (int index in roomIndexesToRemove.OrderByDescending(x => x))
                    footerRoomData.RemoveAt(index);

            if (objDoc.Words.Count > 1)
                objDoc.Words.Last.InsertBreak(WdBreakType.wdPageBreak);
            int maxDay = lectureGroups.Max(x => x.day);
            if (int.Parse(nrOfDays) > 5 && (maxDay + 1) < int.Parse(nrOfDays))
                nrOfDays = "5";

            //Insert a paragraph at the end of the document.
            Microsoft.Office.Interop.Word.Paragraph objPara2; //define paragraph object
            object oRng = objDoc.Bookmarks.get_Item(ref objEndOfDocFlag).Range; //go to end of the page
            objPara2 = objDoc.Content.Paragraphs.Add(ref oRng); //add paragraph at end of document
            objPara2.Range.Font.Color = WdColor.wdColorGray50;
            objPara2.Range.Font.Name = "Times New Roman";
            objPara2.Range.Text = getTableInfo(departmentId, semesterId, degree, group); //add some text in paragraph
            objPara2.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            objPara2.Format.SpaceAfter = 2; //define some style
            objPara2.Range.InsertParagraphAfter(); //insert paragraph
            
            //Insert a table
            Microsoft.Office.Interop.Word.Table objTab1; //create table object
            Range objWordRng = objDoc.Bookmarks.get_Item(ref objEndOfDocFlag).Range; //go to end of document
            objTab1 = objDoc.Tables.Add(objWordRng, ((int.Parse(periodsPerDay) * 4) + 1), (int.Parse(nrOfDays) + 1), ref objMiss, ref objMiss); //add table object in word document
           // objTab1.Range.ParagraphFormat.SpaceAfter = 2;
            objTab1.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
            objTab1.Borders.InsideColor = WdColor.wdColorGray50;
            objTab1.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
            objTab1.Borders.OutsideColor = WdColor.wdColorGray50;
            objTab1.AllowPageBreaks = false;
           
            int iRow, iCols;
            string strText;
            objTab1.Range.Font.Size = 9;
            objTab1.Range.Font.Name = "Times New Roman";
            #region
            for (iRow = 1; iRow <= ((int.Parse(periodsPerDay) * 4) + 1); iRow++)
                for (iCols = 1; iCols <= (int.Parse(nrOfDays) + 1); iCols++)
                {
                    if (iCols == 1 && iRow == 1) strText = "Koha";
                    else if (iCols == 1 && iRow != 1)
                    {
                        strText = subPeriodsNames[iRow - 2].ToString() + " - " + subPeriodsNames[iRow - 1].ToString();
                    }
                    else if (iRow == 1 && iCols != 1)
                    {
                        switch (iCols)
                        {
                            case 2:
                                strText = "E hënë";
                                break;
                            case 3:
                                strText = "E martë";
                                break;
                            case 4:
                                strText = "E mërkurë";
                                break;
                            case 5:
                                strText = "E enjte";
                                break;
                            case 6:
                                strText = "E premte";
                                break;
                            case 7:
                                strText = "E shtunë";
                                break;
                            case 8:
                                strText = "E diel";
                                break;
                            default:
                                strText = "";
                                break;
                        }
                    }
                    else
                    {
                        strText = "";
                    }
                    objTab1.Cell(iRow, iCols).Range.Text = strText; //add some text to cell
                }
            #endregion
            objTab1.Rows[1].Range.Font.Bold = 1; //make first row of table BOLD
            objTab1.Columns[1].AutoFit();
            objTab1.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitWindow);
            foreach (Cell a in objTab1.Range.Cells)
                a.Range.Font.Color = WdColor.wdColorBlack;
            List<int> daySplitedCells = new List<int>();

            #region
            for (int d = 0; d < int.Parse(nrOfDays); d++)
            {
                List<WordAssignedData> dayLectures = lectureGroups.FindAll(x => x.day == d).OrderBy(x => x.startPeriod).ToList();
                int overlapingLectures = findOverlapings(dayLectures);
                int addedDays = 0;
                addedDays = daySplitedCells.Sum();
                daySplitedCells.Add(overlapingLectures > 1 ? overlapingLectures - 1 : overlapingLectures);

                for (int dRow = 2; dRow <= ((int.Parse(periodsPerDay) * 4) + 1); dRow++)
                {
                    if (overlapingLectures > 0)
                    {
                        objTab1.Cell(dRow, (addedDays + d + 2)).Split(1, overlapingLectures > 1 ? overlapingLectures : (overlapingLectures + 1));
                    }


                }
                foreach (WordAssignedData lecture in dayLectures)
                {
                    int startPoint = (addedDays + lecture.day + 2);
                    int endPoint = (daySplitedCells.Sum() + lecture.day + 2);

                    for (int i = startPoint; i <= endPoint; i++)
                    {
                        try
                        {
                            var actualCell = objTab1.Cell(lecture.startPeriod + 2, i);
                            if (actualCell.Range.Text == null || actualCell.Range.Text == "\r\a")
                            {
                                int mergedCells = (lecture.endPeriod + 1) - (lecture.startPeriod + 2);
                                actualCell.Merge(objTab1.Cell(lecture.endPeriod + 1, i));

                                float colWidth = actualCell.Width;
                                float colHeight = actualCell.Height;

                                string courseData = lecture.name;

                                if (System.Windows.Forms.TextRenderer.MeasureText(courseData, new System.Drawing.Font("Times New Roman", 9, System.Drawing.FontStyle.Regular)).Width > colWidth)
                                {
                                   // footerSubjectData;
                                    string actualText = courseData.Remove(0, lecture.parentName.Length);
                                    courseData = getAbbreviation(lecture.parentName, ref footerSubjectData, false, false) + actualText;
                                }

                                if (lecture.teacher != "")
                                {
                                   // footerTeacherData;
                                    if (System.Windows.Forms.TextRenderer.MeasureText(lecture.teacher, new System.Drawing.Font("Times New Roman", 9, System.Drawing.FontStyle.Regular)).Width > colWidth)
                                    {
                                        courseData += ",\n" + getAbbreviation(lecture.teacher, ref footerTeacherData, true, false);
                                    }
                                    else courseData += ",\n" + lecture.teacher;
                                }
                                if (lecture.room != "")
                                {
                                    // footerRoomData;
                                    if (System.Windows.Forms.TextRenderer.MeasureText(lecture.room, new System.Drawing.Font("Times New Roman", 9, System.Drawing.FontStyle.Regular)).Width > colWidth)
                                    {
                                        courseData += ",\n" + getAbbreviation(lecture.room, ref footerRoomData, false, true);
                                    }
                                    else courseData += ",\n" + lecture.room;
                                }

                                string periodStr = "\n<" + subPeriodsNames[lecture.startPeriod].ToString() + "-" + subPeriodsNames[lecture.endPeriod] + ">";
                                if (System.Windows.Forms.TextRenderer.MeasureText(periodStr, new System.Drawing.Font("Times New Roman", 9, System.Drawing.FontStyle.Regular)).Width > colWidth)
                                {
                                    actualCell.Range.Font.Size -= 2;
                                }
                                courseData += "\n<" + subPeriodsNames[lecture.startPeriod].ToString() + "-" + subPeriodsNames[lecture.endPeriod] + ">";
                                courseData += "\n(" + lecture.classType + ", " + lecture.type.ToCharArray()[0].ToString().ToUpper() + ", " + lecture.numberOfLectures + "h, " + lecture.ects + "k)";

                                if (courseData.Split("\n".ToArray(), StringSplitOptions.None).Count() > mergedCells)
                                    actualCell.Range.Font.Size -= 2;

								if (actualCell.Range.Font.Size < 9 && lecture.numberOfLectures == 1)
								{
									courseData = Regex.Replace(courseData, @",\n|\n", ", ");
									courseData = Regex.Replace(courseData, @"<|>", "");
								}
								actualCell.Range.Text = courseData;
                                actualCell.HeightRule = WdRowHeightRule.wdRowHeightExactly;
                                actualCell.WordWrap = true;

                                objTab1.Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter; //.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                               
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }

                }
            }
            #endregion
            objTab1.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitWindow);

            //Merge day empty cells
            mergeEmptyCells(objTab1, daySplitedCells);

            //Add some text after table
            insertFooterText(footerSubjectData, footerTeacherData, footerRoomData, footerOtherData, objDoc, objEndOfDocFlag);

            objWordRng = objDoc.Bookmarks.get_Item(ref objEndOfDocFlag).Range;
            objWordRng.InsertParagraphAfter(); //put enter in document
        }

        private void mergeEmptyCells(Microsoft.Office.Interop.Word.Table objTab1, List<int> daySplitedCells)
        {
            var a = daySplitedCells;
            for (int day = daySplitedCells.Count - 1; day >= 0; day--)
            {
                if (daySplitedCells[day] > 0)
                {
                    int startCol = day + 2 + daySplitedCells.Take(day + 1).Sum();
                    int endCol = startCol - daySplitedCells[day];
                    for (int col = startCol; col > endCol; col--)
                        for (int row = objTab1.Rows.Count; row > 1; row--)
                        {
                            try
                            {
                                Cell actualCell = objTab1.Cell(row, col);
                                Cell prevCell;
                                try { prevCell = objTab1.Cell(row, col - 1); }
                                catch (Exception) { prevCell = null; }

                                if (prevCell != null && col - 1 > 0)
                                {
                                    if ((actualCell.Range.Text == null && prevCell.Range.Text == null) || (actualCell.Range.Text == "\r\a" && prevCell.Range.Text == "\r\a"))
                                    {
                                        objTab1.Cell(row, col).Merge(objTab1.Cell(row, col - 1));
                                    }
                                    else if ((actualCell.Range.Text == null && prevCell.Range.Text != null) || (actualCell.Range.Text == "\r\a" && prevCell.Range.Text != "\r\a"))
                                    {
                                        objTab1.Cell(row, col).Merge(objTab1.Cell(row, col - 1));
                                        objTab1.Cell(row, col - 1).Range.Font.Size += 2;
                                        actualCell.HeightRule = WdRowHeightRule.wdRowHeightExactly;
                                        actualCell.WordWrap = true;
                                    }
                                }
                                else
                                {
                                    Cell nextCell = objTab1.Cell(row - 1, col);
                                    Cell actual = objTab1.Cell(row, col);

                                    if ((actual.Range.Text == null && nextCell.Range.Text == null) || (actual.Range.Text == "\r\a" && nextCell.Range.Text == "\r\a"))
                                    {
                                        objTab1.Cell(row, col).Merge(objTab1.Cell(row - 1, col));
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                }
            }
        }

        private void comboClearFilters_Initialized(object sender, EventArgs e)
        {
            comboClearFilters.SelectedValuePath = "id";
            comboClearFilters.DisplayMemberPath = "name";
            comboClearFilters.Items.Add(new { id = "degree", name = "Clear Degree" });
            comboClearFilters.Items.Add(new { id = "department", name = "Clear Program" });
            comboClearFilters.Items.Add(new { id = "semester", name = "Clear Semester" });
            comboClearFilters.Items.Add(new { id = "teacher", name = "Clear Teacher" });
            comboClearFilters.Items.Add(new { id = "course", name = "Clear Class" });
            comboClearFilters.Items.Add(new { id = "room", name = "Clear Room" });
            comboClearFilters.Items.Add(new { id = "all", name = "Clear All Filters" });
        }

        private void comboClearFilters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedText = comboClearFilters.SelectedValue.ToString();
            switch (selectedText)
            {
                case "degree":
                    comboBoxDegree.SelectedIndex = -1;
                    break;
                case "department":
                    comboBoxDepartment.SelectedIndex = -1;
                    break;
                case "semester":
                    comboBoxSemester.SelectedIndex = -1;
                    break;
                case "teacher":
                    txtFilterByTeacher.Text = null;
                    break;
                case "course":
                    txtFilterByCourse.Text = null;
                    break;
                case "room":
                    txtFilterByRoom.Text = null;
                    break;
                case "all":
                    comboBoxDegree.SelectedIndex = -1;
                    comboBoxSemester.SelectedIndex = -1;
                    comboBoxDepartment.SelectedIndex = -1;
                    comboBoxDays.SelectedIndex = -1;
                    txtFilterByCourse.Text = null;
                    txtFilterByTeacher.Text = null;
                    txtFilterByRoom.Text = null;
                    break;
            }

            Window_Initialized(sender, e);
        }

        private void generateRemaining_MouseUp(object sender, MouseButtonEventArgs e)
        {
			MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to generate remaining timetable?", "Generate Remaining Timetable Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (messageBoxResult == MessageBoxResult.Yes)
			{
				try
				{
					Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
					newWindowThread.SetApartmentState(ApartmentState.STA);
					newWindowThread.IsBackground = true;
					newWindowThread.Start();

					generatePartialSolution();
					GenerateTimetabling timetabling = new GenerateTimetabling();
					timetabling.generateRemainingTimetabling( "PartialSolution", newWindowThread);

					Window_Initialized(sender, e);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			}
        }

        private void generateAll_MouseUp(object sender, MouseButtonEventArgs e)
        {
			MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to generate the complete timetable? This will delete all assigned courses!", "Generate Timetable Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (messageBoxResult == MessageBoxResult.Yes)
			{
				try
				{
					Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
					newWindowThread.SetApartmentState(ApartmentState.STA);
					newWindowThread.IsBackground = true;
					newWindowThread.Start();

					GenerateTimetabling timetabling = new GenerateTimetabling();
					timetabling.generateTimetabling(newWindowThread);

					Window_Initialized(sender, e);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			}
        }
    }

    class Footer
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    class WordAssignedData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string parentName { get; set; }
        public string lectureGroup { get; set; }
        public string numericGroup { get; set; }
        public string laboratoryGroup { get; set; }
        public string teacher { get; set; }
        public string room { get; set; }
        public int day { get; set; }
        public int startPeriod { get; set; }
        public string type { get; set; }
        public string ects { get; set; }
        public string detail { get; set; }
        public int numberOfLectures { get; set; }
        public string classType { get; set; }
        public int endPeriod { get; set; }
    }
}
