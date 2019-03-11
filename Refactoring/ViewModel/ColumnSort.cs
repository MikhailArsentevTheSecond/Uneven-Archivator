using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Diagnostics;

// AutoSort from this blog http://www.thomaslevesque.com/2009/08/04/wpf-automatically-sort-a-gridview-continued/

// переименовать
namespace UnevenArchivatorMVVM.Sorting
{
    public class ListViewSort
    {
        //Прикрепляемое свойство, которое привязывает click handler к GridViewColumnHeader
        #region Sort Property

        public static bool GetSort(DependencyObject obj)
        {
            return (bool)obj.GetValue(SortProperty);
        }

        public static void SetSort(DependencyObject obj, bool value)
        {
            obj.SetValue(SortProperty, value);
        }

        // При наличии свойства Sort вызывает SortPropertyChangedCallback, который добавляет обработчик клика по заголовку колонны
        public static readonly DependencyProperty SortProperty =
            DependencyProperty.RegisterAttached("Sort",
                typeof(bool),
                typeof(ListViewSort), new UIPropertyMetadata(false, SortPropertyChangedCallBack));

       // Избыточный регион ?
        #region Sort Logic

        /// <summary>
        /// Вызывается при каждом изменении свойства Sort
        /// </summary>
        /// <param name="o">Объект к которому прикреплено свойство</param>
        /// <param name="e">Значение свойства</param>
        private static void SortPropertyChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            //DependencyPropertyChanged
            // Проверка на то что DependencyObject является ListView. Иначе говоря, что Sort свойство применено к ListView
            if (o is ListView List)
            {
                // конструкция из оригинала проверяет изменилось ли свойство. То есть if e.NewValue != e.OldValue (записано в оптимизированной форме if ( NewValue && !OldValue )
                if ((bool)e.NewValue)
                {
                    List.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(SortClickHandler));
                }
                else
                {
                    List.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(SortClickHandler));
                }
            }
            else
            {
                throw new InvalidEnumArgumentException("поддерживаются только элемент управления ListView");
            }
        }

        private static void SortClickHandler(object sender, RoutedEventArgs e)
        {
            
            //Проверки на null ?
            ListView list = sender as ListView;
            if (e.OriginalSource is GridViewColumnHeader HeaderClicked && HeaderClicked.Column != null)
            {
                ApplySort(list.Items, GetSortName(HeaderClicked.Column), list, HeaderClicked);
            }
        }

        private static void ApplySort(ICollectionView view, string propertyName, ListView listView, GridViewColumnHeader sortedColumnHeader)
        {
            // направление сортировки по умолчанию. 
            ListSortDirection direction = ListSortDirection.Descending;
            if (view.SortDescriptions.Count > 0)
            {
                SortDescription currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                {
                    if (currentSort.Direction == ListSortDirection.Ascending)
                        direction = ListSortDirection.Descending;
                    else
                        direction = ListSortDirection.Ascending;
                }
                view.SortDescriptions.Clear();

            }
            if (!string.IsNullOrEmpty(propertyName))
            {
                view.SortDescriptions.Add(new SortDescription(propertyName, direction));
            }
        }

        #endregion

        #endregion

        // Прикрепляемое свойство, которое передаёт имя свойства объекта коллекции, по которому будет произведено упорядочивание.
        #region SortName Property
        public static string GetSortName(DependencyObject obj)
        {
            return (string)obj.GetValue(SortNameProperty);
        }

        public static void SetSortName(DependencyObject obj, string value)
        {
            obj.SetValue(SortNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortNameProperty =
            DependencyProperty.RegisterAttached("SortName",
                typeof(string),
                typeof(ListViewSort));

        #endregion

    }
}

