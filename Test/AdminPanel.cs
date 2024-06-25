using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Test
{
    public partial class AdminPanel : Form
    {
        private string xmlFilePath;
        private XmlDocument xmlDoc;

        public AdminPanel(string filepath)
        {
            InitializeComponent();

            xmlFilePath = filepath;

            xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);

            LoadTopics();

            comboBoxLevels.Items.Add(1);
            comboBoxLevels.Items.Add(2);
            comboBoxLevels.Items.Add(3);
        }

        private void LoadTopics()
        {
            comboBoxTopics.Items.Clear();
            XmlNodeList topicNodes = xmlDoc.SelectNodes("//Topic");
            foreach (XmlNode topicNode in topicNodes)
            {
                string topicName = topicNode.Attributes["Name"].Value;
                comboBoxTopics.Items.Add(topicName);
            }
        }

        private void btnAddTopic_Click(object sender, EventArgs e)
        {
            string newTopicName = textBoxNewTopic.Text.Trim();
            if (string.IsNullOrEmpty(newTopicName))
            {
                MessageBox.Show("Пожалуйста, введите название темы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            XmlNode existingTopic = xmlDoc.SelectSingleNode($"//Topic[@Name='{newTopicName}']");
            if (existingTopic != null)
            {
                MessageBox.Show("Тема уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            XmlNode root = xmlDoc.DocumentElement;
            XmlElement newTopic = xmlDoc.CreateElement("Topic");
            newTopic.SetAttribute("Name", newTopicName);
            root.AppendChild(newTopic);
            xmlDoc.Save(xmlFilePath);

            LoadTopics();
        }

        private void btnAddQuestion_Click(object sender, EventArgs e)
        {
            string selectedTopic = comboBoxTopics.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedTopic))
            {
                MessageBox.Show("Выберите тему.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int selectedLevel;
            if (!int.TryParse(comboBoxLevels.SelectedItem?.ToString(), out selectedLevel))
            {
                MessageBox.Show("Выберите уровень.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string questionText = textBoxQuestion.Text.Trim();
            if (string.IsNullOrEmpty(questionText))
            {
                MessageBox.Show("Введите вопрос.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (dataGridViewAnswers.Rows.Count < 3)
            {
                MessageBox.Show("Дайте хотя бы два ответа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool hasCorrectAnswer = false;
            bool hasEmptyCell = false;
            foreach (DataGridViewRow row in dataGridViewAnswers.Rows)
            {
                if (row.Cells[0].Value == null)
                {
                    hasEmptyCell = true;
                    break;
                }
                else if (row.Cells[1].Value != null && (bool)row.Cells[1].Value)
                {
                    hasCorrectAnswer = true;
                }

            }
            if (!hasCorrectAnswer)
            {
                MessageBox.Show("Дайте хотя бы один правильный ответ.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!hasEmptyCell)//не работает
            {
                MessageBox.Show("Заполните все поля ответов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            XmlNode root = xmlDoc.DocumentElement;
            XmlNode selectedTopicNode = root.SelectSingleNode($"//Topic[@Name='{selectedTopic}']");
            if (selectedTopicNode == null)
            {
                MessageBox.Show("Выбранная тема не найдена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            XmlNode selectedLevelNode = selectedTopicNode.SelectSingleNode($"Level[@Number='{selectedLevel}']");
            if (selectedLevelNode == null)
            {
            // Если выбранного уровня не существует, создаем новый
                XmlElement newLevelNode = xmlDoc.CreateElement("Level");
                newLevelNode.SetAttribute("Number", selectedLevel.ToString());
                selectedTopicNode.AppendChild(newLevelNode);
                selectedLevelNode = newLevelNode;
            }

            XmlElement questionNode = xmlDoc.CreateElement("Question");
            XmlElement textNode = xmlDoc.CreateElement("Text");
            textNode.InnerText = questionText;
            questionNode.AppendChild(textNode);

            foreach (DataGridViewRow row in dataGridViewAnswers.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    XmlElement answerNode = xmlDoc.CreateElement("Answer");
                    answerNode.InnerText = row.Cells[0].Value.ToString();
                    answerNode.SetAttribute("IsCorrect", row.Cells[1].Value != null && (bool)row.Cells[1].Value ? "true" : "false");
                    questionNode.AppendChild(answerNode);
                }
            }

            selectedLevelNode.AppendChild(questionNode);
            xmlDoc.Save(xmlFilePath);

            MessageBox.Show("Вопрос успешно добавлен!!!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearQuestionFields();
        }


        private void ClearQuestionFields()
        {
            textBoxQuestion.Clear();
            dataGridViewAnswers.Rows.Clear();
        }
    }
}





