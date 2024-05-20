using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyUniversity.Command
{
    /// <summary>
    /// Реализует интерфейс ICommand для создания команд, которые могут выполняться в пользовательском интерфейсе.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private Action<object> execute; 
        private Func<object, bool> canExecute; 

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; } 
            remove { CommandManager.RequerySuggested -= value; } 
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса RelayCommand.
        /// </summary>
        /// <param name="execute">Действие, которое будет выполнено при выполнении команды.</param>
        /// <param name="canExecute">Функция, определяющая, может ли команда быть выполнена в данный момент.</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute; 
            this.canExecute = canExecute; 
        }

        /// <summary>
        /// Определяет, может ли команда быть выполнена в данный момент.
        /// </summary>
        /// <param name="parameter">Параметр, используемый для определения возможности выполнения команды.</param>
        /// <returns>True, если команда может быть выполнена; в противном случае - false.</returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        /// <summary>
        /// Выполняет команду.
        /// </summary>
        /// <param name="parameter">Параметр, передаваемый в команду для выполнения.</param>
        public void Execute(object parameter)
        {
            this.execute(parameter); 
        }
    }
}
