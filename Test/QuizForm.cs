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
    public partial class QuizForm : Form
    {
        private QuizData quizData;

        Topic currentTopic = null;

        private Random random = new Random();

        private int currentTopicIndex = 0;
        private int currentLevelIndex = 0;
        private int currentQuestionIndex = 0;

        private string selectedTopic = null;
        private bool isFirstTime = true;

        private int gameTimer = 300;
        private int totalScore = 0;
        

        public QuizForm(string filepath, string selectedTop)
        {
            InitializeComponent();
            quizData = new QuizData(filepath);
            this.selectedTopic = selectedTop;
            ShowNextQuestion();

            timer1.Start();
            timer2.Start();
            labelTimer.Text = "Оставшееся время: " + gameTimer.ToString();
            labelScore.Text = "Текущий счет: " + totalScore;
        }

        public QuizForm(string filepath)
        {
            InitializeComponent();
            quizData = new QuizData(filepath);
            ShowNextQuestion();

            timer1.Start();
            timer2.Start();
            labelTimer.Text = "Оставшееся время: " + gameTimer.ToString();
            labelScore.Text = "Текущий счет: " + totalScore;
        }

        private void ShowNextQuestion()
        {
            if (currentTopicIndex >= quizData.Topics.Count)
            {
                MessageBox.Show("Тест завершен!");
                return;
            }
            
            
            if (selectedTopic != null && isFirstTime)
            {
                currentTopic = quizData.Topics.FirstOrDefault(t => t.Name == selectedTopic);
                isFirstTime = false;
            }
            else if(selectedTopic == null)
            {
                currentTopic = quizData.Topics[currentTopicIndex];
            }

            if (currentTopic == null)
            {
                MessageBox.Show("Выбранная тема не найдена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            labelTheme.Text = currentTopic.Name;
            if (currentLevelIndex >= currentTopic.Levels.Count)
            {
                if(selectedTopic != null) {
                    MessageBox.Show("Тест завершен!");
                    this.Close();
                    return;

                }
                currentTopicIndex++;
                currentLevelIndex = 0;
                currentQuestionIndex = 0;
                ShowNextQuestion();
                return;
            }

            Level currentLevel = currentTopic.Levels[currentLevelIndex];
            label1.Text = "Текущий уровень: " + currentLevel.Number;
            if (currentQuestionIndex >= currentLevel.Questions.Count && totalScore > 80)
            {
                totalScore = 0;
                currentLevelIndex++;
                currentQuestionIndex = 0;
                ShowNextQuestion();
                return;
            }
            else if (currentQuestionIndex >= currentLevel.Questions.Count)
            {
                //currentLevelIndex++;
                currentQuestionIndex = 0;
                ShowNextQuestion();
                return;
            }

            if(currentQuestionIndex == 0)
            {
                Shuffle(currentLevel.Questions);
            }

            Question currentQuestion = currentLevel.Questions[currentQuestionIndex];
            Shuffle(currentQuestion.Answers);
            DisplayQuestion(currentQuestion);
            currentQuestionIndex++;
        }

        private void Shuffle<T>(List<T> list)//Для перемешивания строки
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }



        public List<Answer> CurrrrentAnswers = new List<Answer>();

        private void DisplayQuestion(Question question)
        {
            labelQuestion.Text = question.Text;
            
            flowLayoutPanelAnswers.Controls.Clear();
            CurrrrentAnswers.Clear();
            // Добавим новые варианты ответов на форму
            foreach (Answer answer in question.Answers)
            {
                RadioButton radioButton = new RadioButton();
                //radioButton.CheckAlign = ContentAlignment.MiddleLeft;
                
                radioButton.Text = answer.Text; 
                radioButton.Size = new Size(400, 50);
                //radioButton.ForeColor = Color.Cornsilk;

                flowLayoutPanelAnswers.Controls.Add(radioButton);
                
                CurrrrentAnswers.Add(answer);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckAnswer())
            {
                totalScore += 30;
                ShowNextQuestion();
            }
            else totalScore -= 5;
            
        }

        private bool CheckAnswer()
        {
            foreach (Control control in flowLayoutPanelAnswers.Controls)
            {
                if (control is RadioButton radioButton && radioButton.Checked)
                {
                    string selectedAnswerText = radioButton.Text;
                        foreach (Answer answer in CurrrrentAnswers)
                        {
                            if (answer.Text == selectedAnswerText && answer.IsCorrect)
                            {
                                //MessageBox.Show("Правильный ответ!");
                                return true;
                            }
                        }
                    MessageBox.Show("Неправильный ответ!");
                    return false;
                }
            }
            MessageBox.Show("Выберите ответ!");
            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelTimer.Text = "Оставшееся время: " + gameTimer.ToString();
            if (gameTimer == 0)
            {
                MessageBox.Show("Время закончилось!(");
                timer1.Stop();
                this.Close();
            }
            gameTimer--;
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            labelScore.Text = "Текущий счет: " + totalScore;
        }
    }
}
