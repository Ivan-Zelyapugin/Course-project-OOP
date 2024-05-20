using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUniversity.Models
{
    /// <summary>
    /// Класс представляет информацию о студенте.
    /// Реализует интерфейс INotifyPropertyChanged для оповещения об изменениях свойств объекта.
    /// </summary>
    public class Student : INotifyPropertyChanged
    {
        /// <summary>
        /// Получает или задает идентификатор студента (первичный ключ).
        /// </summary>
        public int StudentId { get; set; } 

        private string _nomer;
        private string _name;
        private string _surname;
        private string _patronymic;
        private string _semester;
        private string _department;
        private string _speciality;

        /// <summary>
        /// Список предметов.
        /// </summary>
        public ObservableCollection<Subject> Subjects { get; set; } = new ObservableCollection<Subject>();

        /// <summary>
        /// Свойство получает или задает номер зачетной книжки студента.
        /// </summary>
        public string Nomer
        {
            get { return _nomer; }
            set
            {
                _nomer = value;
                OnPropertyChanged(nameof(Nomer));
            }
        }

        /// <summary>
        /// Свойство получает или задает имя студента.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {

                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Свойство получает или задает фамилию студента.
        /// </summary>
        public string Surname
        {
            get { return _surname; }
            set
            {

                _surname = value;
                OnPropertyChanged(nameof(Surname));
            }
        }

        /// <summary>
        /// Свойство получает или задает отчество студента.
        /// </summary>
        public string Patronymic
        {
            get { return _patronymic; }
            set
            {

                _patronymic = value;
                OnPropertyChanged(nameof(Patronymic));
            }
        }

        /// <summary>
        /// Свойство получает или задает семестр обучения студента.
        /// </summary>
        public string Semester
        {
            get { return _semester; }
            set
            {

                _semester = value;
                OnPropertyChanged(nameof(Semester));
            }
        }

        /// <summary>
        /// Свойство получает или задает факультет обучения студента.
        /// </summary>
        public string Department
        {
            get { return _department; }
            set
            {

                _department = value;
                OnPropertyChanged(nameof(Department));
            }
        }

        /// <summary>
        /// Свойство получает или задает специальность студента.
        /// </summary>
        public string Speciality
        {
            get { return _speciality; }
            set
            {

                _speciality = value;
                OnPropertyChanged(nameof(Speciality));
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
