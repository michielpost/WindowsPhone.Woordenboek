using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using DutchDitctionary.Services;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using DutchDictionary.Resources;
using Microsoft.Phone.Tasks;

namespace DutchDictionary
{
    public partial class MainPage : PhoneApplicationPage
    {
        VanDaleService _vanDaleService = new VanDaleService();

        private string styles = "/* Result styles */.pnn4web_a {	background-color: rgb(255, 255, 255);	color: rgb(0, 0, 0);	font-family: \"Times New Roman\", serif;	margin-left: 0.3em;}"
        + ".pnn4web_b {}"
        + ".pnn4web_c {}"
        + ".pnn4web_d {	color: #CC0033;	/*font-size: 1.25em;*/	font-weight: bold;}"
        + ".pnn4web_e {	color: #CC0033;	padding-left: 0.8em;	font-weight: bold;}"
        + ".pnn4web_f {	/*font-size: 0.7em;*/	vertical-align: -5%;}"
        + ".pnn4web_g {	/*font-size: 0.7em;*/	vertical-align: 30%;}"
        + ".pnn4web_h {	color: #CC0033;	font-weight: 700;	/*font-size: 0.7em;*/	vertical-align: 30%;}"
        + ".pnn4web_i {	color:#F8F2D4;	border:none;	width:5px;	height:10px;	float:left;	display:block;	/*font-size:0px !important;*/	line-height:0px !important;	}"
        + " /* removed for indent 	display: none;	visibility: hidden;	font-weight: 700;	font-size: 0.4em;	vertical-align: 30%;*/"
        + ".pnn4web_j {	/*font-size: 0.5em;*/	vertical-align: 30%;}"
        + ".pnn4web_k {	font-style: oblique;}"
        + ".pnn4web_l {	font-style: normal;}"
        + ".pnn4web_m {	text-decoration: underline;}"
        + ".pnn4web_n {	/*font-size: 0.85em;*/	vertical-align: 4%;}"
        + ".pnn4web_o {	border-color: #FCA37E;	background-color: #FFFFE0;}"
        + ".pnn4web_p {	background-color: #FDC19F;}"
        + ".pnn4web_b  ::-moz-selection {	background: #FEE0BF;}"
        + ".pnn4web_b  ::selection {	background: #FEE0BF;}";

        string BackgroundColor = "Black";
        string ForegroundColor = "White";
        string lastWord;
        SearchWord searchWord;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.Beoordeel;
            (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Text = AppResources.about;


            webBrowser1.Loaded += new RoutedEventHandler(webBrowser1_Loaded);
            webBrowser1.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(webBrowser1_LoadCompleted);

           
             var bgc = App.Current.Resources["PhoneBackgroundColor"].ToString();
             if (bgc != "#FF000000")
             {
                 BackgroundColor = "White";
                 ForegroundColor = "Black";
             }

            

             ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
             b.Text = AppResources.HistoryMenuButton;

             this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (searchWord == null)
                InputTextBox.Focus();
        }

        // When page is navigated to, set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _vanDaleService.ReadFinished += new VanDaleService.ReadFinishedDelegate(VanDaleService_ReadFinished);

            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                int index = int.Parse(selectedIndex);

                var historyList = HistoryService.GetHistory();
                if (index < historyList.Count)
                {
                    searchWord = historyList[index];

                    
                }


            }

          
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _vanDaleService.ReadFinished -= new VanDaleService.ReadFinishedDelegate(VanDaleService_ReadFinished);
        }

        void webBrowser1_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            webBrowser1.Visibility = System.Windows.Visibility.Visible;

            if (searchWord != null)
            {
                VanDaleService_ReadFinished(searchWord.Description, false);
                searchWord = null;
            }
        }

        void webBrowser1_Loaded(object sender, RoutedEventArgs e)
        {
             webBrowser1.NavigateToString(string.Format("<html><head></head><body {0}></body></html>", GetBodyStyle()));
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            if (InputTextBox.Text.Trim().Length > 0)
            {
                webBrowser1.Visibility = System.Windows.Visibility.Collapsed;
                webBrowser1.NavigateToString(string.Format("<html><head></head><body {0}><b>Searching...</b></body></html>", GetBodyStyle()));

                lastWord = InputTextBox.Text.Trim();

                var word = HistoryService.GetHistory().Where(x => x.Word == lastWord).FirstOrDefault();

                if (word == null)
                    _vanDaleService.Find(lastWord);
                else
                {
                    ShowResult(word.Description);
                }


                InputTextBox.Text = string.Empty;
            }

            SearchButton.Focus();
        }

        void VanDaleService_ReadFinished(string result, bool succes)
        {
            if(succes)
                HistoryService.SaveSearchWord(new SearchWord { Word = lastWord, Description = result });

            ShowResult(result);
        }

        private void ShowResult(string result)
        {
            webBrowser1.Visibility = System.Windows.Visibility.Collapsed;
            webBrowser1.NavigateToString(string.Format("<html><head><style>{0}</style></head><body {1}>{2}</body></html>", styles, GetBodyStyle(), result));
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                
                Search();
            }
        }

        private string GetBodyStyle()
        {
            return "style='background-color:" + BackgroundColor + "; color: " + ForegroundColor + "'";
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));

        }

        private void HistoryMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/HistoryPage.xaml", UriKind.Relative));
        }

        private void ReviewMenuItem_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
    }
}