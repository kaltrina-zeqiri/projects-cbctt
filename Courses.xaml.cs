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
    /// Interaction logic for Courses.xaml
    /// </summary>
    public partial class Courses : Window
    {
        public Courses()
        {
            InitializeComponent();
        }

        private void courses_Initialized(object sender, EventArgs e)
        {
			filterCourse();
        }

        private void addCourse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CourseAddEdit course = new CourseAddEdit(this);
			course.Owner = this;
			course.Closed += (s, eventarg) =>
			{
				filterCourse();
			};
			course.ShowDialog();
        }

        private void editCourse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)coursesGrid.SelectedItem;
            if (row != null)
            {
                int id = (int)row["ID"];
                var courseData = DBConnection.Get("courses", id);
                var item = (Dictionary<string, string>)courseData[0];

                string parent;
                item.TryGetValue("parent_id", out parent);
                if (parent != "")
                {
                    MessageBox.Show("Only main course can be edited!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
             
                string name = (string)row["Name"];
                string code = (string)row["Code"];
                string ects = row["Ects"].ToString();
                string semester = row["Semester"].ToString();
                string type; item.TryGetValue("type", out type);
                string[] number_of_lectures = ((string)row["Lectures"]).Split('+');
                string nr_lectures = number_of_lectures.Length >= 0 ? number_of_lectures[0] : "0";
                string numeric_lectures = number_of_lectures.Length >= 1 ? number_of_lectures[1] : "0";
                string laboratory_lectures = number_of_lectures.Length >= 2 ? number_of_lectures[2] : "0";
				string min_days = row["Min_Days"].ToString();
                string degree; item.TryGetValue("degree", out degree);
                string double_lectures = row["Double_Lectures"].ToString();
                string number_of_students = row["Students"].ToString();
                
                string groups = row["Groups"].ToString();

                DataTable semestersData = DBConnection.Select("semesters", "*", " name = '" + semester + "'");              
                var selectedSemester = semestersData.Rows[0].ItemArray;

                //Get all lecture groups of the course
                DataTable lectureGroups = DBConnection.Select("courses", "*", " parent_id = " + id + " AND detail = 'lecture'");
                ArrayList baseCourseGroupIds = new ArrayList();
                foreach (DataRow lectureRow in lectureGroups.Rows)
                {
                    //First column is ID
                    baseCourseGroupIds.Add(lectureRow.ItemArray[0]);
                }
                //Get all room added for lectures
                DataTable lectureRooms = getRooms(baseCourseGroupIds);

                //Get all numeric exercises groups of the course
                DataTable numExerciseGroups = DBConnection.Select("courses", "*", " parent_id IN ( " + String.Join(",", baseCourseGroupIds.ToArray()) + ") AND detail = 'numeric'");
                ArrayList numExerciseGroupIds = new ArrayList();
                DataTable numericExRooms = new DataTable();
                if (numExerciseGroups.Rows.Count != 0)
                {
                    foreach (DataRow numExerciseRow in numExerciseGroups.Rows)
                    {
                        //First column is ID
                        numExerciseGroupIds.Add(numExerciseRow.ItemArray[0]);
                    }
                    //Get all room added for numeric exercises
                    numericExRooms = getRooms(numExerciseGroupIds);
                }
                //Get all laboratory exercises groups of the course
                DataTable labExerciseGroups = DBConnection.Select("courses", "*", " parent_id IN ( " + 
                            String.Join(",", (numExerciseGroupIds.Count > 0) ? numExerciseGroupIds.ToArray() : baseCourseGroupIds.ToArray()) + ") AND detail = 'laboratory'");
                ArrayList labExerciseGroupIds = new ArrayList();
                DataTable laboratoryExRooms = new DataTable();
                if (labExerciseGroups.Rows.Count != 0)
                {
                    foreach (DataRow labExerciseRow in labExerciseGroups.Rows)
                    {
                        //First column is ID
                        labExerciseGroupIds.Add(labExerciseRow.ItemArray[0]);
                    }
                    //Get all room added for laboratory exercises
                    laboratoryExRooms = getRooms(labExerciseGroupIds);
                }
                ArrayList allCourseIds = new ArrayList();
                allCourseIds.AddRange(baseCourseGroupIds);
                allCourseIds.AddRange(numExerciseGroupIds);
                allCourseIds.AddRange(labExerciseGroupIds);

                ArrayList teachersDetails = getTeachers(allCourseIds);
                ArrayList departments = getDepartments(allCourseIds);

                CourseAddEdit course = new CourseAddEdit(this);
				course.Owner = this;
				course.Closed += (s, eventarg) =>
				{
					filterCourse();
				};
				course.txtId.Text = id.ToString();
                course.txtName.Text = name;
                course.comboBoxMinDays.SelectedValue = min_days;
                course.comboBoxEcts.SelectedValue = ects;
                course.txtCode.Text = code;
                course.comboBoxNrLectures.SelectedValue = nr_lectures;
                course.comboBoxNrNumExercLectures.SelectedValue = numeric_lectures;
                course.comboBoxNrLabExercLectures.SelectedValue = laboratory_lectures;
                course.txtNrStudents.Text = number_of_students;
                course.comboBoxGroups.SelectedValue = groups;
                course.comboBoxSemester.SelectedValue = selectedSemester[0];
                course.comboBoxType.SelectedValue = type.Trim();
                course.comboBoxDegree.SelectedValue = degree.Trim();
                course.comboBoxNrExerciseGroups.SelectedValue = numExerciseGroupIds.Count.ToString();
                course.comboBoxLabExercisesGroups.SelectedValue = labExerciseGroupIds.Count.ToString();
                course.comboBoxDoubleLectures.SelectedValue = (double_lectures == "True" || double_lectures == "1") ? 1 : 0;
                foreach(DataRow l in lectureRooms.Rows)
                    course.courseRoomRelationLecturesGrid.Items.Add(new CourseRooms() { RoomID = Int32.Parse(l.ItemArray[0].ToString()), RoomName = l.ItemArray[1].ToString() });

                if (numericExRooms != null)
                {
                    foreach (DataRow n in numericExRooms.Rows)
                        course.courseRoomRelationNumExGrid.Items.Add(new CourseRooms() { RoomID = Int32.Parse(n.ItemArray[0].ToString()), RoomName = n.ItemArray[1].ToString() });
                }
                if (laboratoryExRooms != null)
                {
                    foreach (DataRow la in laboratoryExRooms.Rows)
                        course.courseRoomRelationLabExGrid.Items.Add(new CourseRooms() { RoomID = Int32.Parse(la.ItemArray[0].ToString()), RoomName = la.ItemArray[1].ToString() });
                }
                foreach (CourseTeachers t in teachersDetails)
                    course.courseTeacherRelationGrid.Items.Add(t);

                foreach (ComboDepartments d in departments)
                    course.courseDepartmentsGrid.Items.Add(d);
				course.ShowDialog();
			}
            else
                MessageBox.Show("Please select a row to edit!", "Warinig", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void deleteCourse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //Also delete all child courses
            DataRowView row = (DataRowView)coursesGrid.SelectedItem;
            if (row != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to delete this course?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    int id = (int)row["ID"];

                    deleteCoursesCascade(id);

                    courses_Initialized(sender, e);
                }
            }
            else
                MessageBox.Show("Please select a row to delete!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void deleteCoursesCascade(int id, bool hideMessage = false)
        {
			ArrayList codes = new ArrayList();
			DataTable childCourses = DBConnection.Select("courses", " id, code, semester_id ", " parent_id = " + id);
            ArrayList childCoursesIds = new ArrayList();
			int semester = 0;
			foreach (DataRow childCourse in childCourses.Rows)
            {
                childCoursesIds.Add(childCourse.ItemArray[0]);
                codes.Add(childCourse.ItemArray[1]);
				semester = int.Parse(childCourse.ItemArray[2].ToString());
			}

            //Get all numeric exercises groups of the course
            DataTable numExerciseGroups = DBConnection.Select("courses", " id, code ", " parent_id IN ( " + String.Join(",", childCoursesIds.ToArray()) + ") AND detail = 'numeric'");
            ArrayList numExerciseGroupIds = new ArrayList();
            
            foreach (DataRow numExerciseRow in numExerciseGroups.Rows)
            {
                numExerciseGroupIds.Add(numExerciseRow.ItemArray[0]);
                codes.Add(numExerciseRow.ItemArray[1]);
            }

            //Get all laboratory exercises groups of the course
            DataTable labExerciseGroups = DBConnection.Select("courses", " id, code ", " parent_id IN ( " +
                        String.Join(",", (numExerciseGroupIds.Count > 0) ? numExerciseGroupIds.ToArray() : childCoursesIds.ToArray()) + ") AND detail = 'laboratory'");
            ArrayList labExerciseGroupIds = new ArrayList();
            foreach (DataRow labExerciseRow in labExerciseGroups.Rows)
            {
                labExerciseGroupIds.Add(labExerciseRow.ItemArray[0]);
				codes.Add(labExerciseRow.ItemArray[1]);
			}

            foreach (int lab in labExerciseGroupIds)
                DBConnection.Delete("courses", lab);

            foreach (int num in numExerciseGroupIds)
                DBConnection.Delete("courses", num);

            foreach (int lect in childCoursesIds)
                DBConnection.Delete("courses", lect);

            int result = DBConnection.Delete("courses", id);
            if (result > 0)
            {
                //Delete all course assignment
                string table = "timetable_" + (((semester % 2) == 0) ? "spring" : "autumn");
                foreach (string code in codes)
                    DBConnection.Delete(table, 0, " course_code='" + code + "'");

               if(!hideMessage)
					MessageBox.Show("Successfully deleted Course!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void refreshCourse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            courses_Initialized(sender, e);
        }

        private void comboBoxSemester_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("semesters", comboBoxSemesters);
        }

        private void comboBoxFilterByDegree_Initialized(object sender, EventArgs e)
        {
            comboBoxFilterByDegree.SelectedValuePath = "id";
            comboBoxFilterByDegree.DisplayMemberPath = "name";
            comboBoxFilterByDegree.Items.Add(new { id = "bachelor", name = "Bachelor" });
            comboBoxFilterByDegree.Items.Add(new { id = "master", name = "Master" });
        }

        private void comboBoxDetail_Initialized(object sender, EventArgs e)
        {
            comboBoxDetail.SelectedValuePath = "id";
            comboBoxDetail.DisplayMemberPath = "name";
            comboBoxDetail.Items.Add(new { id = "base", name = "Courses" });
            comboBoxDetail.Items.Add(new { id = "lecture", name = "Lectures" });
            comboBoxDetail.Items.Add(new { id = "numeric", name = "Numeric Exercises" });
            comboBoxDetail.Items.Add(new { id = "laboratory", name = "Laboratory Exercises" });
        }

        public void filterCourse()
        {
            string detail = (comboBoxDetail.SelectedIndex >= 0) ? comboBoxDetail.SelectedValue.ToString() : "base";
            string name = txtFilterByName.Text;
            int department = (comboBoxDepartment.SelectedIndex >= 0) ? int.Parse(comboBoxDepartment.SelectedValue.ToString()) : 0;
            int semester = (comboBoxSemesters.SelectedIndex >= 0) ? Convert.ToInt32(comboBoxSemesters.SelectedValue) : 0;
            string degree = (comboBoxFilterByDegree.SelectedIndex >= 0) ? comboBoxFilterByDegree.SelectedValue.ToString() : "";

            string query = " WITH res as (SELECT c.id AS id,  c.name AS name, c.code AS code, c.ects AS ects, s.name AS semester, c.groups AS groups,c.type AS type, " +
                                  " c.number_of_lectures AS lectures, c.min_days AS min_days, c.degree AS degree, c.double_lectures AS double_lectures," +
                                  " c.number_of_students AS students, cc.name AS parent " +
                                  " FROM courses c " +
                                  " LEFT JOIN courses cc ON cc.id = c.parent_id " +
                                  " LEFT JOIN course_department_rel cdr ON cdr.course_id = c.id " +
                                  " LEFT JOIN departments d ON cdr.department_id = d.id " +
                                  " JOIN semesters s ON c.semester_id = s.id " +
                                  " WHERE c.detail= '" + detail + "'";

            if (name != "") query += " AND c.name LIKE '%" + name + "%' ";
            if (department > 0) query += " AND  d.id=" + department;
            if (semester > 0) query += " AND c.semester_id = '" + semester + "' ";
            if (degree != "") query += " AND c.degree LIKE '%" + degree + "%' ";

			query += ") " +
			  " SELECT MAX(res.id) as id, MAX(res.name) as name, MAX(res.code) as code, MAX(res.code) as code, " +
				  " MAX(res.ects) as ects, MAX(res.semester) as semester, MAX(res.groups) as groups, MAX(res.type) as type, " +
				  " MAX(res.lectures) as lectures, MAX(res.min_days) as min_days, MAX(res.degree) as degree, " +
				  " MAX(cast(res.double_lectures as int)) as double_lectures, " +
				  " MAX(res.students) as students, MAX(res.parent) as parent, depts = " +
				  " STUFF((SELECT DISTINCT ', ' + CAST(d.name AS VARCHAR(MAX)) " +
					  " FROM departments d " +
							" JOIN course_department_rel cdr ON cdr.department_id = d.id " +
						  " WHERE cdr.course_id = MAX(res.id)  FOR XMl PATH('')), 1, 1, ''  ) " +
			 " FROM res " +
			 " GROUP BY res.id " +
			 " ORDER BY MAX(res.name); ";

			coursesGrid.ItemsSource = DBConnection.RAW_Select(query).DefaultView;
        }

        private void clearFilterCourse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.courses_Initialized(sender, e);
            comboBoxDetail.SelectedIndex = -1;
            txtFilterByName.Text = null;
            comboBoxDepartment.SelectedIndex = -1;
            comboBoxSemesters.SelectedIndex = -1;
            comboBoxFilterByDegree.SelectedIndex = -1;
        }

        public DataTable getRooms(ArrayList courseIds)
        {
            string Columns = " DISTINCT(r.id) AS RoomId, r.name AS RoomName";
            string From = "rooms r";
            string Conditions = " r.id NOT IN (SELECT room_id FROM courses_rooms_rel WHERE course_id IN (" + String.Join(",", courseIds.ToArray()) + ")) ";

            DataTable res = DBConnection.Select(From, Columns, Conditions);
            
            return res;
        }

        public ArrayList getTeachers(ArrayList courseIds)
        {
            string Columns = " DISTINCT(t.id) AS teacherId, concat(t.name, ' ', t.surname) AS teacherName";
            string From = "teachers t" +
               " LEFT JOIN course_teacher_rel ct ON t.id=ct.teacher_id ";
            string Conditions = " ct.course_id IN (" + String.Join(",", courseIds.ToArray()) + ")";

            DataTable res = DBConnection.Select(From, Columns, Conditions);

            ArrayList result = new ArrayList();
            foreach (DataRow r in res.Rows)
            {
                int lectures = 0, numericEx = 0, laboratoryEx = 0;
                string ColumnsQ = " c.detail";
                string FromQ = "courses c" +
                   " LEFT JOIN course_teacher_rel ct ON c.id=ct.course_id ";
                string ConditionsQ = " ct.teacher_id ='" + r.ItemArray[0] + "' AND ct.course_id IN (" + String.Join(",", courseIds.ToArray()) + ")";

                DataTable teacherCourses = DBConnection.Select(FromQ, ColumnsQ, ConditionsQ);
                if (teacherCourses.Rows != null && teacherCourses.Rows.Count > 0)
                {
                    foreach (DataRow course in teacherCourses.Rows)
                    {
                        switch (course.ItemArray[0].ToString().Trim())
                        {
                            case "lecture":
                                lectures++;
                                break;
                            case "numeric":
                                numericEx++;
                                break;
                            case "laboratory":
                                laboratoryEx++;
                                break;
                        }
                    }
                    result.Add(new CourseTeachers()
                    {
                        TeacherID = Int32.Parse(r.ItemArray[0].ToString()),
                        TeacherName = r.ItemArray[1].ToString(),
                        Lectures = lectures,
                        NumericExercises = numericEx,
                        LaboratoryExercises = laboratoryEx
                    });
                }
            }

            return result;
        }

        public ArrayList getDepartments(ArrayList courseIds)
        {
            string Columns = " DISTINCT(d.id) AS departmentId, d.name AS departmentName";
            string From = " departments d" +
               " LEFT JOIN course_department_rel cd ON d.id=cd.department_id ";
            string Conditions = " cd.course_id IN (" + String.Join(",", courseIds.ToArray()) + ")";

            DataTable res = DBConnection.Select(From, Columns, Conditions);

            ArrayList result = new ArrayList();
            foreach (DataRow r in res.Rows)
            {
                result.Add(new ComboDepartments()
                {
                    DepartmentID = int.Parse(r.ItemArray[0].ToString()),
                    DepartmentName = r.ItemArray[1].ToString()
                });
            }

            return result;
        }

        private void comboBoxDepartment_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("departments", comboBoxDepartment);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterCourse();
        }

        private void txtFilterByName_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterCourse();
        }

        private void comboboxSemesters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterCourse();
        }

        private void coursesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            editCourse_MouseUp(sender, e);
        }
    }
}