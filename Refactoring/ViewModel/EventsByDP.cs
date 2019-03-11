using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EventToCommand
{
        /* для разделения представления и логики ( в чём и есть смысл MVVM\MVC и т.д. ) 
           нужно создать прослойку между ивентом и собственно логикой.
           Создадим общий класс EventCommand который будет состоять из двух прикреплённых свойств
           Первое - это имя ивента который нужно обработать
           Второе - ссылка на команду которая производит обработку
           Идея честно скоммунизжена с 
           https://www.codeproject.com/Articles/210022/Getting-WPF-SizeChanged-Events-at-Start-up-when-Us
        */
        public class EventCommand
        {
            #region Command DP

            public static DependencyProperty CommandProperty =
               DependencyProperty.RegisterAttached("Command",
               typeof(ICommand),
               typeof(EventCommand));

            public static void SetCommand(DependencyObject target, ICommand value)
            {
                target.SetValue(EventCommand.CommandProperty, value);
            }

            public static ICommand GetCommand(DependencyObject target)
            {
                return (ICommand)target.GetValue(CommandProperty);
            }

            #endregion

            #region EventName DP

            public static DependencyProperty EventNameProperty =
               DependencyProperty.RegisterAttached("EventName",
               typeof(string),
               typeof(EventCommand),
               new PropertyMetadata(NameChanged));

            public static void SetEventName(DependencyObject target, string value)
            {
                target.SetValue(EventCommand.EventNameProperty, value);
            }

            public static string GetEventName(DependencyObject target)
            {
                return (string)target.GetValue(EventNameProperty);
            }

            #endregion

            private static void NameChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
            {
                if (target is UIElement Element)
                {
                    // для универсальности класса EventCommand подключение ивентов происходит через Рефлексию.
                    // В настоящий момент OldValue по какой-то причине всегда равна null.
                    if (e.NewValue != e.OldValue)
                    {
                        if (e.OldValue == null)
                        {
                            // Если OldValue == null , то до этого обработчик не был привязан к другим ивентам
                            // А значит нам просто нужно привязать обработчик к указанному ивенту

                            // Получаем EventInfo из переданного свойства. System.Reflection.TargetInvocationException при неправильном имени ивента.
                            EventInfo EvInfo = Element.GetType().GetEvent((string)e.NewValue);

                            // Создаём делегат для обработки ивента, используюя функцию EventCommand.EvHandler ( сведенья о нём получаем из метаданных )
                            Delegate d = Delegate.CreateDelegate(EvInfo.EventHandlerType,
                                typeof(EventCommand).GetMethod("EvHandler", BindingFlags.NonPublic | BindingFlags.Static));

                            EvInfo.AddEventHandler(Element, d);
                            Trace.WriteLine("Привязал ивент");
                        }
                        else
                        {
                            // Иначе OldValue != null, значит уже привязан Handler и надо его отвязать

                            // Сначала привязываем новый
                            EventInfo EvInfo = Element.GetType().GetEvent((string)e.NewValue);

                            // Делегат в обоих случаях один и тот же
                            Delegate d = Delegate.CreateDelegate(EvInfo.EventHandlerType,
                                typeof(EventCommand).GetMethod("EvHandler", BindingFlags.NonPublic | BindingFlags.Static));

                            EvInfo.AddEventHandler(Element, d);

                            // Новый EventInfo для того чтобы отвязать обработчик от старого ивента
                            EvInfo = Element.GetType().GetEvent((string)e.OldValue);

                            EvInfo.RemoveEventHandler(Element, d);

                            Trace.WriteLine("Отвязал ивент");
                        }
                    }
                }
            }

            // функция которая привязывается к ивенту
            static void EvHandler(object sender, EventArgs e)
            {
                // Не разобрался почему command не через command = EventCommand.GetCommand(sender as DependencyObject) - скорее всего эта операция медленнее
                UIElement element = (UIElement)sender;
                ICommand command = (ICommand)element.GetValue(EventCommand.CommandProperty);

                // передаём параметры ивента как кортеж
                var src = Tuple.Create(sender, e);

                // стандартная проверка на возможность запуска
                if (command != null && command.CanExecute(src) == true)
                    command.Execute(src);
            }
        }
}
