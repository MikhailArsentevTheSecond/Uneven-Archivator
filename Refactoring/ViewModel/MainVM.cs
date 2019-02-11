using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Shell;
using System.Windows.Input;
using System.Windows.Media;

namespace ViewModelNameSpace
{
    // это DataContext. Их можно сделать несколько. Вот этот для explorer-а.
    // А можно ещё и наследованием заняться
    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(new object(), new PropertyChangedEventArgs(PropertyName));
        }

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
            Trace.WriteLine("Вызван конструктор");
            ListViewData = new ObservableCollection<ListItem>();
            ListViewData.Add(new ListItem("Danya","Debil"));
            ListViewData.Add(new ListItem("Anub", "Zond"));
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
                    Trace.WriteLine("Команда сработала");
                    ListViewData.Add(new ListItem("Penis", "Anus"));
                });
            }
        }

        #region Пример НЕ МОЙ
        private int _number1;
        public int Number1
        {
            get { return _number1; }
            set
            {
                _number1 = value;
                OnPropertyChanged("Number3"); // уведомление View о том, что изменилась сумма
            }
        }

        private int _number2;
        public int Number2
        {
            get { return _number2; }
            set { _number2 = value; OnPropertyChanged("Number3"); }
        }

        public int Number3 => Refactoring.Model.MathFuncs.GetSumOf(_number1, _number2);

        #endregion
        
    }
    /* Data Context может быть только один.
    public class WindowVM:MainVM
    {

    }
    */
}
