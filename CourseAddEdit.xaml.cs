using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;

namespace UniversityTimetabling
{
    /// <summary>
    /// Interaction logic for CourseAddEdit.xaml
    /// </summary>
    public partial class CourseAddEdit : Window
    {
		private Courses parentGrid;

		public CourseAddEdit(Courses coursesGrid)
        {
            InitializeComponent();
			this.parentGrid = coursesGrid;
        }

        private List<char> alphabet = new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private List<string> primes = new List<string>() { "`", "``", "```", "````", "`````", "``````" };

        private void ThreadStartingPoint()
        {
            LoadingPanel.LoadingPanel bar = new LoadingPanel.LoadingPanel();
            bar.Show();
            System.Windows.Threading.Dispatcher.Run();
        }

        private void saveCourse_MouseUp(object sender, EventArgs e)
        {           
            //Insert Course
            if (txtId.Text == "")
            {
				Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
				newWindowThread.SetApartmentState(ApartmentState.STA);
				newWindowThread.IsBackground = true;
				newWindowThread.Start();
				insertCourse(newWindowThread);
            }

            //Update Course
            else if (Convert.ToInt32(txtId.Text) > 0)
            {
				var result = MessageBox.Show("Update of this course will delete all assignments of its childs!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
				if (result == MessageBoxResult.OK)
				{
					Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
					newWindowThread.SetApartmentState(ApartmentState.STA);
					newWindowThread.IsBackground = true;
					newWindowThread.Start();
					updateCourse(newWindowThread);
				}
            }
        }

        private void close_MouseUp(object sender, EventArgs e)
        {
            Close();
        }

        private void insertCourse(Thread newWindowThread, bool hideMessage = false)
        {
			try
			{
				string code = null;
				for (int i = 1; i < 1000; i++)
				{
					code = "p" + i.ToString("000");
					DataView data = DBConnection.Select("courses", "'*'", " code = '" + code + "'").DefaultView;
					if (data.Count == 0) break;
				}

				validateFields(newWindowThread);
				int departmentRes;
				List<ComboDepartments> departments = courseDepartmentsGrid.Items.OfType<ComboDepartments>().ToList();
				if (departments.Count() == 0)
				{
					throw new Exception("No department selected!");
				}

				List<CourseTeachers> teachersGroups = courseTeacherRelationGrid.Items.OfType<CourseTeachers>().ToList();
				int nrGroups = int.Parse(comboBoxGroups.SelectedIndex.ToString()) >= 0 ? int.Parse(comboBoxGroups.SelectedValue.ToString()) : 0;
				int nrExerciseGroups = int.Parse(comboBoxNrExerciseGroups.SelectedIndex.ToString()) >= 0 ? int.Parse(comboBoxNrExerciseGroups.SelectedValue.ToString()) : 0;
				int nrLaboratoryGroups = int.Parse(comboBoxLabExercisesGroups.SelectedIndex.ToString()) >= 0 ? int.Parse(comboBoxLabExercisesGroups.SelectedValue.ToString()) : 0;

				string lectures = comboBoxNrLectures.SelectedIndex >= 0 ? comboBoxNrLectures.SelectedValue.ToString() : "0";
				string numLectures = comboBoxNrNumExercLectures.SelectedIndex >= 0 ? comboBoxNrNumExercLectures.SelectedValue.ToString() : "0";
				string labLectures = comboBoxNrLabExercLectures.SelectedIndex >= 0 ? comboBoxNrLabExercLectures.SelectedValue.ToString() : "0";
				string number_of_lectures = lectures + "+" + numLectures + "+" + labLectures;
				string ects = comboBoxEcts.SelectedValue.ToString();
				int min_days = int.Parse(comboBoxMinDays.SelectedValue.ToString());
				var type = comboBoxType.SelectedValue;
				var degree = comboBoxDegree.SelectedValue;
				var semester_id = comboBoxSemester.SelectedValue;
				var double_lectures = int.Parse(comboBoxDoubleLectures.SelectedValue.ToString()) == 1 ? true : false;

				string values = "'" + txtName.Text +
								"', '" + code.ToString() +
								"', '" + ects +
								"', '" + type +
								"', '" + degree +
								"', '" + semester_id +
								"', '" + number_of_lectures +
								"', '" + min_days +
								"', '" + double_lectures +
								"', '" + int.Parse(txtNrStudents.Text) +
								"', '" + int.Parse(comboBoxGroups.SelectedValue.ToString()) +
								 "', 'base', null, null, null, null";

				string columns = "name, code, ects, type, degree, semester_id, number_of_lectures, min_days, double_lectures, number_of_students, groups, detail, lecture_group, numeric_group, laboratory_group, base_parent_code";

				int result = DBConnection.Create("courses", columns, values);    //Returns last inserted id

				//Insert Groups of Course
				if (result > 0)
				{
					int gr = 0, ex = 0, lab = 0;
					int[] groupIds = new int[nrGroups];
					int[] numExercisesIds = new int[nrExerciseGroups];
					int[] labExercisesIds = new int[nrLaboratoryGroups];
					if (nrGroups > 0)
					{
						int studentsPerGroup = Convert.ToInt32(txtNrStudents.Text) / nrGroups;
						for (int i = 1; i <= nrGroups; i++)
						{
							int exercisePerGroup = nrExerciseGroups / nrGroups;
							int exDiff = nrExerciseGroups - (nrGroups * exercisePerGroup);
							string groupsCode = null;
							for (int j = 1; j < 1000; j++)
							{
								groupsCode = "c" + j.ToString("000");
								DataView data = DBConnection.Select("courses", "'*'", " code = '" + groupsCode + "'").DefaultView;
								if (data.Count == 0) break;
							}

							string groupValues = "'" + txtName.Text + ((nrGroups > 1) ? (" Gr. " + i.ToString()) : " ") +
											"', '" + groupsCode.ToString() +
											"', '" + ects +
											"', '" + type +
											"', '" + degree +
											"', '" + semester_id +
											"', '" + number_of_lectures +
											"', '" + min_days +
											"', '" + double_lectures +
											"', '" + studentsPerGroup +
											"', '" + result +
											"', 'lecture', 'Gr" + i.ToString() + "', null, null, '" + code.ToString() + "'";


							string grColumns = "name, code, ects, type, degree, semester_id, number_of_lectures, min_days, double_lectures, number_of_students, parent_id, detail, lecture_group, numeric_group, laboratory_group, base_parent_code";

							int grResult = DBConnection.Create("courses", grColumns, groupValues);
							groupIds[gr] = grResult;
							gr++;
							//Insert Exercises per Group
							if (exercisePerGroup > 0)
							{
								if (i == 1 && exDiff > 0) exercisePerGroup += exDiff;
								int studentsPerExercises = studentsPerGroup / exercisePerGroup;
								for (int a = 1; a <= exercisePerGroup; a++)
								{
									int laboratoryPerGroup = nrLaboratoryGroups / nrExerciseGroups;
									int labDiff = nrLaboratoryGroups - (nrExerciseGroups * laboratoryPerGroup);
									string exerciseCode = null;

									for (int j = 1; j < 1000; j++)
									{
										exerciseCode = groupsCode + j;
										DataView data = DBConnection.Select("courses", "'*'", " code = '" + exerciseCode + "'").DefaultView;
										if (data.Count == 0) break;
									}

									string exGroup = i.ToString() + alphabet[a - 1];
									string exerciseValues = "'" + txtName.Text + ((nrGroups > 1) ? (" Gr. ") : " ") + (nrGroups > 1 || exercisePerGroup > 1 ? (i.ToString() + exGroup.Replace(i.ToString(), "")) : "") +
													"', '" + exerciseCode.ToString() +
													"', '" + ects +
													"', '" + type +
													"', '" + degree +
													"', '" + semester_id +
													"', '" + number_of_lectures +
													"', '" + min_days +
													"', '" + double_lectures +
													"', '" + studentsPerExercises +
													"', '" + grResult +
													"', 'numeric', 'Gr" + i.ToString() + "', '" + exGroup + "', null, '" + code.ToString() + "'";


									int exResult = DBConnection.Create("courses", grColumns, exerciseValues);
									numExercisesIds[ex] = exResult;
									ex++;
									//Insert Laboratory Exercises
									if (laboratoryPerGroup > 0)
									{
										if (a == 1 & labDiff > 0) laboratoryPerGroup += labDiff;
										int studentsPerLaboratory = studentsPerExercises / laboratoryPerGroup;
										for (int b = 0; b < laboratoryPerGroup; b++)
										{
											string labCode = null;
											for (int j = 1; j < 1000; j++)
											{
												labCode = exerciseCode + j;
												DataView data = DBConnection.Select("courses", "'*'", " code = '" + labCode + "'").DefaultView;
												if (data.Count == 0) break;
											}

											string labValues = "'" + txtName.Text + ((nrGroups > 1) ? (" Gr. ") : " ") + (nrGroups > 1 || (exercisePerGroup > 1 && laboratoryPerGroup >= 1) ? (i.ToString() + exGroup.Replace(i.ToString(), "") + primes[b]) : "") +
															"', '" + labCode.ToString() +
															"', '" + ects +
															"', '" + type +
															"', '" + degree +
															"', '" + semester_id +
															"', '" + number_of_lectures +
															"', '" + min_days +
															"', '" + double_lectures +
															"', '" + studentsPerLaboratory +
															"', '" + exResult +
															 "', 'laboratory', 'Gr" + i.ToString() + "', '" + exGroup + "', '" + exGroup + primes[b] + "', '" + code.ToString() + "'";


											int labResult = DBConnection.Create("courses", grColumns, labValues);
											labExercisesIds[lab] = labResult;
											lab++;
										}

									}
								}
							}
							//Insert Laboratory if no exercise groups
							else if (nrLaboratoryGroups / nrGroups > 0)
							{
								int laboratoryPerGroup = nrLaboratoryGroups / nrGroups;
								int studentsPerLaboratory = studentsPerGroup / laboratoryPerGroup;
								int labDiff = nrLaboratoryGroups - (nrGroups * laboratoryPerGroup);
								if (i == 1 && labDiff > 0) laboratoryPerGroup += labDiff;
								for (int b = 1; b <= laboratoryPerGroup; b++)
								{
									string labCode = null;
									for (int j = 1; j < 1000; j++)
									{
										labCode = groupsCode + j;
										DataView data = DBConnection.Select("courses", "'*'", " code = '" + labCode + "'").DefaultView;
										if (data.Count == 0) break;
									}

									double des = Convert.ToDouble(b / 2.0);
									double devideRes = Math.Ceiling(Convert.ToDouble(b / 2.0));
									int remain = b % 2;
									string labName = i.ToString() + alphabet[Convert.ToInt32(devideRes) - 1] + primes[remain];

									string labValues = "'" + txtName.Text + ((nrGroups > 1) ? (" Gr. ") : " ") + (laboratoryPerGroup > 1 ? (labName) : "") +
													"', '" + labCode.ToString() +
													"', '" + ects +
													"', '" + type +
													"', '" + degree +
													"', '" + semester_id +
													"', '" + number_of_lectures +
													"', '" + min_days +
													"', '" + double_lectures +
													"', '" + studentsPerLaboratory +
													"', '" + grResult +
													"', 'laboratory', 'Gr" + i.ToString() + "', null, '" + labName + "', '" + code.ToString() + "'";


									int labResult = DBConnection.Create("courses", grColumns, labValues);
									labExercisesIds[lab] = labResult;
									lab++;
								}
							}
						}

						//Get teacher and their number of groups
						List<CourseTeachers> teachers = courseTeacherRelationGrid.Items.OfType<CourseTeachers>().ToList();
						int lecture = 0, exercise = 0, laboratory = 0;
						foreach (CourseTeachers row in teachers)
						{
							while (row.Lectures > 0)
							{
								if (lecture >= groupIds.Count())
								{
                                    break;
								}
								string vals = "'" + groupIds[lecture] + "', '" + row.TeacherID + "'";

								int res = DBConnection.Create("course_teacher_rel", "course_id, teacher_id", vals);

								//Insert course preferences
								insertCoursePreferences(groupIds[lecture], row.TeacherID);
								lecture++;
								row.Lectures--;
							}

							while (row.NumericExercises > 0)
							{
								if (exercise >= numExercisesIds.Count())
								{
                                    break;
								}
								string vals = "'" + numExercisesIds[exercise] + "', '" + row.TeacherID + "'";

								int res = DBConnection.Create("course_teacher_rel", "course_id, teacher_id", vals);

								//Innsert course preferences
								insertCoursePreferences(numExercisesIds[exercise], row.TeacherID);
								exercise++;
								row.NumericExercises--;
							}

							while (row.LaboratoryExercises > 0)
							{
								if (laboratory >= labExercisesIds.Count())
								{
                                    break;
								}
								string vals = "'" + labExercisesIds[laboratory] + "', '" + row.TeacherID + "'";

								int res = DBConnection.Create("course_teacher_rel", "course_id, teacher_id", vals);

								//Insert course preferences
								insertCoursePreferences(labExercisesIds[laboratory], row.TeacherID);
								laboratory++;
								row.LaboratoryExercises--;
							}
						}

						// Get all registered rooms
						DataTable rooms = DBConnection.Select("rooms", "id, name");
						List<CourseRooms> allRooms = new List<CourseRooms>();
						if (rooms.Rows.Count != 0)
						{
							foreach (DataRow room in rooms.Rows)
							{
								//First column is ID
								allRooms.Add(new CourseRooms() { RoomID = int.Parse(room.ItemArray[0].ToString()), RoomName = room.ItemArray[1].ToString() });
							}

						}

						//Get rooms and their number of groups
						int roomRes;
						List<CourseRooms> lectureRooms = courseRoomRelationLecturesGrid.Items.OfType<CourseRooms>().ToList();
						List<CourseRooms> numericExRooms = courseRoomRelationNumExGrid.Items.OfType<CourseRooms>().ToList();
						List<CourseRooms> laboratoryExRooms = courseRoomRelationLabExGrid.Items.OfType<CourseRooms>().ToList();

						foreach (CourseRooms row in allRooms)
						{
							if (lectureRooms.Count > 0 && !lectureRooms.Where(x => x.RoomID == row.RoomID).Any())
							{
								foreach (int m in groupIds)
									roomRes = DBConnection.Create("courses_rooms_rel", "course_id, room_id", "'" + m + "', '" + row.RoomID + "'");
							}

							if (numericExRooms.Count > 0 && !numericExRooms.Where(x => x.RoomID == row.RoomID).Any())
							{
								foreach (int n in numExercisesIds)
									roomRes = DBConnection.Create("courses_rooms_rel", "course_id, room_id", "'" + n + "', '" + row.RoomID + "'");
							}

							if (laboratoryExRooms.Count > 0 && !laboratoryExRooms.Where(x => x.RoomID == row.RoomID).Any())
							{
								foreach (int o in labExercisesIds)
									roomRes = DBConnection.Create("courses_rooms_rel", "course_id, room_id", "'" + o + "', '" + row.RoomID + "'");
							}
						}

						foreach (ComboDepartments row in departments)
						{
							departmentRes = DBConnection.Create("course_department_rel", "course_id, department_id", "'" + result + "', '" + row.DepartmentID + "'");

							if(groupIds.Length > 0)
								foreach (int m in groupIds)
									departmentRes = DBConnection.Create("course_department_rel", "course_id, department_id", "'" + m + "', '" + row.DepartmentID + "'");

							if (numExercisesIds.Length > 0)
								foreach (int n in numExercisesIds)
									departmentRes = DBConnection.Create("course_department_rel", "course_id, department_id", "'" + n + "', '" + row.DepartmentID + "'");

							if (labExercisesIds.Length > 0)
								foreach (int o in labExercisesIds)
									departmentRes = DBConnection.Create("course_department_rel", "course_id, department_id", "'" + o + "', '" + row.DepartmentID + "'");
						}
					}
				}
				newWindowThread.Abort();
				if (!hideMessage)
					MessageBox.Show("Successfully generated new Courses!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
				else
					MessageBox.Show("Courses were successfully updated!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
				Close();
				if (parentGrid.IsVisible)
					parentGrid.filterCourse();
			} catch (Exception e)
			{
				newWindowThread.Abort();
				MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
        }

		private void validateFields(Thread runningThread)
		{
			try
			{
				if (txtName.Text.Length == 0)
					throw new Exception("Course name is required!");
				if (int.Parse(comboBoxEcts.SelectedIndex.ToString()) < 0)
					throw new Exception("ECTS of a course can not be empty!");
				if (int.Parse(comboBoxType.SelectedIndex.ToString()) < 0)
					throw new Exception("Please select a course type!");
				if (int.Parse(comboBoxDegree.SelectedIndex.ToString()) < 0)
					throw new Exception("Please select course degree!");
				if (int.Parse(comboBoxSemester.SelectedIndex.ToString()) < 0)
					throw new Exception("Please select course semester!");
				if (int.Parse(comboBoxDoubleLectures.SelectedIndex.ToString()) < 0)
					throw new Exception("Please define double lecture value!");
				if (int.Parse(comboBoxMinDays.SelectedIndex.ToString()) < 0)
					throw new Exception("Minimum number of days must be greater than 0!");				
				if (int.Parse(comboBoxNrLectures.SelectedIndex.ToString()) < 0 && 
					int.Parse(comboBoxNrNumExercLectures.SelectedIndex.ToString()) < 0 && 
					int.Parse(comboBoxNrLabExercLectures.SelectedIndex.ToString()) < 0)
					throw new Exception("Number of lectures is required!");
                
                List<ComboDepartments> departments = courseDepartmentsGrid.Items.OfType<ComboDepartments>().ToList();
                if (departments.Count() == 0)
                {
                    throw new Exception("No department selected!");
                }
            }
			catch (Exception e)
			{
				runningThread.Abort();
				throw new Exception(e.Message);
			}
		}


		private void insertCoursePreferences(int courseId, int teacherId = 0)
        {
			if (teacherId > 0)
			{
				DataTable preferences = DBConnection.Select("teacher_periods_preferences", "day, period", "teacher_id=" + teacherId);
				if (preferences.Rows.Count != 0)
				{
					foreach (DataRow preference in preferences.Rows)
					{
						string values = "'" + courseId + "', '" + preference.ItemArray[0] + "', '" + preference.ItemArray[1] + "'";
						int result = DBConnection.Create("course_periods_preferences", "course_id, day, period", values);
					}

				}
			}
        }

        private void updateCourse(Thread thread)
        {
			try
			{
				validateFields(thread);

				(new Courses()).deleteCoursesCascade(Convert.ToInt32(txtId.Text), true);
				insertCourse(thread, true);

			} catch(Exception e)
			{
				thread.Abort();
				MessageBox.Show(e.Message, "Error", MessageBoxButton.OK);
			}
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NumberPlusValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9+]+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void comboBoxType_Initialized(object sender, EventArgs e)
        {
            comboBoxType.SelectedValuePath = "id";
            comboBoxType.DisplayMemberPath = "name";
            comboBoxType.Items.Add(new { id = "obl", name = "Obligatory" });
            comboBoxType.Items.Add(new { id = "zgj", name = "Elective" });
        }

        private void comboBoxDegree_Initialized(object sender, EventArgs e)
        {
            comboBoxDegree.SelectedValuePath = "id";
            comboBoxDegree.DisplayMemberPath = "name";
            comboBoxDegree.Items.Add(new { id = "bachelor", name = "Bachelor" });
            comboBoxDegree.Items.Add(new { id = "master", name = "Master" });
        }

        private void comboBoxSemester_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("semesters", comboBoxSemester);
        }

        private void comboBoxDepartment_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("departments", comboBoxDepartment);
        }

        private void comboBoxDoubleLectures_Initialized(object sender, EventArgs e)
        {
            comboBoxDoubleLectures.SelectedValuePath = "id";
            comboBoxDoubleLectures.DisplayMemberPath = "name";
            comboBoxDoubleLectures.Items.Add(new { id = 0, name = "False" });
            comboBoxDoubleLectures.Items.Add(new { id = 1, name = "True" });
        }

        private void comboBoxTeachers_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("teachers", comboBoxTeacher, "name,surname");
        }

        private void comboBoxLectureRooms_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("rooms", comboBoxLectureRooms);
        }

        private void comboBoxNumExRooms_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("rooms", comboBoxNumExRooms);
        }

        private void comboBoxLabExRooms_Initialized(object sender, EventArgs e)
        {
            DBConnection.FillCombo("rooms", comboBoxLabExRooms);
        }

        private void addLectureRoom_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (comboBoxLectureRooms.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a room!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            foreach (CourseRooms row in courseRoomRelationLecturesGrid.Items)
            {
                if (Int32.Parse(row.RoomID.ToString()) == Int32.Parse(comboBoxLectureRooms.SelectedValue.ToString()))
                {
                    MessageBox.Show("This room is already added!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            courseRoomRelationLecturesGrid.Items.Add(new CourseRooms() { RoomID = int.Parse(comboBoxLectureRooms.SelectedValue.ToString()), RoomName = comboBoxLectureRooms.Text });
			comboBoxLectureRooms.SelectedIndex = -1;
		}

        private void addNumericExRoom_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (comboBoxNumExRooms.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a room!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            foreach (CourseRooms row in courseRoomRelationNumExGrid.Items)
            {
                if (int.Parse(row.RoomID.ToString()) == int.Parse(comboBoxNumExRooms.SelectedValue.ToString()))
                {
                    MessageBox.Show("This room is already added!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            courseRoomRelationNumExGrid.Items.Add(new CourseRooms() { RoomID = int.Parse(comboBoxNumExRooms.SelectedValue.ToString()), RoomName = comboBoxNumExRooms.Text });
			comboBoxNumExRooms.SelectedIndex = -1;
		}

        private void addLaboratoryRoom_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (comboBoxLabExRooms.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a room!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            foreach (CourseRooms row in courseRoomRelationLabExGrid.Items)
            {
                if (int.Parse(row.RoomID.ToString()) == int.Parse(comboBoxLabExRooms.SelectedValue.ToString()))
                {
                    MessageBox.Show("This room is already added!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            courseRoomRelationLabExGrid.Items.Add(new CourseRooms() { RoomID = int.Parse(comboBoxLabExRooms.SelectedValue.ToString()), RoomName = comboBoxLabExRooms.Text });
			comboBoxLabExRooms.SelectedIndex = -1;
		}
        
        private void addTeacher_MouseUp(object sender, EventArgs e)
        {
            if(comboBoxTeacher.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a teacher!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            foreach (CourseTeachers row in courseTeacherRelationGrid.Items)
            {
                if (int.Parse(row.TeacherID.ToString()) == int.Parse(comboBoxTeacher.SelectedValue.ToString()))
                {
                    MessageBox.Show("This instructor is already added!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            courseTeacherRelationGrid.Items.Add(new CourseTeachers() { TeacherID = int.Parse(comboBoxTeacher.SelectedValue.ToString()),
                                                                       TeacherName = comboBoxTeacher.Text,
                                                                       Lectures = int.Parse(comboBoxLectureGroups.SelectedIndex.ToString()) >= 0 ? int.Parse(comboBoxLectureGroups.SelectedValue.ToString()) : int.Parse("0"),
                                                                       NumericExercises = int.Parse(comboBoxNrGroups.SelectedIndex.ToString()) >= 0 ? int.Parse(comboBoxNrGroups.SelectedValue.ToString()) : int.Parse("0"),
                                                                       LaboratoryExercises = int.Parse(comboBoxLabGroups.SelectedIndex.ToString()) >= 0 ? int.Parse(comboBoxLabGroups.SelectedValue.ToString()) : int.Parse("0")
            });

			comboBoxTeacher.SelectedIndex = -1;
			comboBoxLectureGroups.SelectedIndex = -1;
			comboBoxNrGroups.SelectedIndex = -1;
			comboBoxLabGroups.SelectedIndex = -1;

		}

        private void deleteTeacherRelation_MouseUp(object sender, EventArgs e)
        {
            CourseTeachers row = (CourseTeachers)courseTeacherRelationGrid.SelectedItem;
            if (row != null)
                courseTeacherRelationGrid.Items.Remove(row);
            else
            {
                MessageBox.Show("Please select a record to delete!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void deleteRoomRelation_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CourseRooms lectureRow = (CourseRooms)courseRoomRelationLecturesGrid.SelectedItem;
            CourseRooms numExRow = (CourseRooms)courseRoomRelationNumExGrid.SelectedItem;
            CourseRooms labExRow = (CourseRooms)courseRoomRelationLabExGrid.SelectedItem;
            if (lectureRow != null)
                courseRoomRelationLecturesGrid.Items.Remove(lectureRow);
            else if (numExRow != null)
                courseRoomRelationNumExGrid.Items.Remove(numExRow);
            else if (labExRow != null)
                courseRoomRelationLabExGrid.Items.Remove(labExRow);
            else
            {
                MessageBox.Show("Please select a room to delete!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void addDepartment_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (comboBoxDepartment.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a department!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            foreach (ComboDepartments row in courseDepartmentsGrid.Items)
            {
                if (int.Parse(row.DepartmentID.ToString()) == int.Parse(comboBoxDepartment.SelectedValue.ToString()))
                {
                    MessageBox.Show("This department is already added!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    comboBoxDepartment.SelectedIndex = -1;
                    return;
                }
            }
            courseDepartmentsGrid.Items.Add(new ComboDepartments()
            {
                DepartmentID = int.Parse(comboBoxDepartment.SelectedValue.ToString()),
                DepartmentName = comboBoxDepartment.Text
            });

            comboBoxDepartment.SelectedIndex = -1;
        }

        private void deleteDepartment_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ComboDepartments departmentRow = (ComboDepartments)courseDepartmentsGrid.SelectedItem;

            if (departmentRow != null)
                courseDepartmentsGrid.Items.Remove(departmentRow);
            else
            {
                MessageBox.Show("Please select a department to delete!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
    }
        private void updateCourseNames()
        {
            DataTable data = DBConnection.RAW_Select("Select c.name, p.groups, c.lecture_group, c.numeric_group, c.laboratory_group, c.id from courses c" +
                                " JOIN courses p on c.base_parent_code = p.code Where c.base_parent_code IS not null");
            if (data.Rows.Count != 0)
            {
                foreach (DataRow row in data.Rows)
                {
                    string name = row.ItemArray[0].ToString().Split(new string[] { "_Gr" }, StringSplitOptions.None)[0];
                    string grField = "";
                    if (Convert.ToInt32(row.ItemArray[1].ToString()) > 1)
                    {
                        grField += " Gr. ";
                    }
                    else grField += " ";

                    string lab = row.ItemArray[4].ToString();
                    string num = row.ItemArray[3].ToString();
                    string lect = row.ItemArray[2].ToString();
                    if (lab != null && lab != "")
                    {
                        name += grField + row.ItemArray[4].ToString();
                    }
                    else if (num != null && num != "")
                    {
                        name += grField + row.ItemArray[3].ToString();
                    }
                    else if (lect != null && lect != "" && grField != " ")
                    {
                        string group = grField + row.ItemArray[2].ToString().Split('r')[1];
                        name += group;
                    }

                    DBConnection.Update(" courses ", " name='" + name + "' ", Convert.ToInt32(row.ItemArray[5].ToString()));
                }
            }
        }

		private void comboBoxEcts_Initialized(object sender, EventArgs e)
		{
			fillCombo(comboBoxEcts, 2, 12, 0.5);
		}

		private void fillCombo(ComboBox comboBox, double start, double end, double increment)
		{
			comboBox.SelectedValuePath = "id";
			comboBox.DisplayMemberPath = "name";
			for(double i = start; i <= end; i += increment)
				comboBox.Items.Add(new { id = i, name = i });
		}

		private void comboBoxMinDays_Initialized(object sender, EventArgs e)
		{
			fillCombo(comboBoxMinDays, 1, 6, 1);
		}

		private void comboBoxNrLectures_Initialized(object sender, EventArgs e)
		{
			fillCombo(comboBoxNrLectures, 0, 10, 1);
		}

		private void comboBoxNrNumExercLectures_Initialized(object sender, EventArgs e)
		{
			fillCombo(comboBoxNrNumExercLectures, 0, 10, 1);
		}

		private void comboBoxNrLabExercLectures_Initialized(object sender, EventArgs e)
		{
			fillCombo(comboBoxNrLabExercLectures, 0, 10, 1);
		}

		private void comboBoxGroups_Initialized(object sender, EventArgs e)
		{
			fillCombo(comboBoxGroups, 0, 30, 1);
		}

		private void comboBoxNrExerciseGroups_Initialized(object sender, EventArgs e)
		{
			fillCombo(comboBoxNrExerciseGroups, 0, 30, 1);
		}

		private void comboBoxLabExercisesGroups_Initialized(object sender, EventArgs e)
		{
			fillCombo(comboBoxLabExercisesGroups, 0, 30, 1);
		}

		private void comboBoxLabGroups_Initialized(object sender, EventArgs e)
		{
			fillCombo(comboBoxLabGroups, 0, 30, 1);
		}

		private void comboBoxNrGroups_Initialized(object sender, EventArgs e)
		{
			fillCombo(comboBoxNrGroups, 0, 30, 1);
		}

		private void comboBoxLectureGroups_Initialized(object sender, EventArgs e)
		{
			fillCombo(comboBoxLectureGroups, 0, 30, 1);
		}

		private void AddTeacher_KeyUp(object sender, KeyEventArgs ev)
		{
			if (ev.Key == Key.Return)
			{
				addTeacher_MouseUp(sender, ev);
				Keyboard.ClearFocus();
			}
		}

		private void DeleteCourse_KeyUp(object sender, KeyEventArgs ev)
		{
			if (ev.Key == Key.Return)
			{
				deleteTeacherRelation_MouseUp(sender, ev);
				Keyboard.ClearFocus();
			}
		}

		private void SaveCourse_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				saveCourse_MouseUp(sender, e);
				Keyboard.ClearFocus();
			}
		}

		private void Close_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				close_MouseUp(sender, e);
				Keyboard.ClearFocus();
			}
		}
	}

	public class CourseTeachers
    {
        public int TeacherID { get; set; }
        public string TeacherName { get; set; }
        public int Lectures { get; set; }
        public int NumericExercises { get; set; }
        public int LaboratoryExercises { get; set; }
    }

    public class CourseRooms
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }
    }

    public class ComboDepartments
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }
}