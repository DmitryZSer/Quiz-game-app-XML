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
using System.Xml;
using Microsoft.VisualBasic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Test
{
    public partial class MainMenu : Form
    {

        private string selectedFile;
        bool AdminMode = false;
        string password = "";

        AdminPanel adminPanel;
        QuizForm quizForm;

        public MainMenu()
        {
            InitializeComponent();

            try
            {
                comboBox1.Items.Clear();
                dirs = FileController.LoadList(Path.GetDirectoryName(Application.ExecutablePath));
                comboBox1.Items.AddRange(dirs.ToArray());
                comboBox1.SelectedIndex = 0;
            }
            catch
            {
                MessageBox.Show("Ошибка при обнаружении исполняемого файла!");
            }

            UpdateThemes();
        }

        List<string> dirs = new List<string>();
        static class FileController
        {
            public static List<string> LoadList(string p)
            {
                List<string> paths = Directory.GetFiles(p).ToList<string>();
                List<string> temp = new List<string>();
                for (int i = 0; i < paths.Count; i++)
                {
                    paths[i] = Path.GetFileName(paths[i]);
                }

                foreach (var item in paths)
                {
                    if (item.Split('.')[1] == "xml") temp.Add(item);
                }

                return temp;
            }
        }

        private void UpdateThemes()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(comboBox1.Text);

            comboBox2.Items.Clear();
            XmlNodeList topicNodes = xmlDoc.SelectNodes("//Topic");
            foreach (XmlNode topicNode in topicNodes)
            {
                string topicName = topicNode.Attributes["Name"].Value;
                comboBox2.Items.Add(topicName);
            }
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                string selectedFile = comboBox1.SelectedItem.ToString();

                if (IsFormAlreadyOpen(typeof(QuizForm)))
                {
                    //quizForm.BringToFront();
                    MessageBox.Show("Форма уже открыта.");
                }
                else
                {
                    QuizForm quizForm = new QuizForm(selectedFile);
                    quizForm.Show();
                }
                //this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                MessageBox.Show("Выберите тему перед началом теста.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedFile = comboBox1.Text;
        }

        private bool IsFormAlreadyOpen(Type formType)
        {
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm.GetType() == formType)
                {
                    return true;
                }
            }
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IsFormAlreadyOpen(typeof(AdminPanel)))
            {
                //adminPanel.BringToFront();
                MessageBox.Show("Форма уже открыта.");
            }
            else
            {
                if (comboBox1.SelectedIndex != -1)
                {
                    AdminPanel adminPanel = new AdminPanel(selectedFile);
                    adminPanel.Show();
                }
                else
                {
                    MessageBox.Show("Выберите файл перед началом теста.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != -1 && comboBox1.SelectedIndex != -1)
            {
                string selectedFile = comboBox1.SelectedItem.ToString();
                string selectedTopic = comboBox2.Text.ToString();

                if (IsFormAlreadyOpen(typeof(QuizForm)))
                {
                    //quizForm.BringToFront();
                    MessageBox.Show("Форма уже открыта.");
                }
                else
                {
                    QuizForm quizForm = new QuizForm(selectedFile, selectedTopic);
                    quizForm.Show();
                }
            }
            else
            {
                MessageBox.Show("Выберите тему перед началом теста.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
