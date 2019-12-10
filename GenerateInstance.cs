using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;

namespace UniversityTimetabling
{
    class GenerateInstance
    {
        private int semesterModule;

        public void main(string path)
        {
              semesterModule = getSemesterModule();
             XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
             {
                 Indent = true,
                 IndentChars = "\t"
             };

             string _basePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
             string _instancePath = "Input\\" + path + ".xml";
             XmlWriter xmlWriter = XmlWriter.Create(_basePath + "\\" + _instancePath, xmlWriterSettings);
             xmlWriter.WriteStartDocument(false);
             xmlWriter.WriteDocType("instance", null, "http://tabu.diegm.uniud.it/ctt/cb_ctt.dtd", null);

             xmlWriter.WriteStartElement("instance");
             xmlWriter.WriteAttributeString("name", path);

             //Descriptor
             xmlWriter.WriteStartElement("descriptor");
             GeneralInfo(xmlWriter);
             xmlWriter.WriteEndElement();

             //Courses
             xmlWriter.WriteStartElement("courses");
             GenerateCourses(xmlWriter);
             xmlWriter.WriteEndElement();

             //Rooms
             xmlWriter.WriteStartElement("rooms");
             GenerateRooms(xmlWriter);
             xmlWriter.WriteEndElement();

             //Curricula
             xmlWriter.WriteStartElement("curricula");
             GenerateCurricula(xmlWriter);
             xmlWriter.WriteEndElement();

             //Constraints
             xmlWriter.WriteStartElement("constraints");
             GeneratePeriodConstraints(xmlWriter);
             GenerateRoomConstraints(xmlWriter);
             xmlWriter.WriteEndElement();

             //End of Document
             xmlWriter.WriteEndDocument();       
             xmlWriter.Close();
        }

        private int getSemesterModule()
        {
            Dictionary<string, string> first = new Dictionary<string, string>();
            ArrayList data = DBConnection.Get("currentSemester", 1);
            first = (Dictionary<string, string>)data[0];

            string index;
            first.TryGetValue("module", out index);

            return Convert.ToInt32(index);
        }

        private void GeneralInfo(XmlWriter writer)
        {
            Dictionary<string, string> first = new Dictionary<string, string>();

            ArrayList data = DBConnection.Get("general_info", 1);
            first = (Dictionary<string, string>)data[0];

            string nrOfDays, periodsPerDay, minNrOfLectures, maxNrOfLectures;
            first.TryGetValue("number_of_days", out nrOfDays);
            first.TryGetValue("periods_per_day", out periodsPerDay);
            first.TryGetValue("min_daily_lectures", out minNrOfLectures);
            first.TryGetValue("max_daily_lectures", out maxNrOfLectures);

            writer.WriteStartElement("days");
            writer.WriteAttributeString("value", nrOfDays);
            writer.WriteEndElement();
            writer.WriteStartElement("periods_per_day");
            writer.WriteAttributeString("value", periodsPerDay);
            writer.WriteEndElement();
            writer.WriteStartElement("daily_lectures");
            writer.WriteAttributeString("min", minNrOfLectures);
            writer.WriteAttributeString("max", maxNrOfLectures);
            writer.WriteEndElement();
        }

        private void GenerateCourses(XmlWriter writer)
        {
            DataTable data = DBConnection.Select(" courses ", " id ", " parent_id IS NOT NULL AND (semester_id % 2) = " + semesterModule);
       
            ArrayList baseCourseGroupIds = new ArrayList();
            foreach (DataRow lectureRow in data.Rows)
                baseCourseGroupIds.Add(lectureRow.ItemArray[0]);
           
            Dictionary<string, string> first = new Dictionary<string, string>();
            string courseCode, teacher = "", numberOfLectures, detail, minDays, students, doubleLectures;
            foreach (int cId in baseCourseGroupIds)
            {
                ArrayList rec = DBConnection.Get("courses", cId);
                first = (Dictionary<string, string>)rec[0];
                first.TryGetValue("code", out courseCode);
                first.TryGetValue("number_of_lectures", out numberOfLectures);
                first.TryGetValue("min_days", out minDays);
                first.TryGetValue("number_of_students", out students);
                first.TryGetValue("double_lectures", out doubleLectures);
                first.TryGetValue("detail", out detail);

                string table = " course_teacher_rel ctr" +
                                " JOIN teachers t ON ctr.teacher_id=t.id ";
                DataTable teacherRec = DBConnection.Select(table, " t.code AS teacher_code", " ctr.course_id=" + cId);
                foreach (DataRow teacherRow in teacherRec.Rows)
                {
                    teacher = teacherRow.ItemArray[0].ToString();
                }

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

                //for every course in courses table
                writer.WriteStartElement("course");
                writer.WriteAttributeString("id", courseCode); 
                writer.WriteAttributeString("teacher", teacher);
                writer.WriteAttributeString("lectures", numberOfLectures); 
                writer.WriteAttributeString("min_days", minDays); 
                writer.WriteAttributeString("students", students); 
                writer.WriteAttributeString("double_lectures", doubleLectures); 
                writer.WriteEndElement();
            }
        }

        private void GenerateRooms(XmlWriter writer)
        {
            DataTable data = DBConnection.Select("rooms", " id ");

            ArrayList roomIds = new ArrayList();
            foreach (DataRow row in data.Rows)
                roomIds.Add(row.ItemArray[0]);

            Dictionary<string, string> first = new Dictionary<string, string>();
            string roomCode, buildingId, size;
            foreach (int rId in roomIds)
            {
                ArrayList rec = DBConnection.Get("rooms", rId);
                first = (Dictionary<string, string>)rec[0];
                first.TryGetValue("code", out roomCode);
                first.TryGetValue("building_id", out buildingId);
                first.TryGetValue("size", out size);

                //for every room in rooms table
                writer.WriteStartElement("room");
                writer.WriteAttributeString("id", roomCode);
                writer.WriteAttributeString("size", size);
                writer.WriteAttributeString("building", (Convert.ToInt32(buildingId) - 1 ).ToString());
                writer.WriteEndElement();
            }
        }

        private void GeneratePeriodConstraints(XmlWriter writer)
        {
            DataTable courseData = DBConnection.Select("course_periods_preferences cp JOIN courses c ON cp.course_id = c.id ", " Distinct cp.course_id ", " (c.semester_id % 2) =" + semesterModule + " AND c.parent_id IS NOT NULL ");

            ArrayList courseIds = new ArrayList();
            foreach (DataRow row in courseData.Rows)
                courseIds.Add(row.ItemArray[0]);

            foreach (int cId in courseIds)
            {
                Dictionary<string, string> course = (Dictionary<string, string>)DBConnection.Get("courses", cId)[0];
                string courseCode;
                course.TryGetValue("code", out courseCode);
                writer.WriteStartElement("constraint");
                writer.WriteAttributeString("type", "period");
                writer.WriteAttributeString("course", courseCode);
            

                DataTable data = DBConnection.Select("course_periods_preferences", " id ", " course_id= " + cId);

                ArrayList recIds = new ArrayList();
                foreach (DataRow row in data.Rows)
                    recIds.Add(row.ItemArray[0]);

                Dictionary<string, string> first = new Dictionary<string, string>();
                string timeslotDay, timeslotPeriod;
                foreach (int pId in recIds)
                {
                    ArrayList preferenceRec = DBConnection.Get("course_periods_preferences", pId);

                    first = (Dictionary<string, string>)preferenceRec[0];
                    first.TryGetValue("day", out timeslotDay);
                    first.TryGetValue("period", out timeslotPeriod);

                    //for every preference in rooms table
                    writer.WriteStartElement("timeslot");
                    writer.WriteAttributeString("day", (Convert.ToInt32(timeslotDay) - 1).ToString());
                    writer.WriteAttributeString("period", (Convert.ToInt32(timeslotPeriod) - 1).ToString());
                    writer.WriteEndElement();

                }
                writer.WriteEndElement();//period constraint
            }
        }

        private void GenerateRoomConstraints(XmlWriter writer)
        {
            DataTable courseData = DBConnection.Select("courses_rooms_rel crr JOIN courses c ON crr.course_id = c.id ", " Distinct crr.course_id ", " (c.semester_id % 2) =" + semesterModule + " AND c.parent_id IS NOT NULL");

            ArrayList courseIds = new ArrayList();
            foreach (DataRow row in courseData.Rows)
                courseIds.Add(row.ItemArray[0]);

            foreach (int cId in courseIds)
            {
                Dictionary<string, string> course = (Dictionary<string, string>)DBConnection.Get("courses", cId)[0];
                string courseCode;
                course.TryGetValue("code", out courseCode);
                writer.WriteStartElement("constraint");
                writer.WriteAttributeString("type", "room");
                writer.WriteAttributeString("course", courseCode);


                DataTable data = DBConnection.Select("courses_rooms_rel", " room_id ", " course_id= " + cId);

                ArrayList recIds = new ArrayList();
                foreach (DataRow row in data.Rows)
                    recIds.Add(row.ItemArray[0]);

                Dictionary<string, string> first = new Dictionary<string, string>();
                string roomRef;
                foreach (int rId in recIds)
                {
                    ArrayList roomRec = DBConnection.Get("rooms", rId);

                    first = (Dictionary<string, string>)roomRec[0];
                    first.TryGetValue("code", out roomRef);

                    //for every room in courses_rooms_rel table
                    writer.WriteStartElement("room");
                    writer.WriteAttributeString("ref", roomRef);
                    writer.WriteEndElement();

                }
                writer.WriteEndElement();
            }
        }

        private void GenerateCurricula(XmlWriter writer)
        {
            int i = 1;
            string curriculaCode;
            string actualGroup;
            ArrayList semesterIds = getSemesters(" (id % 2) =" + semesterModule);
            ArrayList departmentIds = getDepartments();
            
            foreach (int semesterId in semesterIds)
            {
                foreach (int departmentId in departmentIds)
                {
                    string from = "courses c" +
                                    " LEFT JOIN course_department_rel cdr ON c.id = cdr.course_id ";
                    string conditions = " c.parent_id IS NOT NULL AND c.semester_id= " + semesterId + " AND cdr.department_id= " + departmentId;
                    //get Distinct groups of all lectures
                    DataTable lectureGroups = DBConnection.Select(from, " DISTINCT c.lecture_group ", conditions);
                    if (lectureGroups != null)
                    {
                        foreach (DataRow group in lectureGroups.Rows)
                        {
                            actualGroup = group.ItemArray[0].ToString();

                            conditions = " c.parent_id IS NOT NULL AND c.semester_id= " + semesterId + " AND cdr.department_id= " + departmentId +" AND lecture_group='" + actualGroup + "' ";
                            string columns = " TOP 1 count(c.id), c.base_parent_code ";
                            string groupBy = " c.base_parent_code ";
                            string orderBy = " count(c.id) DESC ";

                            DataTable bachelorCoursesData = DBConnection.Select(from, columns, conditions + " AND c.degree = 'bachelor' AND c.detail = 'laboratory' ", groupBy, orderBy);

                            DataTable masterCoursesData = DBConnection.Select(from, columns, conditions + " AND c.degree = 'master' AND c.detail = 'laboratory' ", groupBy, orderBy);

                            if (bachelorCoursesData != null && bachelorCoursesData.Rows.Count > 0)
                            {
                                string baseParentCode = null;
                                foreach (DataRow dr in bachelorCoursesData.Rows)
                                {
                                    baseParentCode = dr.ItemArray[1].ToString();
                                }

                                DataTable laboratorys = DBConnection.Select(" courses ", " id, numeric_group, laboratory_group ", "  lecture_group='" + actualGroup + "' AND base_parent_code = '" + baseParentCode + "' AND detail = 'laboratory' ");

                                string numericVal, laboratoryVal;
                                foreach (DataRow row in laboratorys.Rows)
                                 {
                                    numericVal = row.ItemArray[1].ToString().Length > 0 ? row.ItemArray[1].ToString() : row.ItemArray[2].ToString().Replace("`", "");
                                    laboratoryVal = row.ItemArray[2].ToString();

                                    string curriculaCols = " c.id, c.code ";
                                    string curriculaConditions = " c.parent_id IS NOT null AND c.semester_id=" + semesterId + " AND cdr.department_id=" + departmentId + " AND c.lecture_group='" + actualGroup +
                                        "' AND c.id IN (SELECT c1.id FROM courses c1 WHERE (c1.laboratory_group='" + laboratoryVal + "' OR c1.laboratory_group IS NULL) AND(c1.numeric_group='" + numericVal +
                                        "' OR c1.numeric_group IS NULL) AND c1.lecture_group = c.lecture_group) ";

                                    DataTable curricula = DBConnection.Select(from, curriculaCols, curriculaConditions);
                                    if (curricula != null && curricula.Rows.Count > 0)
                                    {
                                        curriculaCode = "q" + i.ToString("000");
                                        i++;
                                        writer.WriteStartElement("curriculum");
                                        writer.WriteAttributeString("id", curriculaCode);
                                        foreach (DataRow c in curricula.Rows)
                                        {
                                            writer.WriteStartElement("course");
                                            writer.WriteAttributeString("ref", c.ItemArray[1].ToString());
                                            writer.WriteEndElement();
                                        }
                                        writer.WriteEndElement();
                                    }
                                }
                            }
                            else
                            {
                                DataTable bachelorNumericCoursesData = DBConnection.Select(from, columns, conditions + " AND c.degree = 'bachelor' AND c.detail = 'numeric' ", groupBy, orderBy);
                                if (bachelorNumericCoursesData != null && bachelorNumericCoursesData.Rows.Count > 0)
                                {
                                    string baseCode = null;
                                    foreach (DataRow dr in bachelorNumericCoursesData.Rows)
                                    {
                                        baseCode = dr.ItemArray[1].ToString();
                                    }

                                    DataTable numerics = DBConnection.Select(" courses ", " id, numeric_group ", " lecture_group='" + actualGroup + "' AND base_parent_code = '" + baseCode + "' AND detail = 'numeric' ");

                                    string numericVal;
                                    foreach (DataRow row in numerics.Rows)
                                    {
                                        numericVal = row.ItemArray[1].ToString();
                              
                                        string curriculaCols = " c.id, c.code ";
                                        string curriculaConditions = " c.parent_id IS NOT null AND c.semester_id=" + semesterId + " AND cdr.department_id=" + departmentId + " AND c.lecture_group='" + actualGroup +
                                            "' AND c.id IN (SELECT c1.id FROM courses c1 WHERE (c1.numeric_group='" + numericVal + "' OR c1.numeric_group IS NULL) AND c1.lecture_group = c.lecture_group) ";

                                        DataTable curricula = DBConnection.Select(from, curriculaCols, curriculaConditions);
                                        if (curricula != null && curricula.Rows.Count > 0)
                                        {
                                            curriculaCode = "q" + i.ToString("000");
                                            i++;
                                            writer.WriteStartElement("curriculum");
                                            writer.WriteAttributeString("id", curriculaCode);
                                            foreach (DataRow c in curricula.Rows)
                                            {
                                                writer.WriteStartElement("course");
                                                writer.WriteAttributeString("ref", c.ItemArray[1].ToString());
                                                writer.WriteEndElement();
                                            }
                                            writer.WriteEndElement();
                                        }
                                    }
                                }
                                else
                                {
                                    string curriculaCols = " c.id, c.code ";
                                    string curriculaConditions = " c.parent_id IS NOT null AND c.semester_id=" + semesterId + " AND cdr.department_id=" + departmentId + " AND c.lecture_group='" + actualGroup + "' AND c.detail = 'lecture' AND c.degree = 'bachelor' ";

                                    DataTable curricula = DBConnection.Select(from, curriculaCols, curriculaConditions);
                                    if (curricula != null && curricula.Rows.Count > 0)
                                    {
                                        curriculaCode = "q" + i.ToString("000");
                                        i++;
                                        writer.WriteStartElement("curriculum");
                                        writer.WriteAttributeString("id", curriculaCode);
                                        foreach (DataRow c in curricula.Rows)
                                        {
                                            writer.WriteStartElement("course");
                                            writer.WriteAttributeString("ref", c.ItemArray[1].ToString());
                                            writer.WriteEndElement();
                                        }
                                        writer.WriteEndElement();
                                    }
                                }
                            }

                            if (masterCoursesData != null && masterCoursesData.Rows.Count > 0)
                            {
                                string baseParentCodeM = null;
                                foreach (DataRow dr in masterCoursesData.Rows)
                                {
                                    baseParentCodeM = dr.ItemArray[1].ToString();
                                }

                                DataTable laboratorys = DBConnection.Select(" courses ", " id, numeric_group, laboratory_group ", "  lecture_group='" + actualGroup + "' AND base_parent_code = '" + baseParentCodeM + "' AND detail = 'laboratory' ");

                                string numericValM, laboratoryValM;
                                foreach (DataRow row in laboratorys.Rows)
                                {
                                    numericValM = row.ItemArray[1].ToString().Length > 0 ? row.ItemArray[1].ToString() : row.ItemArray[2].ToString().Replace("`", "");
									laboratoryValM = row.ItemArray[2].ToString();

                                    string curriculaColsM = " c.id, c.code ";
                                    string curriculaConditionsM = " c.parent_id IS NOT null AND c.semester_id=" + semesterId + " AND cdr.department_id=" + departmentId + " AND c.lecture_group='" + actualGroup +
                                        "' AND c.id IN (SELECT c1.id FROM courses c1 WHERE (c1.laboratory_group='" + laboratoryValM + "' OR c1.laboratory_group IS NULL) AND(c1.numeric_group='" + numericValM +
                                        "' OR c1.numeric_group IS NULL) AND c1.lecture_group = c.lecture_group) ";

                                    DataTable curricula = DBConnection.Select(from, curriculaColsM, curriculaConditionsM);
                                    if (curricula != null && curricula.Rows.Count > 0)
                                    {
                                        curriculaCode = "q" + i.ToString("000");
                                        i++;
                                        writer.WriteStartElement("curriculum");
                                        writer.WriteAttributeString("id", curriculaCode);
                                        foreach (DataRow c in curricula.Rows)
                                        {
                                            writer.WriteStartElement("course");
                                            writer.WriteAttributeString("ref", c.ItemArray[1].ToString());
                                            writer.WriteEndElement();
                                        }
                                        writer.WriteEndElement();
                                    }
                                }
                            }
                            else
                            {
                                DataTable masterNumericCoursesData = DBConnection.Select(from, columns, conditions + " AND c.degree = 'master' AND c.detail = 'numeric' ", groupBy, orderBy);
                                if (masterNumericCoursesData != null && masterNumericCoursesData.Rows.Count > 0)
                                {
                                    string baseCodeM = null;
                                    foreach (DataRow dr in masterNumericCoursesData.Rows)
                                    {
                                        baseCodeM = dr.ItemArray[1].ToString();
                                    }

                                    DataTable numerics = DBConnection.Select(" courses ", " id, numeric_group ", "  lecture_group='" + actualGroup + "' AND base_parent_code = '" + baseCodeM + "' AND detail = 'numeric' ");

                                    string numericValM;
                                    foreach (DataRow row in numerics.Rows)
                                    {
                                        numericValM = row.ItemArray[1].ToString();

                                        string curriculaColsM = " c.id, c.code ";
                                        string curriculaConditionsM = " c.parent_id IS NOT null AND c.semester_id=" + semesterId + " AND cdr.department_id=" + departmentId + " AND c.lecture_group='" + actualGroup +
                                            "' AND c.id IN (SELECT c1.id FROM courses c1 WHERE (c1.numeric_group='" + numericValM + "' OR c1.numeric_group IS NULL) AND c1.lecture_group = c.lecture_group) ";

                                        DataTable curricula = DBConnection.Select(from, curriculaColsM, curriculaConditionsM);
                                        if (curricula != null && curricula.Rows.Count > 0)
                                        {
                                            curriculaCode = "q" + i.ToString("000");
                                            i++;
                                            writer.WriteStartElement("curriculum");
                                            writer.WriteAttributeString("id", curriculaCode);
                                            foreach (DataRow c in curricula.Rows)
                                            {
                                                writer.WriteStartElement("course");
                                                writer.WriteAttributeString("ref", c.ItemArray[1].ToString());
                                                writer.WriteEndElement();
                                            }
                                            writer.WriteEndElement();
                                        }
                                    }
                                }
                                else
                                {
                                    string curriculaCols = " c.id, c.code ";
                                    string curriculaConditions = " c.parent_id IS NOT null AND c.semester_id=" + semesterId + " AND cdr.department_id=" + departmentId + " AND c.lecture_group='" + actualGroup + "' AND c.detail = 'lecture' AND c.degree = 'master' ";

                                    DataTable curricula = DBConnection.Select(from, curriculaCols, curriculaConditions);
                                    if (curricula != null && curricula.Rows.Count > 0)
                                    {
                                        curriculaCode = "q" + i.ToString("000");
                                        i++;
                                        writer.WriteStartElement("curriculum");
                                        writer.WriteAttributeString("id", curriculaCode);
                                        foreach (DataRow c in curricula.Rows)
                                        {
                                            writer.WriteStartElement("course");
                                            writer.WriteAttributeString("ref", c.ItemArray[1].ToString());
                                            writer.WriteEndElement();
                                        }
                                        writer.WriteEndElement();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static ArrayList getDepartments(int id = 0)
        {
            string conditions = null;
            if(id > 0)
            {
                conditions = " id=" + id;
            }
            DataTable data = DBConnection.Select(" departments ", " id ", conditions);
            ArrayList departmentIds = new ArrayList();

            foreach (DataRow departmentRow in data.Rows)
                departmentIds.Add(int.Parse(departmentRow.ItemArray[0].ToString()));

            return departmentIds;
        }

        public static ArrayList getSemesters(string conditions = null)
        {
            DataTable data = DBConnection.Select(" semesters ", " id ", conditions);
            ArrayList semesterIds = new ArrayList();

            foreach (DataRow semesterRow in data.Rows)
                semesterIds.Add(semesterRow.ItemArray[0]);
        
            return semesterIds;
        }

        /*  private void preferences()
          {
              DataTable data = DBConnection.Select(" courses c " +
                                      " LEFT JOIN course_department_rel cdr ON c.id = cdr.course_id ", " c.id ", "c.detail='laboratory'");//AND c.detail='lecture'
              ArrayList courseIds = new ArrayList();

              foreach (DataRow courseRow in data.Rows)
                  courseIds.Add(courseRow.ItemArray[0]);

              ArrayList days = new ArrayList() {1,2,3,4,5,6};
              ArrayList periods = new ArrayList() {1,2,3 };
              int res;
              foreach (int id in courseIds)
              {
                  foreach(int day in days)
                  {
                      foreach(int period in periods)
                      {
                          DataView dt = DBConnection.Select("course_periods_preferences", "'*'", " course_id = " + id + " AND day=" + day + " AND period=" + period ).DefaultView;
                          if (dt.Count == 0)
                              res = DBConnection.Create("course_periods_preferences", " course_id, day, period", "'" + id + "','" + day + "','" + period + "'");
                      }
                  }
              }
          }*/

        private void roomPreferences(string code, ArrayList lecRooms, ArrayList numRooms, ArrayList labRooms)
        {
            //Lecture
            DataTable lecData = DBConnection.Select(" courses ", " id ", " base_parent_code='" + code + "' AND detail='lecture'");//AND c.detail='lecture'
            ArrayList lecCourseIds = new ArrayList();

            foreach (DataRow courseRow in lecData.Rows)
                lecCourseIds.Add(courseRow.ItemArray[0]);

            int res;
            foreach (int id in lecCourseIds)
            {
                foreach (int lecRoom in lecRooms)
                {
                    DataView dt = DBConnection.Select("courses_rooms_rel", "'*'", " course_id = " + id + " AND room_id=" + lecRoom).DefaultView;
                    if (dt.Count == 0)
                        res = DBConnection.Create("courses_rooms_rel", " course_id, room_id", "'" + id + "','" + lecRoom + "'");
                }
            }

            //Numeric
            if (numRooms.Count > 0)
            {
                DataTable numData = DBConnection.Select(" courses ", " id ", " base_parent_code='" + code + "' AND detail='numeric'");//AND c.detail='lecture'
                ArrayList numCourseIds = new ArrayList();

                foreach (DataRow courseRow in numData.Rows)
                    numCourseIds.Add(courseRow.ItemArray[0]);

                int res1;
                foreach (int id in numCourseIds)
                {
                    foreach (int numRoom in numRooms)
                    {
                        DataView dt = DBConnection.Select("courses_rooms_rel", "'*'", " course_id = " + id + " AND room_id=" + numRoom).DefaultView;
                        if (dt.Count == 0)
                            res1 = DBConnection.Create("courses_rooms_rel", " course_id, room_id", "'" + id + "','" + numRoom + "'");
                    }
                }
            }

            //Laboratory
            if (labRooms.Count > 0)
            {
                DataTable labData = DBConnection.Select(" courses ", " id ", " base_parent_code='" + code + "' AND detail='laboratory'");//AND c.detail='lecture'
                ArrayList labCourseIds = new ArrayList();

                foreach (DataRow courseRow in labData.Rows)
                    labCourseIds.Add(courseRow.ItemArray[0]);

                int res2;
                foreach (int id in labCourseIds)
                {
                    foreach (int labRoom in labRooms)
                    {
                        DataView dt = DBConnection.Select("courses_rooms_rel", "'*'", " course_id = " + id + " AND room_id=" + labRoom).DefaultView;
                        if (dt.Count == 0)
                            res2 = DBConnection.Create("courses_rooms_rel", " course_id, room_id", "'" + id + "','" + labRoom + "'");
                    }
                }
            }
        }

     /*   private void courseSynchronization()
        {
            DataTable data = DBConnection.Select(" courses c ", " c.id, c.name, c.lecture_group, c.numeric_group, c.laboratory_group ", "c.detail IN ('laboratory')");//AND c.detail='lecture'
           // ArrayList courseIds = new ArrayList();

            foreach (DataRow courseRow in data.Rows)
            {

                string lName = courseRow.ItemArray[2].ToString();
                string nName = courseRow.ItemArray[3].ToString();//check if is null
                string labName = courseRow.ItemArray[4].ToString();//check if is null
                int id = int.Parse(courseRow.ItemArray[0].ToString());

                string newName = courseRow.ItemArray[1].ToString();
                string newNum = nName;
                string lab_name = labName; 
                var gr = int.Parse(Regex.Match(lName, @"\d+$").Value);
                if ((nName != null && nName != "") && newName.Contains("Lab"))
                {
                    var labGr = int.Parse(Regex.Match(labName, @"\d+$").Value);
                    newNum = "" + nName;
                    switch (labGr)
                    {
                        case 1:
                            newNum += "`";
                            break;
                        case 2:
                            newNum += "``";
                            break;
                        case 3:
                            newNum += "```";
                            break;   
                    }
                    newName = courseRow.ItemArray[1].ToString().Replace("_" + labName, "_" + newNum);
                }
                else if ((nName == null || nName == "") && labName.StartsWith("Lab"))
                {
                    string substr = newName.Substring(newName.Length - 4);
                    var labGr = (labName.Contains("Lab")) ? int.Parse(Regex.Match(labName, @"\d+$").Value) : int.Parse(Regex.Match(labName, @"\d+$").Value);
                    newNum = "" + gr;
                    switch (labGr)
                    {
                        case 1:
                            newNum += "a`";
                            break;
                        case 2:
                            newNum += "a``";
                            break;
                        case 3:
                            newNum += "b`";
                            break;
                        case 4:
                            newNum += "b``";
                            break;
                        case 5:
                            newNum += "c`";
                            break;
                        case 6:
                            newNum += "c``";
                            break;
                    }
                    newName = courseRow.ItemArray[1].ToString().Replace("" + labName, "_" + newNum);
                }

                string values = " name='" + newName +
                               "', laboratory_group='" + newNum + "'";

                int r = DBConnection.Update("courses", values, id);
            }
        }*/
    }
}
