using Global;
using System;
using System.Text.Json;
using System.Windows;
using RestSharp;

namespace WPFApp
{
    public class HTTPMessageOK
    {
        public Data data { get; set; }
        public class Data
        {
            public string user_token { get; set; }
            public int role_id { get; set; }
            public string name { get; set; }
            public string surname { get; set; }
            public string patronymic { get; set; }
            public string login { get; set; }
            public int user_id { get; set; }
        }
    }

    public class HTTPMessageError
    {
        public Error error { get; set; }
        public class Error
        {
            public int code { get; set; }
            public string message { get; set; }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Globals globals = new Globals();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LogInAsync(object sender, RoutedEventArgs e)
        {
            var request = new RestRequest(globals.localUrl, method: Method.Post);
            request.AddHeader("Content-Type", "application/json");

            var body = "{\n    \"login\": \"" + userLogin.Text + "\",\n    \"password\": \"" + userPassword.Password + "\"\n}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            try
            {
                var response = globals.client.ExecutePost(request);

                if (response != null)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        AdminPage adminPage = new AdminPage();

                        var json = JsonSerializer.Deserialize<HTTPMessageOK>(response.Content);

                        Globals.userToken = json.data.user_token;

                        adminPage.InitializeProfile(json.data.login, json.data.name, json.data.surname, json.data.patronymic, json.data.role_id, json.data.user_id);

                        adminPage.Show();

                        Hide();
                    }
                    else
                    {
                        var json = JsonSerializer.Deserialize<HTTPMessageError>(response.Content);

                        MessageBox.Show(this, "Code: " + json.error.code + "\nMessage: " + json.error.message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
