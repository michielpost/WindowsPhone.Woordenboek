using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using mpost.WP7.Client.Storage;

namespace DutchDitctionary.Services
{
    public static class HistoryService
    {
        public static List<SearchWord> GetHistory()
        {
            List<SearchWord> info = IsolatedStorageCacheManager<List<SearchWord>>.Retrieve("History.xml");
            if (info == null)
                info = new List<SearchWord>();


            return info;
        }

        public static void SaveSearchWord(SearchWord word)
        {
            var list = GetHistory();
            list.Reverse();
            list.Add(word);

            list.Reverse();

            list = list.Take(50).ToList();

            SaveHistory(list);
        }

        private static void SaveHistory(List<SearchWord> info)
        {
            IsolatedStorageCacheManager<List<SearchWord>>.Store("History.xml", info);
        }




        public static void Clear()
        {
            SaveHistory(new List<SearchWord>());
        }

        public static void Delete(string word)
        {
            var list = GetHistory().Where(x => x.Word.ToLower() != word.ToLower()).ToList();

            SaveHistory(list);
        }
    }
}
