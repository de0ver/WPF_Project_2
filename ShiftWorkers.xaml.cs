using Global;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for ShiftWorkers.xaml
    /// </summary>
    public partial class ShiftWorkers : Window
    {
        Globals globals = new Globals();
        public List<int> workersId = new List<int>();
        private bool skip_close = false;

        public ShiftWorkers()
        {
            InitializeComponent();
            GetWorkers();
        }

        private void GetWorkers()
        {
            var request = new RestRequest(globals.getUsersURL, method: Method.Get);
            request.AddHeader("Authorization", "Bearer " + Globals.userToken);

            try
            {
                var response = globals.client.Execute(request);

                var json = JsonSerializer.Deserialize<Globals.JSONUser>(response.Content);

                for (int i = 0; i < json.user.Count; i++)
                {
                    WorkersList.Items.Add("ID: " + json.user[i].id + " | Name: " + json.user[i].name + " | Status: " + json.user[i].status + " | Group: " + json.user[i].group);
                    workersId.Add(int.Parse(json.user[i].id.ToString())); //trash
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void Clear()
        {
            WorkersList.Items.Clear();
            workersId = new List<int>();
            GetWorkers();
        }

        private void DeleteWorker(object sender, RoutedEventArgs e)
        {
            if (WorkersList.SelectedIndex >= 0)
            {
                var request = new RestRequest(globals.getUsersURL + "/" + workersId[WorkersList.SelectedIndex], method: Method.Delete);

                request.AddHeader("Authorization", "Bearer " + Globals.userToken);

                try
                {
                    var response = globals.client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        var json = JsonSerializer.Deserialize<Globals.HTTPMessageCreated>(response.Content);
                        MessageBox.Show("ID: " + json.data.id + "\nStatus: " + json.data.status);
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.ToString());
                }
            }

            Clear();
        }

        private void ReturnWorker(object sender, RoutedEventArgs e)
        {
            var request = new RestRequest(globals.getUsersURL + "/" + workersId[WorkersList.SelectedIndex] + "/back", method: Method.Delete);
            request.AddHeader("Authorization", "Bearer " + Globals.userToken);

            try
            {
                var response = globals.client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var json = JsonSerializer.Deserialize<Globals.HTTPMessageCreated>(response.Content);
                    MessageBox.Show("ID: " + json.data.id + "\nStatus: " + json.data.status);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }

            Clear();
        }

        private void AddWorker(object sender, RoutedEventArgs e)
        {
            var request = new RestRequest(globals.getUsersURL, method: Method.Post);

            request.AddHeader("Authorization", "Bearer " + Globals.userToken);
            request.AddParameter("name", inputName.Text);
            request.AddParameter("surname", inputSurname.Text);
            request.AddParameter("patronymic", inputPatronymic.Text);
            request.AddParameter("login", inputLogin.Text);
            request.AddParameter("password", inputPassword.Password);
            request.AddParameter("role_id", inputRole.SelectedIndex + 1);

            try
            {
                var response = globals.client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var json = JsonSerializer.Deserialize<Globals.HTTPMessageCreated>(response.Content);
                    MessageBox.Show("ID: " + json.data.id + "\nStatus: " + json.data.status);

                    inputName.Text = inputSurname.Text = inputPatronymic.Text = inputLogin.Text = inputPassword.Password = inputPasswordConfirm.Password = null;
                    inputRole.SelectedIndex = -1;

                    Clear();
                }
                else
                {
                    var json = JsonSerializer.Deserialize<Globals.HTTPMessageError>(response.Content);
                    if (json.error.errors != null)
                    {
                        string big_message = "Code: " + json.error.code + "\nMessage: " + json.error.message;

                        if (json.error.errors.name != null)
                            big_message += "\nName: " + json.error.errors?.name[0];

                        if (json.error.errors.surname != null)
                            big_message += "\nSurname: " + json.error.errors?.surname[0];

                        if (json.error.errors.patronymic != null)
                            big_message += "\nPatronymic: " + json.error.errors?.patronymic[0];

                        if (json.error.errors.login != null)
                            big_message += "\nLogin: " + json.error.errors?.login[0];

                        if (json.error.errors.password != null)
                            big_message += "\nPassword: " + json.error.errors?.password[0];

                        if (json.error.errors.role_id != null)
                            big_message += "\nRole: " + json.error.errors?.role_id[0];

                        MessageBox.Show(big_message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("Code: " + json.error.code + "\nMessage: " + json.error.message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void CheckInputs(object sender, TextChangedEventArgs e)
        {
            checkName.IsChecked = inputName.Text.Length >= 3;
            checkSurname.IsChecked = inputSurname.Text.Length >= 3;
            checkPatronymic.IsChecked = inputPatronymic.Text.Length >= 3;
            checkLogin.IsChecked = inputLogin.Text.Length >= 3;
        }

        private void CheckPasswords(object sender, RoutedEventArgs e)
        {
            checkPassword.IsChecked = inputPassword.Password.Length >= 4;
            checkPasswordConf.IsChecked = inputPassword.Password == inputPasswordConfirm.Password;
        }

        private void inputRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            checkRole.IsChecked = inputRole.SelectedIndex >= 0;
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            //1 million windows is ready, another one is on the way
            skip_close = true;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (!skip_close)
                Application.Current.Shutdown();
        }
    }
}
