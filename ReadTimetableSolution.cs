using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;

namespace UniversityTimetabling
{
    class ReadTimetableSolution
    {
        public void main()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".txt";
                dlg.Filter = "TXT Files (*.txt)|*.txt";

                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();

                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    //clean timetable_temp
                    DBConnection.Delete("timetable_temp");

                    // Open document 
                    string filename = dlg.FileName;
                    List<string> myValues = new List<string>();
                    string line;
                    // Read the file and display it line by line.
                    StreamReader file = new StreamReader(filename);
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

                        int res = DBConnection.Create("timetable_temp", "course_code, room_code, day, start_period", values);
                    }

                    TimetablingResultDisplay display = new TimetablingResultDisplay();
                    display.Show();

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
