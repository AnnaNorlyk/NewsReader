using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NewsReader
{
    public partial class MainWindow : Window
    {
        private INntpClient nntpClient;
        private IniFile iniFile;
        private List<int> articleNumbers = new List<int>();
        private List<string> articleHeaders = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            iniFile = new IniFile();
        }



        // Opens Setup Window
        private void Setup_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().ShowDialog();
        }



        // Closes Application
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



        // Connects using setup details
        private async void Connection_Click(object sender, RoutedEventArgs e)
        {
            await ConnectToNntpServerAsync();
        }



        // Connects to the NNTP server using setup details
        private async Task ConnectToNntpServerAsync()
        {
            if (!TryGetConnectionDetails(out string hostname, out int port, out string username, out string password))
                return; // Exit if details are missing or invalid 

            try
            {
                nntpClient = new NntpClient();
                await nntpClient.ConnectAsync(hostname, port);
                await nntpClient.AuthenticateAsync(username, password);

                var newsgroups = await nntpClient.GetNewsgroupsAsync();
                UpdateNewsgroupsList(newsgroups);
            }
            catch (Exception ex)
            {
                ShowError($"Connection error: {ex.Message}");
            }
        }



        // Validates and retrieves connection details from the INI file
        private bool TryGetConnectionDetails(out string hostname, out int port, out string username, out string password)
        {
            hostname = iniFile.Read("Server");
            string portString = iniFile.Read("Port");
            username = iniFile.Read("Username");
            password = iniFile.Read("Password");

            if (string.IsNullOrEmpty(hostname) || string.IsNullOrEmpty(portString))
            {
                ShowError("Please setup details first.");
                port = 0;
                return false;
            }

            if (!int.TryParse(portString, out port))
            {
                ShowError("Invalid port number in setup.");
                return false;
            }

            return true;
        }



        // Updates the UI with newsgroups list or shows a message if none are found
        private void UpdateNewsgroupsList(List<string> newsgroups)
        {
            if (newsgroups.Count == 0)
            {
                MessageBox.Show("No newsgroups found.");
            }
            else
            {
                NewsgroupsListBox.ItemsSource = newsgroups;
                Console.WriteLine("Newsgroups loaded.");
            }
        }



        // Loads article's content when headline is selected
        private async void HeadlinesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = HeadlinesListBox.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < articleNumbers.Count)
            {
                int articleNumber = articleNumbers[selectedIndex];
                await LoadArticleContent(articleNumber);
            }
        }



        // Loads the article content based on article number
        private async Task LoadArticleContent(int articleNumber)
        {
            try
            {
                string articleContent = await nntpClient.GetArticleAsync(articleNumber.ToString());
                ArticleContentTextBox.Text = articleContent;
            }
            catch (Exception ex)
            {
                ShowError($"Error loading article: {ex.Message}");
            }
        }



        // Loads article headers when a newsgroup is selected
        private async void NewsgroupsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NewsgroupsListBox.SelectedItem is string selectedNewsgroup)

            {
                // Clear the current article headers and content before loading new ones. Added post hand-in
                HeadlinesListBox.ItemsSource = null;
                articleHeaders.Clear();
                articleNumbers.Clear();
                ArticleContentTextBox.Clear();

                await LoadArticleHeaders(selectedNewsgroup);
            }
        }



        // Retrieves article headers for the specified newsgroup
        private async Task LoadArticleHeaders(string newsgroup)
        {
            try
            {
                var headers = await nntpClient.GetArticleHeadersAsync(newsgroup);

                articleNumbers.Clear();
                articleHeaders.Clear();

                // Iterates over headers, separates and stores
                foreach (var (articleNumber, subject) in headers)
                {
                    articleNumbers.Add(articleNumber);
                    articleHeaders.Add(subject);
                }

                // Sets the list of article headers as the data source for HeadlinesListBox
                HeadlinesListBox.ItemsSource = articleHeaders;
                ArticleContentTextBox.Clear();
            }
            catch (Exception ex)
            {
                ShowError($"Error loading articles: {ex.Message}");
            }
        }

        // Shows an error message to the user
        private void ShowError(string message)
        {
            MessageBox.Show(message);
            Console.WriteLine(message);
        }

        // Disconnects from the NNTP server when the window is closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            nntpClient?.Disconnect();
        }
    }
}
