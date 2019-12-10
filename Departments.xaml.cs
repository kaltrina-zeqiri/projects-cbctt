using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UniversityTimetabling
{
    /// <summary>
    /// Interaction logic for Departments.xaml
    /// </summary>
    public partial class Departments : Window
    {
        public Departments()
        {
            InitializeComponent();
        }

        private void departments_Initialized(object sender, EventArgs e)
        {
            string Columns = " d.id AS id, d.name AS name, d.faculty_id AS faculty_id, f.name AS faculty ";
            string Table = " departments d JOIN faculties f ON d.faculty_id = f.id";
            departmentsGrid.ItemsSource = DBConnection.Select(Table, Columns, null, null, " d.name ").DefaultView;
        }

        private void editDepartment_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)departmentsGrid.SelectedItem;
            if (row != null)
            {
                int id = (int)row["ID"];
                string name = (string)row["Name"];
                string faculty = (string)row["Faculty"];

                DepartmentAddEdit addEdit = new DepartmentAddEdit();
                addEdit.Closed += (s, eventarg) =>
                {
                    departments_Initialized(s, eventarg);
                };
                addEdit.Owner = this;
                addEdit.txtId.Text = id.ToString();
                addEdit.txtName.Text = name;
                addEdit.comboBox.Text = faculty;
                addEdit.ShowDialog();
            }
            else
                MessageBox.Show("Please select a row to edit!", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void deleteDepartment_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = (DataRowView)departmentsGrid.SelectedItem;
            if (row != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure to delete this study program?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    int id = (int)row["ID"];
                    int result = DBConnection.Delete("departments", id);
                    if (result > 0)
                        MessageBox.Show("Selected study program has been successfully deleted!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    departments_Initialized(sender, e);
                }
            }
            else
                MessageBox.Show("Please select a row to delete!", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void addDepartment_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DepartmentAddEdit addEdit = new DepartmentAddEdit();
            addEdit.Closed += (s, eventarg) =>
            {
                departments_Initialized(s, eventarg);
            };
            addEdit.Owner = this;
            addEdit.ShowDialog();
        }

        private void refreshDepartments_MouseUp(object sender, MouseButtonEventArgs e)
        {
            departments_Initialized(sender, e);
        }

        private void txtFilterByName_TextChanged(object sender, TextChangedEventArgs e)
        {
            filter();
        }

        private void filter()
        {
            string name = txtFilterByName.Text;

            string Columns = " d.id AS id, d.name AS name, d.faculty_id AS faculty_id, f.name AS faculty ";
            string Table = " departments d JOIN faculties f ON d.faculty_id = f.id";
            string Conditions = null;

            if (name != "") Conditions = " d.name LIKE '%" + name + "%' ";

            departmentsGrid.ItemsSource = DBConnection.Select(Table, Columns, Conditions).DefaultView;
        }

        private void departmentsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            editDepartment_MouseUp(sender, e);
        }
    }
}
