
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MyUniversity.Views;
using MyUniversity.Command;
using MyUniversity.Models;

namespace MyUniversity.ViewModels
{
    /// <summary>
    /// Модель представления для окна экзаменов, отображающая предметы студента и позволяющая управлять ими.
    /// </summary>
    class ExamsViewModel
    {
        private Student student;
        private UniversityContext db;

        /// <summary>
        /// Получает список записей об предметах студента.
        /// </summary>
        public ObservableCollection<Subject> Subjects => student.Subjects;

        /// <summary>
        /// Создает экземпляр модели представления для окна экзаменов.
        /// </summary>
        /// <param name="db">Контекст базы данных университета.</param>
        /// <param name="student">Студент, чьи предметы отображаются в окне экзаменов.</param>
        public ExamsViewModel(UniversityContext db, Student student) 
        {
            this.student = student;
            this.db = db;
        }

        private RelayCommand _addSubjectCommand;
        private RelayCommand _deleteSubjectCommand;
        private RelayCommand _editSubjectCommand;

        /// <summary>
        /// Команда для добавления предмета студенту.
        /// </summary>
        public RelayCommand AddSubjectCommand
        {
            get
            {
                return _addSubjectCommand ??
                  (_addSubjectCommand = new RelayCommand((selectedItem) =>
                  {
                      SubjectsWindow subjectWindow = new SubjectsWindow(new Subject());

                      if (subjectWindow.ShowDialog() == true)
                      {
                          
                          Subject subject = subjectWindow.Subject;
                          if (IsSubjectAlreadyAdded(subject.Name))
                          {
                              MessageBox.Show("Такой предмет уже есть у студента.");
                              return;
                          }
                          subject.StudentId = student.StudentId;
                          db.Subjects.Add(subject);
                          db.SaveChanges();


                      }
                  }));
            }
        }

        /// <summary>
        /// Команда для удаления предмета студенту.
        /// </summary>
        public RelayCommand DeleteSubjectCommand
        {
            get
            {
                return _deleteSubjectCommand ??
                  (_deleteSubjectCommand = new RelayCommand((selectedItem) =>
                  {
                      Subject subject = selectedItem as Subject;
                      if (subject == null) return;
                      db.Subjects.Remove(subject);
                      db.SaveChanges();
                      student.Subjects.Remove(subject);
                  }));
            }
        }

        /// <summary>
        /// Команда для редактирования предмета студенту.
        /// </summary>
        public RelayCommand EditSubjectCommand
        {
            get
            {
                return _editSubjectCommand ??
                  (_editSubjectCommand = new RelayCommand((selectedItem) =>
                  {
                      Subject subject = selectedItem as Subject;
                      if (subject == null) return;

                      Subject vm = new Subject
                      {
                          Grade = subject.Grade,
                          Name = subject.Name
                      };
                      SubjectsWindow subjectWindow = new SubjectsWindow(vm);

                      if (subjectWindow.ShowDialog() == true)
                      {
                          subject.Name = subjectWindow.Subject.Name;
                          subject.Grade = subjectWindow.Subject.Grade;
                          db.SaveChanges();
                      }
                  }));
            }
        }

        /// <summary>
        /// Проверяет, добавлен ли предмет уже студенту.
        /// </summary>
        /// <param name="subjectName">Имя предмета.</param>
        /// <returns>True, если предмет уже добавлен студенту, иначе - false.</returns>
        public bool IsSubjectAlreadyAdded(string subjectName)
        {
            return Subjects.Any(e => e.Name.Equals(subjectName, StringComparison.OrdinalIgnoreCase));
        }


    }
}
