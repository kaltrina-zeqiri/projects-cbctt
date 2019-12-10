using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UniversityTimetabling
{
    class Assignment
    {
        private int Course_id;
        public string Course_name;
        public string Course_code;
        public string Room_code;
        public int Teacher_id;
        public int Day;
        public int Starting_period;
        public int Ending_period;

        public Assignment(string course_code, string room_code, int day, int start_period)
        {
            try
            {
                Course_code = course_code;
                Room_code = room_code;
                Day = day;
                Starting_period = start_period;

                Teacher_id = getTeacherId();
                if (Course_id > 0)
                    Ending_period = getEndingPeriod();
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private int getTeacherId()
        {
            int res = 0;
            string Columns = " c.id, t.id, c.name";

            string From = "courses c" +
                " JOIN course_teacher_rel ctr ON c.id=ctr.course_id " +
                 " JOIN teachers t ON ctr.teacher_id=t.id ";

            string Conditions = " c.code='" + Course_code + "'";
            
            DataTable result = DBConnection.Select(From, Columns, Conditions);

            foreach (DataRow r in result.Rows)
            {
                Course_id = int.Parse(r.ItemArray[0].ToString());
                res = int.Parse(r.ItemArray[1].ToString());
                Course_name = r.ItemArray[2].ToString();
            }
            return res;
        }

        private int getEndingPeriod()
        {
            Dictionary<string, string> first = new Dictionary<string, string>();
            string detail, numberOfLectures;
            ArrayList rec = DBConnection.Get("courses", Course_id);
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
            int endPeriod = Starting_period + (3 * int.Parse(numberOfLectures)) + breaks;

            return endPeriod;
        }

        public string getCourseTextPeriod()
        {
            string name1, name2;
            Dictionary<string, string> startPeriod = new Dictionary<string, string>();
            ArrayList startSubPeriod = DBConnection.Get("subPeriods", Starting_period + 1);
            startPeriod = (Dictionary<string, string>)startSubPeriod[0];
            startPeriod.TryGetValue("name", out name1);

            Dictionary<string, string> endPeriod = new Dictionary<string, string>();
            ArrayList endSubPeriod = DBConnection.Get("subPeriods", Ending_period + 1);
            endPeriod = (Dictionary<string, string>)endSubPeriod[0];
            endPeriod.TryGetValue("name", out name2);

            return name1 + " to " + name2;
        }
    }
}
