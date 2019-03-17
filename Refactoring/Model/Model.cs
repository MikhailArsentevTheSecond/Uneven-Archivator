using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnevenArchivatorMVVM.Model
{
    #region Прост. пример
    /// <summary>
    /// Это просто пример
    /// </summary>
    static class MathFuncs
    {
        public static int GetSumOf(int a, int b) => a + b;
    }
    #endregion

    public class Collection
    {
        // DataContext для ListView
        // что делать с типом коллекции
        
        public static ObservableCollection<MainViewModel.ListItem> UpdateCollection(string Path)
        {
            // Пока чо здесь. Попробовать где-то в другом месте. Если это вообще имеет смысл.
            ObservableCollection<MainViewModel.ListItem> ListModel = new ObservableCollection<MainViewModel.ListItem>();
            // Должны быть папки и файлы
            Trace.WriteLine(Path);
            using (var Folder =(ShellFolder)ShellFolder.FromParsingName(Path))
            {
                foreach (var item in Folder)
                {
                    ListModel.Add(new MainViewModel.ListItem(item));
                }
            }
            return ListModel;
        }

        public static ObservableCollection<MainViewModel.ListItem> UpdateCollection(ShellFolder Folder)
        {
            // Пока чо здесь. Попробовать где-то в другом месте. Если это вообще имеет смысл.
            ObservableCollection<MainViewModel.ListItem> ListModel = new ObservableCollection<MainViewModel.ListItem>();
            // Должны быть папки и файлы
            using (Folder)
            {
                foreach (var item in Folder)
                {
                    ListModel.Add(new MainViewModel.ListItem(item));
                }
            }

            return ListModel;
        }

        /// <summary>
        /// Обновление списка для тестов
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<MainViewModel.ListItem> UpdateCollection()
        {
            return new ObservableCollection<MainViewModel.ListItem>()
            {
            new MainViewModel.ListItem("РАБОТАЕТ НАКОНЕЦ-ТО"),
            new MainViewModel.ListItem("РАБОТАЕТ НАКОНЕЦ-ТО X 2")
            };
        }
    }

}

