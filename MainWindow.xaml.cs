﻿using Global;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LogIn(object sender, RoutedEventArgs e) //user enter to app
        {
            var request = new RestRequest(globals.localURL3, method: Method.Post);
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
                        var json = JsonSerializer.Deserialize<Globals.HTTPMessageOK>(response.Content);

                        if (json.data.role_id == 1)
                        {
                            AdminPage adminPage = new AdminPage();

                            Globals.userToken = json.data.user_token;

                            adminPage.InitializeProfile(json.data.login, json.data.name, json.data.surname, json.data.patronymic, json.data.role_id, json.data.user_id);

                            Visibility = Visibility.Collapsed;

                            adminPage.ShowDialog();

                            Visibility = Visibility.Visible;
                        } 
                        else
                        {
                            MessageBox.Show("Отказано в доступе.");
                        }
                    }
                    else
                    {
                        var json = JsonSerializer.Deserialize<Globals.HTTPMessageError>(response.Content);

                        if (json.error.errors != null)
                        {
                            string big_message = "Code: " + json.error.code + "\nMessage: " + json.error.message;

                            if (json.error.errors.login != null)
                                big_message += "\nLogin: " + json.error.errors.login[0];

                            if (json.error.errors.password != null)
                                big_message += "\nPassword: " + json.error.errors.password[0];

                            MessageBox.Show(big_message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
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
    }
}
