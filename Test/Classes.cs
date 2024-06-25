using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Test
{
    public class QuizData
    {
        public List<Topic> Topics { get; } = new List<Topic>();

        public QuizData(string filePath)
        {
            LoadData(filePath);
        }

        private void LoadData(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            XmlNodeList topicNodes = xmlDoc.SelectNodes("//Topic");
            foreach (XmlNode topicNode in topicNodes)
            {
                Topic topic = new Topic(topicNode);
                Topics.Add(topic);
            }
        }
    }

    public class Topic
    {
        public string Name { get; }
        public List<Level> Levels { get; } = new List<Level>();

        public Topic(XmlNode topicNode)
        {
            Name = topicNode.Attributes["Name"].Value;

            XmlNodeList levelNodes = topicNode.SelectNodes("Level");
            foreach (XmlNode levelNode in levelNodes)
            {
                Level level = new Level(levelNode);
                Levels.Add(level);
            }
        }
    }

    public class Level
    {
        public int Number { get; }
        public List<Question> Questions { get; } = new List<Question>();

        public Level(XmlNode levelNode)
        {
            Number = int.Parse(levelNode.Attributes["Number"].Value);

            XmlNodeList questionNodes = levelNode.SelectNodes("Question");
            foreach (XmlNode questionNode in questionNodes)
            {
                Question question = new Question(questionNode);
                Questions.Add(question);
            }
        }
    }

    public class Question
    {
        public string Text { get; }
        public List<Answer> Answers { get; } = new List<Answer>();

        public Question(XmlNode questionNode)
        {
            Text = questionNode.SelectSingleNode("Text").InnerText;

            XmlNodeList answerNodes = questionNode.SelectNodes("Answer");
            foreach (XmlNode answerNode in answerNodes)
            {
                Answer answer = new Answer(answerNode); //ошибка при инициализации
                Answers.Add(answer);
            }
        }
    }


    public class Answer
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }

        public Answer(XmlNode answerNode)
        {
            Text = answerNode.InnerText;
            IsCorrect = bool.Parse(answerNode.Attributes["IsCorrect"].Value);
        }
    }

}
