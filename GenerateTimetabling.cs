using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ccbt;
using ccbt.Models;
using System.Windows;
using System.IO;
using System.Collections;

namespace UniversityTimetabling
{
    class GenerateTimetabling
    {
        public void main()
        {
        }

        public void generateTimetabling( Thread loadingThread)
        {
            try
            {
				string path = "comp01";

				GenerateInstance instance = new GenerateInstance();
				instance.main(path);

				ResetILS();

				IO.Read(path);
				
				Solution solution = new Solution();
				ILS ils = new ILS();
				solution = ils.FindSolution();

				IO.Write(solution.assignments, solution.GetScore());
				insertSolutionOnSystem(IO.filename);

				loadingThread.Abort();
			}
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

		public void ResetILS()
		{
			Instance.Rooms = new Dictionary<string, Room>();
			Instance.Courses = new Dictionary<string, Course>();
			Instance.Curricula = new Dictionary<string, Curriculum>();
			Instance.FixedAssignments = new List<ccbt.Models.Assignment>();
		}

		public void generateRemainingTimetabling(string fixedPath, Thread loadingThread)
        {
            try
            {
				string path = "comp01";

				GenerateInstance instance = new GenerateInstance();
                instance.main(path);

				ResetILS();

				IO.Read(path, fixedPath);
                Solution solution = new Solution();
                ILS ils = new ILS();
                solution = ils.FindSolution();

                IO.Write(solution.assignments, solution.GetScore());
				insertSolutionOnSystem(IO.filename);
				loadingThread.Abort();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void insertSolutionOnSystem(string path)
        {
			//clean timetable_temp
			string currentSemester = getCurrentSemester();
			DBConnection.Delete("timetable_" + currentSemester);

			// Open document 
			List<string> myValues = new List<string>();
			string line;
			// Read the file and display it line by line.
			StreamReader file = new StreamReader("Output/" + path);
			while ((line = file.ReadLine()) != null)
			{
				myValues.Add(line);
			}

			foreach (string item in myValues)
			{

				string row = string.Join(" ", item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
				string[] items = row.Split(' ');

				if ((items.Count() != 4) || !items[0].StartsWith("c") || !items[1].StartsWith("r"))
				{
					throw new Exception("Selected file is not on the correct format!");
				}

				string values = "'" + items[0] + "', '" + items[1] + "', '" + items[2] + "', '" + items[3] + "'";

				int res = DBConnection.Create("timetable_" + currentSemester, "course_code, room_code, day, start_period", values);
			}
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
	}
}
