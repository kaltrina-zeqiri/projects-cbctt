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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UniversityTimetabling
{
    /// <summary>
    /// Interaction logic for Teachers.xaml
    /// </summary>
    public partial class Teachers : Window
    {
        public Teachers()
        {
            InitializeComponent();
        }

        private void teachers_Initialized(object sender, EventArgs e)
        {
            string Columns = " id, name, surname, title, email, phone, code, note ";
            teachersGrid.ItemsSource = DBConnection.Select("teachers", Columns, null, null, " name ").DefaultView;
        }

        private void addTeacher_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TeacherAddEdit teacher = new TeacherAddEdit();
            teacher.Owner = this;
            teacher.Closed += (s, eventarg) =>
            {
                teachers_Initialized(s, eventarg);
            };
            teacher.teacherPreferences.Visibility = Visibility.Hidden;
            teacher.labelTeacherPreferences.Visibility = Visibility.Hidden;
            teacher.ShowDialog();
        }

        private void editTeacher_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)teachersGrid.SelectedItem;
            if (row != null)
            {
                int id = (int)row["ID"];
                string name = (string)row["Name"];
                string surname = (string)row["Surname"];
                string title = (string)row["Title"];
                string email = (string)row["Email"];
                string phone = (string)row["Phone"];
                string code = (string)row["Code"];
                string note = (string)row["Note"];
                
                TeacherAddEdit teacher = new TeacherAddEdit();
                teacher.Owner = this;
                teacher.Closed += (s, eventarg) =>
                {
                    teachers_Initialized(s, eventarg);
                };
                teacher.txtId.Text = id.ToString();
                teacher.txtName.Text = name;
                teacher.txtSurname.Text = surname;
                teacher.txtTitle.Text = title;
                teacher.txtEmail.Text = email;
                teacher.txtPhone.Text = phone;
                teacher.txtCode.Text = code;
                teacher.txtNote.Text = note;
                teacher.ShowDialog();
            }
            else
                MessageBox.Show("Please select a row to edit!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void deleteTeacher_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)teachersGrid.SelectedItem;
            if (row != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to delete this teacher?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    int id = (int)row["ID"];
                    int result = DBConnection.Delete("teachers", id);
                    if (result > 0)
                        MessageBox.Show("Successfully deleted Teacher!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    teachers_Initialized(sender, e);
                }
            }
            else
                MessageBox.Show("Please select a row to delete!", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void refreshTeachers_MouseUp(object sender, MouseButtonEventArgs e)
        {
            teachers_Initialized(sender, e);
        }

        private void txtFilterByName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Columns = " id, name, surname, title, email, phone, code, note ";
            string Conditions = null;
            string name = txtFilterByName.Text;
            string surname = txtFilterBySurname.Text;

            if (name != "" && surname != "")
                Conditions = " name LIKE '%" + name + "%' AND surname LIKE '%" + surname + "%' ";
            else if (name != "")
                Conditions = " name LIKE '%" + name + "%'";
            else if (surname != "")
                Conditions = " surname LIKE '%" + surname + "%'";

            teachersGrid.ItemsSource = DBConnection.Select("teachers", Columns, Conditions).DefaultView;
        }

        private void teachersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            editTeacher_MouseUp(sender, e);
        }
    }
}
