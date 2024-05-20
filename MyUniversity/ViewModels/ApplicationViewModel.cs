
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using MyUniversity.Command;
using MyUniversity.Models;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using LiveCharts;
using System.Windows.Input;
using LiveCharts.Wpf;
using System.Xml;
using Newtonsoft.Json;
using MyUniversity.Views;

namespace MyUniversity.ViewModels
{
    /// <summary>
    /// Представляет модель представления приложения.
    /// </summary>
    public class ApplicationViewModel:INotifyPropertyChanged
    {
        private double _axisYStep = 0.5;
        private string _searchText;
        private ObservableCollection<string> _criteria;
        private string _selectedCriteria;
        private SeriesCollection _chartSeries;
        private string[] _axisLabels;
        private Func<double, string> _formatter;

        /// <summary>
        /// Получает или задает коллекцию студентов с данными о записях на курсы.
        /// </summary>
        public ObservableCollection<Student> StudentsWithSubjects { get; set; }

        /// <summary>
        /// Свойство получает или задает шаг оси Y для графика.
        /// </summary>
        public double AxisYStep
        {
            get => _axisYStep;
            set
            {
                _axisYStep = value;
                OnPropertyChanged(nameof(AxisYStep));
            }
        }

        /// <summary>
        /// Свойство получает или задает текст для поиска.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterStudents();
            }
        }

        /// <summary>
        /// Свойство получает или задает коллекцию критериев поиска.
        /// </summary>
        public ObservableCollection<string> Criteria
        {
            get => _criteria;
            set
            {
                _criteria = value;
                OnPropertyChanged(nameof(Criteria));
            }
        }

        /// <summary>
        /// Свойство получает или задает выбранный критерий поиска.
        /// </summary>
        public string SelectedCriteria
        {
            get => _selectedCriteria;
            set
            {
                _selectedCriteria = value;
                OnPropertyChanged(nameof(SelectedCriteria));
                LoadChartData(); // Обновляем данные графика при изменении критерия
            }
        }

        /// <summary>
        /// Свойство получает или задает серии данных для графика.
        /// </summary>
        public SeriesCollection ChartSeries
        {
            get => _chartSeries;
            set
            {
                _chartSeries = value;
                OnPropertyChanged(nameof(ChartSeries));
            }
        }

        /// <summary>
        /// Свойство получает или задает метки для оси X графика.
        /// </summary>
        public string[] AxisLabels
        {
            get => _axisLabels;
            set
            {
                _axisLabels = value;
                OnPropertyChanged(nameof(AxisLabels));
            }
        }

        /// <summary>
        /// Свойство получает или задает функцию форматирования для значения оси Y графика.
        /// </summary>
        public Func<double, string> Formatter
        {
            get => _formatter;
            set
            {
                _formatter = value;
                OnPropertyChanged(nameof(Formatter));
            }
        }

        /// <summary>
        /// Создание экземпляра контекста базы данных для работы с университетом.
        /// </summary>
        UniversityContext db = new UniversityContext();

        private RelayCommand _exitCommand;
        private RelayCommand _addCommand;
        private RelayCommand _editCommand;
        private RelayCommand _deleteCommand;
        private RelayCommand _createDatabaseCommand;
        private RelayCommand _deleteDatabaseCommand;
        private RelayCommand _sortCommand;
        private RelayCommand _showExams;
        private RelayCommand _exportToJsonCommand;
        private RelayCommand _generatePerformanceReportCommand;

        /// <summary>
        /// Команда для закрытия окна.
        /// </summary>
        public RelayCommand ExitCommand
        {
            get
            {
                return _exitCommand ??
                    (_exitCommand = new RelayCommand(obj =>
                    {
                        Application.Current.Shutdown();
                    }));
            }
        }

        /// <summary>
        /// Команда для добавления нового студента.
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??
                  (_addCommand = new RelayCommand((o) =>
                  {
                      if (db.Database.CanConnect())
                      {
                          AddStudentWindow studentWindow = new AddStudentWindow(new Student());

                          if (studentWindow.ShowDialog() == true)
                          {
                              Student student = studentWindow.Student;

                              var local = db.Set<Student>()
                                      .Local
                                      .FirstOrDefault(entry => entry.StudentId.Equals(student.StudentId));
                              if (local != null)
                              {
                                  db.Entry(local).State = EntityState.Detached;
                              }

                              db.Students.Add(student);
                              db.SaveChanges();
                              StudentsWithSubjects.Add(student);
                          }
                      }
                      else
                      {
                          MessageBox.Show("База данных отсутствует.");
                      }
                      
                  }));
            }
        }

        /// <summary>
        /// Команда для изменения данных о студенте.
        /// </summary>
        public RelayCommand EditCommand
        {
            get
            {
                return _editCommand ??
                  (_editCommand = new RelayCommand((selectedItem) =>
                  {
                      Student? student = selectedItem as Student;
                      if (student == null) return;

                      Student editedStudent = new Student
                      {
                          Nomer = student.Nomer,
                          Name = student.Name,
                          Surname = student.Surname,
                          Patronymic = student.Patronymic,
                          Semester = student.Semester,
                          Department = student.Department,
                          Speciality = student.Speciality
                      };
                      AddStudentWindow editWindow = new AddStudentWindow(editedStudent);

                      if (editWindow.ShowDialog() == true)
                      {
                          student.Nomer = editWindow.Student.Nomer;
                          student.Name = editWindow.Student.Name;
                          student.Surname = editWindow.Student.Surname;
                          student.Patronymic = editWindow.Student.Patronymic;
                          student.Semester = editWindow.Student.Semester;
                          student.Department = editWindow.Student.Department;
                          student.Speciality = editWindow.Student.Speciality;

                          db.Entry(student).State = EntityState.Modified;
                          db.SaveChanges();
                      }
                  }));
            }
        }

        /// <summary>
        /// Команда для удаления студента.
        /// </summary>
        public RelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??
                  (_deleteCommand = new RelayCommand((selectedItem) =>
                  {
                      Student? student = selectedItem as Student;
                      if (student == null) return;
                      db.Students.Remove(student);
                      db.SaveChanges();
                      StudentsWithSubjects.Remove(student);
                  }));
            }
        }

        /// <summary>
        /// Команда для создания базы данных.
        /// </summary>
        public RelayCommand CreateDatabaseCommand
        {
            get
            {
                return _createDatabaseCommand ??
                    (_createDatabaseCommand = new RelayCommand(obj =>
                    {
                        if (db.Database.CanConnect())
                        {
                            MessageBox.Show("База данных уже существует");
                        }
                        else
                        {
                            db.Database.EnsureDeleted();
                            db.Database.EnsureCreated();
                            MessageBox.Show("База данных была создана");
                            LoadStudentsWithSubjects();
                        }
                    }));
            }
        }

        /// <summary>
        /// Команда для удаления базы данных.
        /// </summary>
        public RelayCommand DeleteDatabaseCommand
        {
            get
            {
                return _deleteDatabaseCommand ??
                    (_deleteDatabaseCommand = new RelayCommand(obj =>
                    {
                        if (db.Database.CanConnect())
                        {
                            StudentsWithSubjects.Clear();
                            db.Database.EnsureDeleted();
                            MessageBox.Show("База данных была удалена");
                        }
                        else
                        {
                            MessageBox.Show("База данных не существует");
                        }
                    }));
            }
        }

        /// <summary>
        /// Команда для сортировки данных в алфавитном порядке.
        /// </summary>
        public RelayCommand SortCommand
        {
            get
            {
                return _sortCommand ?? (_sortCommand = new RelayCommand(parameter =>
                {
                    var sortField = parameter as string;
                    if (string.IsNullOrEmpty(sortField)) return;

                    switch (sortField)
                    {
                        case "Nomer":
                            StudentsWithSubjects = new ObservableCollection<Student>(StudentsWithSubjects.OrderBy(s => s.Nomer));
                            break;
                        case "Surname":
                            StudentsWithSubjects = new ObservableCollection<Student>(StudentsWithSubjects.OrderBy(s => s.Surname));
                            break;
                        case "Name":
                            StudentsWithSubjects = new ObservableCollection<Student>(StudentsWithSubjects.OrderBy(s => s.Name));
                            break;
                        case "Patronymic":
                            StudentsWithSubjects = new ObservableCollection<Student>(StudentsWithSubjects.OrderBy(s => s.Patronymic));
                            break;
                        case "Semester":
                            StudentsWithSubjects = new ObservableCollection<Student>(StudentsWithSubjects.OrderBy(s => s.Semester));
                            break;
                        case "Department":
                            StudentsWithSubjects = new ObservableCollection<Student>(StudentsWithSubjects.OrderBy(s => s.Department));
                            break;
                        case "Speciality":
                            StudentsWithSubjects = new ObservableCollection<Student>(StudentsWithSubjects.OrderBy(s => s.Speciality));
                            break;
                        default:
                            break;
                    }
                    OnPropertyChanged(nameof(StudentsWithSubjects));
                }));
            }
        }

        /// <summary>
        /// Команда для открытия нового окна с информацией об экзаменах.
        /// </summary>
        public RelayCommand ShowExams
        {
            get
            {
                return _showExams ??
                    (_showExams = new RelayCommand((selectedItem) =>
                    {
                        Student? student = selectedItem as Student;
                        if (student == null) return;

                        Exams examsWindow = new Exams(db, student);
                        examsWindow.ShowDialog();

                    }));
            }
        }

        /// <summary>
        /// Команда для экспорта данных в json формате.
        /// </summary>
        public RelayCommand ExportToJsonCommand
        {
            get
            {
                return _exportToJsonCommand ??
                  (_exportToJsonCommand = new RelayCommand((o) =>
                  {
                      if (StudentsWithSubjects.Count == 0)
                      {
                          MessageBox.Show("Нет данных для экспорта");
                      }
                      else
                      {
                          var settings = new JsonSerializerSettings
                          {
                              Formatting = Newtonsoft.Json.Formatting.Indented,
                              ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                          };

                          string json = JsonConvert.SerializeObject(StudentsWithSubjects, settings);
                          File.WriteAllText("students.json", json);
                          MessageBox.Show("Данные добавлены в файл students.json");
                      }
                  }));
            }
        }

        /// <summary>
        /// Команда для генерации отчета об успеваемости.
        /// </summary>
        public RelayCommand GeneratePerformanceReportCommand
        {
            get
            {
                return _generatePerformanceReportCommand ??
                       (_generatePerformanceReportCommand = new RelayCommand((o) =>
                       {
                           GeneratePerformanceReport();
                       }));
            }
        }

        /// <summary>
        /// Команда для отрисовки графика.
        /// </summary>
        public RelayCommand ShowPerformanceChartCommand { get; private set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса ApplicationViewModel.
        /// </summary>
        public ApplicationViewModel()
        {
            db.Database.EnsureCreated();
            StudentsWithSubjects = new ObservableCollection<Student>();
            LoadStudentsWithSubjects();
            Criteria = new ObservableCollection<string> { "Факультет", "Семестр", "Специальность" };
            Formatter = value => value.ToString("N1"); // Простое числовое форматирование
            ShowPerformanceChartCommand = new RelayCommand(param => LoadChartData());
            this.ChartSeries = new SeriesCollection();
            this.AxisYStep = 0.5;
        }

        /// <summary>
        /// Функция для загрузки данных из бд.
        /// </summary>
        private void LoadStudentsWithSubjects()
        {
            StudentsWithSubjects.Clear();
            var studentsWithSubjects = db.Students.Include(s => s.Subjects).ToList();
            foreach (var student in studentsWithSubjects)
            {
                StudentsWithSubjects.Add(student);
            }
        }

        /// <summary>
        /// Функция для подсчета средней оценки по категорям и добавления их на график.
        /// </summary>
        private void LoadChartData()
        {
            if (SelectedCriteria == null)
                return;

            var results = new List<double>();
            var labels = new List<string>();

            switch (SelectedCriteria)
            {
                case "Факультет":
                    var facultyGroups = db.Students
                                           .Include(s => s.Subjects)
                                           .GroupBy(s => s.Department)
                                           .Select(g => new
                                           {
                                               Faculty = g.Key,
                                               AverageScore = g.Average(s => s.Subjects.Average(sub => sub.Grade))
                                           }).ToList();
                    results.AddRange(facultyGroups.Select(x => x.AverageScore));
                    labels.AddRange(facultyGroups.Select(x => x.Faculty));
                    break;

                case "Семестр":
                    var semesterGroups = db.Students
                                            .Include(s => s.Subjects)
                                            .GroupBy(s => s.Semester)
                                            .Select(g => new
                                            {
                                                Semester = g.Key,
                                                AverageScore = g.Average(s => s.Subjects.Average(sub => sub.Grade))
                                            }).ToList();
                    results.AddRange(semesterGroups.OrderBy(x => x.Semester).Select(x => x.AverageScore));
                    labels.AddRange(semesterGroups.OrderBy(x => x.Semester).Select(x => $"Семестр {x.Semester}"));
                    break;

                case "Специальность":
                    var specialtyGroups = db.Students
                                             .Include(s => s.Subjects)
                                             .GroupBy(s => s.Speciality)
                                             .Select(g => new
                                             {
                                                 Specialty = g.Key,
                                                 AverageScore = g.Average(s => s.Subjects.Average(sub => sub.Grade))
                                             }).ToList();
                    results.AddRange(specialtyGroups.Select(x => x.AverageScore));
                    labels.AddRange(specialtyGroups.Select(x => x.Specialty));
                    break;
            }

            // Обновление данных для диаграммы
            ChartSeries = new SeriesCollection
    {
        new ColumnSeries
        {
            Title = "Средняя оценка",
            Values = new ChartValues<double>(results)
        }
    };

            AxisLabels = labels.ToArray();
            OnPropertyChanged(nameof(ChartSeries));
            OnPropertyChanged(nameof(AxisLabels));
        }

        /// <summary>
        /// Функция для поиска студентов по введенному слову.
        /// </summary>
        private void FilterStudents()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                LoadStudentsWithSubjects();
            }
            else
            {
                var filtered = db.Students
                    .Include(s => s.Subjects)
                    .Where(s => s.Nomer.Contains(SearchText)
                                || s.Surname.Contains(SearchText)
                                || s.Name.Contains(SearchText)
                                || s.Patronymic.Contains(SearchText)
                                || s.Semester.ToString().Contains(SearchText)
                                || s.Department.Contains(SearchText)
                                || s.Speciality.Contains(SearchText))
                    .ToList();

                StudentsWithSubjects = new ObservableCollection<Student>(filtered);
            }
            OnPropertyChanged(nameof(StudentsWithSubjects));
        }

        /// <summary>
        /// Функция для генерации отчета.
        /// </summary>
        private void GeneratePerformanceReport()
        {
            if (StudentsWithSubjects == null || StudentsWithSubjects.Count == 0)
            {
                MessageBox.Show("Нет данных для генерации отчета.");
                return;
            }

            StringBuilder reportBuilder = new StringBuilder();

            reportBuilder.AppendLine("Отчет об успеваемости студентов");
            reportBuilder.AppendLine(new string('-', 40));

            foreach (var student in StudentsWithSubjects)
            {
                double averageGrade = student.Subjects.Any() ? student.Subjects.Average(sub => sub.Grade) : 0.0;
                reportBuilder.AppendLine($"Студент: {student.Surname} {student.Name} {student.Patronymic} Факультет: {student.Department} Специальность: {student.Speciality}");
                reportBuilder.AppendLine("Предметы и оценки:");

                foreach (var subject in student.Subjects)
                {
                    reportBuilder.AppendLine($"{subject.Name}: {subject.Grade}");
                }

                reportBuilder.AppendLine($"Средняя оценка: {averageGrade.ToString("N1")}");
                reportBuilder.AppendLine(new string('-', 40));
            }

            var departmentGroups = StudentsWithSubjects.GroupBy(s => s.Department);
            foreach (var departmentGroup in departmentGroups)
            {
                double departmentAverageGrade = departmentGroup.Average(s => s.Subjects.Any() ? s.Subjects.Average(sub => sub.Grade) : 0.0);
                reportBuilder.AppendLine($"Средняя оценка на факультете {departmentGroup.Key}: {departmentAverageGrade.ToString("N1")}");
            }

            reportBuilder.AppendLine(new string('-', 40));

            var specialityGroups = StudentsWithSubjects.GroupBy(s => s.Speciality);
            foreach (var specialityGroup in specialityGroups)
            {
                double specialityAverageGrade = specialityGroup.Average(s => s.Subjects.Any() ? s.Subjects.Average(sub => sub.Grade) : 0.0);
                reportBuilder.AppendLine($"Средняя оценка на специальности {specialityGroup.Key}: {specialityAverageGrade.ToString("N1")}");
            }

            string fileName = "PerformanceReport.txt";
            try
            {
                File.WriteAllText(fileName, reportBuilder.ToString());
                MessageBox.Show($"Отчет успешно сгенерирован и сохранен в файл {fileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении отчета: {ex.Message}");
            }
        }

        /// <summary>
        /// Событие для отслеживания изменений свойств.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Функция для вызова события.
        /// </summary>
        /// <param name="propertyName">Имя измененного свойства.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
