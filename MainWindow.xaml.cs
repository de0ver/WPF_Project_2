using Global;
using System;
using System.Text.Json;
using System.Windows;
using RestSharp;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Globals globals = new Globals();
        Globals.JSONUser user = new Globals.JSONUser();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LogInAsync(object sender, RoutedEventArgs e)
        {
            var request = new RestRequest(globals.localURL, method: Method.Post);
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

                        var json = JsonSerializer.Deserialize<Globals.HTTPMessageOK>(response.Content);

                        Globals.userToken = json.data.user_token;

                        /*Globals.JSONUser.User fix = new Globals.JSONUser.User();
                        fix.id = json.data.user_id;
                        fix.login = json.data.login;
                        fix.name = json.data.name;
                        switch (json.data.role_id)
                        {
                            case 1:
                                fix.group = "Администратор";
                                break;
                            case 2:
                                fix.group = "Официант";
                                break;
                            case 3:
                                fix.group = "Повар";
                                break;
                        }
                        fix.status = "working";

                        user.user.Add(fix);*/

                        //adminPage.InitializeProfile(user);

                        adminPage.Show();

                        Hide();
                    }
                    else
                    {
                        var json = JsonSerializer.Deserialize<Globals.HTTPMessageError>(response.Content);
                        if (json.error.errors != null)
                        {
                            if (json.error.errors.login != null && json.error.errors.password != null)
                            {
                                MessageBox.Show("Code: " + json.error.code + "\nMessage: " + json.error.message + "\nLogin: " + json.error.errors?.login[0] + "\nPassword: " + json.error.errors?.password[0], "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else if (json.error.errors.login == null && json.error.errors.password != null)
                            {
                                MessageBox.Show("Code: " + json.error.code + "\nMessage: " + json.error.message + "\nPassword: " + json.error.errors?.password[0], "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                MessageBox.Show("Code: " + json.error.code + "\nMessage: " + json.error.message + "\nLogin: " + json.error.errors?.login[0], "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        } else
                        {
                            MessageBox.Show("Code: " + json.error.code + "\nMessage: " + json.error.message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
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
