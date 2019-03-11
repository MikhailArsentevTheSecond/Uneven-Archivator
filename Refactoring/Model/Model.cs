using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public static class Collection
    {
        public static void UpdateCollection(string Path, out ObservableCollection<ShellObject> Data)
        {
            Data = new ObservableCollection<ShellObject>();
            using (var Dir = (ShellFolder)ShellObject.FromParsingName(Path))
            {
                foreach (var file in Dir)
                {
                    Data.Add(file);
                }
            }
        }
    }

}

