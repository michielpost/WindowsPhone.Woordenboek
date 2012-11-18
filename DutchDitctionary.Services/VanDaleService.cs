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
using System.Collections.ObjectModel;

namespace DutchDitctionary.Services
{
    public class VanDaleService
    {
         WebClient webClient = new WebClient();

        // event declaration 
         public delegate void ReadFinishedDelegate(string result, bool succes);
         public event ReadFinishedDelegate ReadFinished;


        public VanDaleService()
        {
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
            
        }

        public void Find(string text)
        {
            webClient.DownloadStringAsync(new Uri("http://www.vandale.nl/ndc-vzs/search/freeSearch.vdw?page=1&viewCount=20&lang=nn&pattern=" + HttpUtility.UrlEncode(text)));

        }

        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string result = string.Empty;
            bool succes = false;

            if (e.Error == null && e.Result != null)
            {

                System.Xml.Linq.XElement MyXElement = System.Xml.Linq.XElement.Parse(e.Result);

                var list = (from x in MyXElement.Descendants("result")
                            select new
                            {
                                Main = x.Element("headword").Value,
                                Betekenis = x.Element("article").Value,
                            });

                // DateTime.Now.DayOfWeek == DayOfWeek.

                foreach (var item in list)
                {
                    if(!item.Betekenis.ToLower().Contains("van dale"))
                    {
                        result += "<h2>" + item.Main + "</h2>" + item.Betekenis;

                    }
                }

                if (string.IsNullOrEmpty(result))
                    result = "No results found.";
                else
                    succes = true;
               
            }
            else
            {
                result = "Unable to contact server.";
            }

            if (ReadFinished != null)
                ReadFinished(result, succes);
        }
    }
}
