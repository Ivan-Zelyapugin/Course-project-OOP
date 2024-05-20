
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MyUniversity.Models;

namespace MyUniversity.Views
{
    /// <summary>
    /// Логика взаимодействия для SubjectsWindow.xaml
    /// </summary>
    public partial class SubjectsWindow : Window
    {
        /// <summary>
        /// Получает или задает объект предмета.
        /// </summary>
        public Subject Subject { get; private set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса SubjectsWindow.
        /// </summary>
        /// <param name="subject">Объект предмета.</param>
        public SubjectsWindow(Subject subject)
        {
            Subject = subject;
            InitializeComponent();
            DataContext = Subject;
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Принять".
        /// </summary>
        void Accept_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(Subject.Name) ||
                string.IsNullOrWhiteSpace(Subject.Grade.ToString()))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            else
            {
                DialogResult = true;
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Выход".
        /// </summary>
        void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
