using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUniversity.Models
{
    /// <summary>
    /// Класс представляет информацию о предмете.
    /// Реализует интерфейс INotifyPropertyChanged для оповещения об изменениях свойств объекта.
    /// </summary>
    public class Subject : INotifyPropertyChanged
    {
        /// <summary>
        /// Получает или задает идентификатор предмета (первичный ключ).
        /// </summary>
        public int SubjectId { get; set; } 

        private string _name;
        private int _grade;

        public int StudentId { get; set; }
        public Student Student { get; set; }

        /// <summary>
        /// Свойство получает или задает название предмета.
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
        /// Свойство получает или задает оценку за предмет.
        /// </summary>
        public int Grade
        {
            get { return _grade; }
            set
            {
                _grade = value;
                OnPropertyChanged(nameof(Grade));
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
