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
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UniversityTimetabling
{
    /// <summary>
    /// Interaction logic for TeacherAddEdit.xaml
    /// </summary>
    public partial class TeacherAddEdit : Window
    {
        public TeacherAddEdit()
        {
            InitializeComponent();
        }

        private void saveTeacher_MouseUp(object sender, EventArgs e)
        {
			if (txtName.Text.Length == 0)
			{
				MessageBox.Show("Teacher name is required!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}
			if (txtSurname.Text.Length == 0)
			{
				MessageBox.Show("Teacher surname is required!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

            if(txtEmail.Text.Length > 0 && !Regex.IsMatch(txtEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                MessageBox.Show("Please enter a valid email!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }


            if (txtId.Text.Length == 0)
            {
                string code = null;
                for (int i = 1; i < 1000; i++)
                {
                    code = "t" + i.ToString("000");
                    DataView data = DBConnection.Select("teachers", "'*'", " code = '" + code + "'").DefaultView;
                    if (data.Count == 0) break;
                }

                string values = "'" + txtName.Text + 
                                "', '" + txtSurname.Text + 
                                "', '" + txtTitle.Text + 
                                "', '" + txtEmail.Text + 
                                "', '" + txtPhone.Text +
                                "', '" + code.ToString() +
                                "', '" + txtNote.Text + "'";

                int result = DBConnection.Create("teachers", "name, surname, title, email, phone, code, note", values);
                if (result > 0)
                {
                    MessageBox.Show("Successfully created new Teacher!", "Information", MessageBoxButton.OK);
                    txtId.Text = result.ToString();
                }
            }
            else if (Convert.ToInt32(txtId.Text) > 0)
            {
                string values = "name='" + txtName.Text +
                                "', surname='" + txtSurname.Text +
                                "', title='" + txtTitle.Text +
                                "', email='" + txtEmail.Text +
                                "', phone='" + txtPhone.Text +
                                "', code='" + txtCode.Text +
                                "', note='" + txtNote.Text + "'";

                int result = DBConnection.Update("teachers", values, Convert.ToInt32(txtId.Text));
                if (result > 0)
                    MessageBox.Show("Successfully updated Teacher!", "Information", MessageBoxButton.OK);
            }
        }

        private void close_MouseUp(object sender, EventArgs e)
        {
            Close();
        }

        private void teacherPreferences_MouseUp(object sender, EventArgs e)
        {
            if (txtId.Text != "")
            {
                PreferencesWindow win = new PreferencesWindow(txtId.Text);
                win.Owner = this;
                win.ShowDialog();
            }
        }

        private void txtId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(txtId.Text == "")
            {
                teacherPreferences.Visibility = Visibility.Hidden;
                labelTeacherPreferences.Visibility = Visibility.Hidden;
            }
            else
            {
                teacherPreferences.Visibility = Visibility.Visible;
                labelTeacherPreferences.Visibility = Visibility.Visible;
            }
        }

        private void teacherPreferences_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                teacherPreferences_MouseUp(sender, e);
                Keyboard.ClearFocus();
            }
        }

        private void saveTeacher_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                saveTeacher_MouseUp(sender, e);
                Keyboard.ClearFocus();
            }
        }

        private void close_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                close_MouseUp(sender, e);
                Keyboard.ClearFocus();
            }
        }
    }
}
