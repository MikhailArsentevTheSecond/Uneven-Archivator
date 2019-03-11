using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowControl
{
    // Начеркал быстро класс команды для кнопок.
    class WindowCommand : ICommand
    {
        private Action Command;

        public WindowCommand(Action WindowCommand)
        {
            Command = WindowCommand;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            Command.Invoke();
        }
    }

    /// <summary>
    /// Логика взаимодействия для WindowButtons.xaml
    /// </summary>
    public partial class WindowButtons : UserControl
    {

        public WindowButtons()
        {
            InitializeComponent();
        }

        #region For Application Close | Property

        public bool IsForAppClose
        {
            get { return (bool)GetValue(IsForAppCloseProperty); }
            set { SetValue(IsForAppCloseProperty, value); }
        }

        // Свойство для создания главного окна. Закрытие его приведёт к закрытию всех окон приложения.
        public static readonly DependencyProperty IsForAppCloseProperty =
            DependencyProperty.Register("IsForAppClose", typeof(bool), typeof(WindowButtons), new PropertyMetadata(false));

        private void IsForAppCloseChangedProp(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if(IsForAppClose)
            {
                //Отписываем обработчик по умолчанию, закрывающий только окно в котором расположен WindowButtons.
                ExitButton.Click -= ExitButton_Click;
                // Подписываем обработчик, который закрывает всё приложение.
                ExitButton.Click += AppExit_Click;
            }
        }

        #endregion

        #region LightUp Color | Property
        public Brush LightUpColor
        {
            get { return (Brush)GetValue(LightUpColorProperty); }
            set { SetValue(LightUpColorProperty, value); }
        }

        // Цвет заднего фона при наведении мыши на кнопку
        public static readonly DependencyProperty LightUpColorProperty =
            DependencyProperty.Register("LightUpColor", typeof(Brush), typeof(WindowButtons), new PropertyMetadata(null));
        #endregion

        #region Exit Color | Property
        public Brush ExitColor
        {
            get { return (Brush)GetValue(ExitColorProperty); }
            set { SetValue(ExitColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExitColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExitColorProperty =
            DependencyProperty.Register("ExitColor", typeof(Brush), typeof(WindowButtons), new PropertyMetadata(null));

        #endregion

        #region Style of Minimize Button | Property
        public Style MinButtonStyle
        {
            get { return (Style)GetValue(MinButtonStyleProperty); }
            set { SetValue(MinButtonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinButtonStyleProperty =
            DependencyProperty.Register("MinButtonStyle", typeof(Style), typeof(WindowButtons), new PropertyMetadata(null));

        #endregion

        #region Style of Maximize Button | Property


        public Style MaxButtonStyle
        {
            get { return (Style)GetValue(MaxButtonStyleProperty); }
            set { SetValue(MaxButtonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxButtonStyleProperty =
            DependencyProperty.Register("MaxButtonStyle", typeof(Style), typeof(WindowButtons), new PropertyMetadata(null));


        #endregion

        #region Style of Exit Button | Property


        public Style ExitButtonStyle
        {
            get { return (Style)GetValue(ExitButtonStyleProperty); }
            set { SetValue(ExitButtonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxButtonStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExitButtonStyleProperty =
            DependencyProperty.Register("ExitButtonStyle", typeof(Style), typeof(WindowButtons), new PropertyMetadata(null));


        #endregion

        // лучше через Click или Command ?
        #region ButtonsLogic

        // Свернуть окно
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if(Window.GetWindow(this) != null)
            {
                Window.GetWindow(this).WindowState = WindowState.Minimized;
            }
        }
        // Развернуть окно на весь экран
        private void FullSizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) != null)
            {
                Window.GetWindow(this).WindowState = WindowState.Maximized;
            }
        }
        // закрыть окно в котором расположен WindowButtons
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Вызвался етот");
            if (Window.GetWindow(this) != null)
            {
                Window.GetWindow(this).Close();
            }
        }

        // закрыть все окна (приложение)
        private void AppExit_Click(object sender, RoutedEventArgs e)
        {
            if(Application.Current != null)
            {
                Application.Current.Shutdown(0);
            }
        }

        #endregion
    }
}
