using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace StudentsDiary
{
    public partial class Main : Form
    {
        private string _filePath = Path.Combine(Environment.CurrentDirectory, "students.txt"); 
        
        public Main()
        {
            InitializeComponent(); //konstruktor

            var students = DeserializeFromFile();

            dgvDiary.DataSource = students;
        }
             
       
        public void SerializeToFile(List<Student> students)
        {
            var serializer = new XmlSerializer(typeof(List<Student>));

            using (var streamWriter = new StreamWriter(_filePath))
            {
                serializer.Serialize(streamWriter, students);
                streamWriter.Close();
            }

        }

        public List<Student> DeserializeFromFile()
        {
            if (!File.Exists(_filePath))
               return new List<Student>();
                        
            var serializer = new XmlSerializer(typeof(List<Student>));

            using (var streamReader = new StreamReader(_filePath))
            {
               var students = (List<Student>)serializer.Deserialize(streamReader); //rzutowanie na listę studentów wyniku metody deserialize
                streamReader.Close();
                return students;
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addEditStudent = new AddEditStudent();
            addEditStudent.ShowDialog();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvDiary.SelectedRows.Count == 0)
            {
                MessageBox.Show("Proszę zaznacz ucznia, którego dane chcesz edytować.");
                return;
            }

            var addEditStudent = new AddEditStudent(Convert.ToInt32(dgvDiary.SelectedRows[0].Cells[0].Value));
            addEditStudent.ShowDialog();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDiary.SelectedRows.Count == 0)
            {
                MessageBox.Show("Proszę zaznacz ucznia, którego chcesz usunąć.");
                return;
            }

            var selectedStudent = dgvDiary.SelectedRows[0];

           var confirmDelete = 
                MessageBox.Show($"Czy na pewno chcesz usunąć ucznia {(selectedStudent.Cells[1].Value.ToString() + " " + selectedStudent.Cells[2].Value.ToString()).Trim()}", 
                "Usuwanie ucznia", 
                MessageBoxButtons.OKCancel);

            if (confirmDelete == DialogResult.OK)
            {
                var students = DeserializeFromFile();
                students.RemoveAll(x =>
                    x.Id == Convert.ToInt32(selectedStudent.Cells[0].Value));
                SerializeToFile(students);
                dgvDiary.DataSource = students; //odświeżenie tabelki z uczniami
            }
                
                
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var students = DeserializeFromFile();

            dgvDiary.DataSource = students;
        }
    }
}
