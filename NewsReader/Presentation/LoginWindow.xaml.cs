using System.Windows;
using System.Windows.Controls;


namespace NewsReader
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private IniFile _iniFile;

        public LoginWindow()
        {
            InitializeComponent();
            _iniFile = new IniFile();
            LoadSettings();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Save the values from textboxes to the .INI file
            _iniFile.Write("Server", ServerTextBox.Text);
            _iniFile.Write("Port", PortTextBox.Text);
            _iniFile.Write("Username", UsernameTextBox.Text);
            _iniFile.Write("Password", PasswordBox.Password);


            MessageBox.Show("Saved.");
            this.Close();
        }

        private void LoadSettings()
        {
            // Read the values from the .INI file and set them in the textboxes
            ServerTextBox.Text = _iniFile.Read("Server") ?? "";
            PortTextBox.Text = _iniFile.Read("Port") ?? "";
            UsernameTextBox.Text = _iniFile.Read("Username") ?? "";
            PasswordBox.Password = _iniFile.Read("Password") ?? "";
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Closing...");
            this.Close();
        }
    }
}



