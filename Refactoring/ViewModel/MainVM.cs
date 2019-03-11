using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Shell;
using System.Windows.Input;
using System.Windows.Media;

namespace EventToCommand
{
    // это DataContext. Их можно сделать несколько. Вот этот для explorer-а.
    // А можно ещё и наследованием заняться
    public class MainVM : INotifyPropertyChanged
    {
        #region Notify Property interface

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(new object(), new PropertyChangedEventArgs(PropertyName));
        }

        #endregion

        public ObservableCollection<ListItem> ListViewData
        { get; set; }

        public struct ListItem
        {
            string Name { get; }
            ImageSource Image { get; }
            string Type { get; }
            DateTime ChangeData { get; }
            ulong Size { get; }

            public ListItem(string Name,string ChangeData)
            {
                this.Name = Name;
                Image = null;
                Type = "TestFile";
                this.ChangeData = new DateTime(53225);
                Size = 53225;
            }
            /// <summary>
            /// Конструктор структуры
            /// </summary>
            /// <param name="file">ShellObject который является файлом</param>
            public ListItem(ShellObject file)
            {
                //Добавить проверку на null
                Name = file.Name;
                Image = file.Thumbnail.SmallBitmapSource;
                Type = file.Properties.System.ContentType.Value;
                ChangeData = file.Properties.System.DateModified.Value.Value;
                Size = file.Properties.System.TotalFileSize.Value.Value;
            }
        }

        // конструктор как инициализатор
        public MainVM()
        {
        }

        // теперь нужна команда которая обновит ListViewData
        
        public class UpdateCommand : ICommand
        {

            Action Command;

            public UpdateCommand(Action Run)
            {
                Command = Run;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                Command.Invoke();
            }
        }

        public UpdateCommand UpdateListComm
        {
            get
            {
                return new UpdateCommand(() =>
                {
                    Trace.WriteLine("Тестовая команда");
                });
            }
        }
        
    }
    
    // Общая реализация команды, выполняемая по нажатия на кнопку
    class DefaultCommand : ICommand
    {
        // Действие, которое выполнится при вызове функции
        private readonly Action<object> execute;

        // функция проверки возможности вызвать Action.
        private readonly Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DefaultCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
