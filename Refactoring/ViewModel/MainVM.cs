using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Shell;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/* Правила комментирования :
 * Комментарий писать сверху. Если есть summary, то в него
 * Если формулировка комментария не нравится ставим восклицательный знак у начала Example: | //! ИЛИ /*!
 * Если нужно что-то допилить какую-то функцию ставим у начала комментария TODO | Example: //TODO ИЛИ /*TODO
 * Если есть какая-то идея написать здесь или пометить с помощью PSTODO | Example: //PSTODO ИЛИ /*PSTODO
 * Использовать названия визуальных элементов, указанных в комментариях xaml разметки
 * 
 * -----------------------------------------------------------------------------------------
 * TODO: Переделать Навигатор из TextBox в ComboBox
 */


namespace MainViewModel
{

    public struct ListItem
    {
        public string Name { get; }
        public ImageSource Image { get; }
        public string Type { get; }

        // Благодаря типу ulong? мы можем обрабатывать размер как папок, так и файлов.
        // Properties.System.Size - для файлов | Properties.System.TotalFileSize для папки.
        // Но чтобы не усложнять конструктор используется только первое свойство, возвращающая null 
        public DateTime? ChangeData { get; }
        public ulong? Size { get; }

        /// <summary>
        /// Конструктор для тестов
        /// </summary>
        /// <param name="n">Устанавливает свойство "Имя"</param>
        public ListItem(string n)
        {
            Name = n;
            Image = null;
            Type = "N + NULL";
            ChangeData = DateTime.Now;
            Size = 10;
        }

        /// <summary>
        /// Основной конструктор принимающий ShellObject
        /// </summary>
        /// <param name="file">Файл или папка</param>
        public ListItem(ShellObject file)
        {
            //Добавить проверку на null
            Name = file.Name;
            Image = file.Thumbnail.SmallBitmapSource;
            Type = file.Properties.System.ItemTypeText.Value;
            ChangeData = file.Properties.System.DateModified.Value;
            Size = file.Properties.System.Size.Value;
        }
    }

    public class MainVM : INotifyPropertyChanged
    {
        #region Notify Property interface

        // Ивент, который вызывает обновление данных
        // Если UpdateSource Trigger Binding-а имеет значение PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        //! метод который вызывает обновление значений в указанном имени свойства.
        private void OnPropertyChanged(string PropertyName)
        {
            Trace.WriteLine($"Property Changed Called {PropertyName}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        
        //Второй метод обновления значений. Может быть использован если внедрён в метод изменения свойства.
        private void InPropertyChange([CallerMemberName] string PropertyName = "")
        {
            Trace.WriteLine($"In Property Changed Called {PropertyName}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion

        //список который отображает проводник
        //TODO: переписать с использованием InPropertyChange. Проблема в том, что:
        /*
         * 1.) нельзя переопределить только метод изменения ( set )
         * 2.) Переопределение get затруднительно. Ссылочный тип приводит к Stack Overflow Exception. Хотя ладно, просто проверить на null через ??
         */
        public ObservableCollection<ListItem> ListViewData
        { get; set; }

        // строка поиска
        public string NavigatorString
        { get; set; }

        //Путь к папке, открытой в Navigator 
        private string Path;

        // конструктор как инициализатор
        public MainVM()
        {
            /*
             *  Пока что выбора папки по умолчанию нет. Но если бы был, то он понадобился бы здесь.
             *  Загрузка содержимого обозревателя в первый раз.
             *  Пока что по умолчанию CLSID ключ папки "Загрузки". У этого подхода минус.
             *  ShellObject загружается очень долго. Можно попробовать выполнять на заднем плане. Тогда это перестанет влиять хотя бы на время загрузки приложения.
             *  Рассматривается возможность получать путь к папке через System.IO
             */

            /* Быстрый тест
            ListViewData = new ObservableCollection<ListItem>();
            ListViewData.Add(new ListItem("А вот так работает"));
            */
            using (var Folder = (ShellFolder)ShellObject.FromParsingName("shell:::{374DE290-123F-4565-9164-39C4925E467B}"))
            {
                Path = Folder.ParsingName;
                ListViewData = UnevenArchivatorMVVM.Model.Collection.UpdateCollection(Folder);
            }
        }

        // Команда для Ивентов
        public class EventCommandHandler : ICommand
        {
            // команда принимающая вызывающий объект и аргументы.
            private Action<object, EventArgs> Command;

            // конструктор
            public EventCommandHandler(Action<object, EventArgs> Handler)
            {
                Command = Handler;
            }


            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                // пока что будем всегда возвращать true
                return true;
            }

            public void Execute(object parameter)
            {
                // Вытаскиваем переменные из картежа
                var UnTuple = parameter as Tuple<object, EventArgs>;
                Command.Invoke(UnTuple.Item1, UnTuple.Item2);
            }
        }

        // Команда для кнопок
        public class ButtonCommandHandler : ICommand
        {

            public event EventHandler CanExecuteChanged;
            Action ButtonCommand;

            public ButtonCommandHandler(Action ButtonHanlder)
            {
                ButtonCommand = ButtonHanlder;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                ButtonCommand.Invoke();
            }
        }


        private ButtonCommandHandler ToRootCommand;

        public ButtonCommandHandler ToRoot
        {
            get { return ToRootCommand ?? (ToRootCommand = new ButtonCommandHandler(ToRootEvent)); }
            set { ToRootCommand = value; }
        }
        
        // Команда перехода к корневой папке
        private void ToRootEvent()
        {
            Trace.WriteLine("Вызвана команда To Root");
            // Обновляем ListViewData с помощью метода из Model.
            //TODO нужно исправить пространства имён
            ListViewData = UnevenArchivatorMVVM.Model.Collection.UpdateCollection((ShellFolder)ShellObject.FromParsingName(Path).Parent);
            //Вызываем обновление Проводника
            //TODO:Переделать в InPropertyChange
            
            OnPropertyChanged("ListViewData");
        }

        // Команда для считывания информации из Навигатора
        private EventCommandHandler NavigateCommand;

        public EventCommandHandler Navigate
        {
            get { return NavigateCommand ?? (NavigateCommand = new EventCommandHandler(NavigatorSetEvent)); }
            set { NavigateCommand = value; }
        }

        private void NavigatorSetEvent(object sender, EventArgs e)
        {
            Trace.WriteLine("Команда NavigatorSet ");
        }


    }

    // Общая реализация команды
    //PSTODO: Возможно нужно переделать все команды на DefaultCommand. А ещё два вида TODO (PSTODO - To do Possible возможное to do )
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
