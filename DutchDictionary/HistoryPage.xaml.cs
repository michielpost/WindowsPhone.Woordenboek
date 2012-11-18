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
using System.Windows.Navigation;
using DutchDitctionary.Services;
using DutchDictionary.Resources;
using Microsoft.Phone.Shell;

namespace DutchDictionary
{
    public class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }

    public class AlwaysICommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            EventHandler unused = CanExecuteChanged;
            EventHandler<CommandEventArgs> notify = Notify;
            if (notify != null)
            {
                notify(this, new CommandEventArgs((string)parameter));
            }
        }
        public event EventHandler<CommandEventArgs> Notify;
    }

    public partial class HistoryPage : PhoneApplicationPage
    {
        public ICommand AlwaysCommand { get; private set; }

        public List<SearchWord> HList { get; set; }

        public HistoryPage()
        {
            InitializeComponent();

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.clear;

            AlwaysCommand = new AlwaysICommand();
            ((AlwaysICommand)AlwaysCommand).Notify +=new EventHandler<CommandEventArgs>(HistoryPage_Notify);

           
        }

        void HistoryPage_Notify(object sender, CommandEventArgs e)
        {
            HistoryService.Delete(e.Message);

            MainListBox.ItemsSource = HistoryService.GetHistory();
            DataContext = this;
            
        }

        // When page is navigated to, set data context 
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Set the data context of the listbox control to the sample data
            if (DataContext == null)
            {
                HList = HistoryService.GetHistory();
                DataContext = this;
            }
        }

        // Handle selection changed on ListBox
        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (MainListBox.SelectedIndex == -1)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/MainPage.xaml?selectedItem=" + MainListBox.SelectedIndex, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            MainListBox.SelectedIndex = -1;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            HistoryService.Clear();

            MainListBox.ItemsSource = HistoryService.GetHistory();
        }

        
    }
}