
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddStudentWindow.xaml
    /// </summary>
    public partial class AddStudentWindow : Window
    {
        /// <summary>
        /// Список специальностей.
        /// </summary>
        public ObservableCollection<string> Specialities { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// Данные студента.
        /// </summary>
        public Student Student { get; private set; }

        /// <summary>
        /// Создает новое окно добавления студента.
        /// </summary>
        /// <param name="student">Студент для добавления или редактирования.</param>
        public AddStudentWindow(Student student)
        {
            InitializeComponent();
            Student = student;
            DataContext = Student;

            UpdateSpecialities(Student.Department);
            var departmentBinding = new Binding("Department")
            {
                Source = Student,
                Mode = BindingMode.TwoWay
            };
            departmentBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            departmentBinding.NotifyOnSourceUpdated = true;
            SetBinding(DepartmentProperty, departmentBinding);

        }

        /// <summary>
        /// Используется для отслеживания изменений факультета.
        /// </summary>
        public static readonly DependencyProperty DepartmentProperty = DependencyProperty.Register("Department", typeof(string), typeof(AddStudentWindow),
                new PropertyMetadata("", OnDepartmentChanged));

        /// <summary>
        /// Обрабатывает изменение значения факультета.
        /// </summary>
        /// <param name="d">Объект зависимости, у которого изменилось значение свойства.</param>
        /// <param name="e">Аргументы, связанные с изменением значения свойства.</param>
        private static void OnDepartmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as AddStudentWindow;
            if (window != null)
            {
                window.UpdateSpecialities(e.NewValue as string);
            }
        }

        /// <summary>
        /// Проверяет вводимый текст на соответствие русским буквам.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события, содержащие информацию о тексте, вводимом пользователем.</param>
        private void ValidateRussianText(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^а-яА-ЯёЁ]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Проверяет текст, вставляемый в поле, на соответствие русским буквам.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события, содержащие информацию о тексте, вставляемом пользователем.</param>
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                Regex regex = new Regex("[^а-яА-ЯёЁ]+");
                if (regex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Обновляет список специальностей в зависимости от выбранного факультета.
        /// </summary>
        /// <param name="department">Выбранный факультет.</param>
        private void UpdateSpecialities(string department)
        {
            Specialities.Clear();
            switch (department)
            {
                case "ФВТ":
                    Specialities.Add("Компьютерные науки");
                    Specialities.Add("Информационные системы");
                    break;
                case "ФИТЭ":
                    Specialities.Add("Телекоммуникации");
                    Specialities.Add("Радиотехника");
                    break;
                case "Стоматология":
                    Specialities.Add("Терапевтическая стоматология");
                    Specialities.Add("Ортопедическая стоматология");
                    break;
                case "ФИТиКН":
                    Specialities.Add("Кибербезопасность");
                    Specialities.Add("Прикладная математика");
                    break;
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку "Принять".
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Student.Name) ||
                string.IsNullOrWhiteSpace(Student.Surname) ||
                string.IsNullOrWhiteSpace(Student.Patronymic) ||
                string.IsNullOrWhiteSpace(Student.Department) ||
                string.IsNullOrWhiteSpace(Student.Semester) ||
                string.IsNullOrWhiteSpace(Student.Nomer) ||
                string.IsNullOrWhiteSpace(Student.Speciality))
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
        /// <param name="sender">Объект, вызвавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
    }
}
