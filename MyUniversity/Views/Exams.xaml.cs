
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MyUniversity.Models;
using MyUniversity.ViewModels;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyUniversity.Views
{
    /// <summary>
    /// Логика взаимодействия для Exams.xaml
    /// </summary>
    public partial class Exams : Window
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса Exams.
        /// </summary>
        /// <param name="db">Контекст базы данных.</param>
        /// <param name="student">Студент, чьи экзамены будут отображены.</param>
        public Exams(UniversityContext db, Student student)
        {
            InitializeComponent();
            DataContext = new ExamsViewModel(db, student);
        }
    }
}
