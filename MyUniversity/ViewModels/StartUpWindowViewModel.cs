using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyUniversity.Views;
using MyUniversity.Command;

namespace MyUniversity.ViewModels
{
    /// <summary>
    /// Модель представления для стартового окна приложения.
    /// </summary>
    class StartUpWindowViewModel
    {
        /// <summary>
        /// Команда для начала работы с приложением.
        /// </summary>
        public RelayCommand StartWorkCommand { get; private set; }

        /// <summary>
        /// Команда для выхода из приложения.
        /// </summary>
        public RelayCommand ExitCommand { get; private set; }

        /// <summary>
        /// Создает экземпляр модели представления для стартового окна.
        /// </summary>
        public StartUpWindowViewModel()
        {
            StartWorkCommand = new RelayCommand(OpenMainWindow);
            ExitCommand = new RelayCommand(CloseApplication);
        }

        /// <summary>
        /// Функция для открытия основного окна и начала работы в приложении.
        /// </summary>
        private void OpenMainWindow(object parameter)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();

            CloseCurrentWindow(parameter);
        }

        /// <summary>
        /// Функция для закрытия приложения.
        /// </summary>
        private void CloseApplication(object parameter)
        {
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// Функция для закрытия текущего окна.
        /// </summary>
        private void CloseCurrentWindow(object parameter)
        {
            var window = parameter as System.Windows.Window;
            if (window != null)
            {
                window.Close();
            }
        }
    }
}
