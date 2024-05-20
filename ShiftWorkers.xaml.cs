using Global;
using System;
using System.Windows;
using RestSharp;
using System.Text.Json;
using System.Collections.Generic;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for ShiftWorkers.xaml
    /// </summary>
    public partial class ShiftWorkers : Window
    {
        Globals globals = new Globals();
        public List<int> workersId = new List<int>();

        public ShiftWorkers()
        {
            InitializeComponent();
            GetWorkers();
        }

        public void GetWorkers()
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
            } catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        public void Clear()
        {
            WorkersList.Items.Clear();
            workersId = new List<int>();
            GetWorkers();
        }

        public void DeleteWorker(object sender, RoutedEventArgs e)
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
                } catch (Exception error)
                {
                    MessageBox.Show(error.ToString());
                }
            }

            Clear();
        }

        public void ReturnWorker(object sender, RoutedEventArgs e)
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
            } catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }

            Clear();
        }

        public void AddWorker(object sender, RoutedEventArgs e)
        {
            if (CheckInputs())
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
                        }
                        else
                        {
                            MessageBox.Show("Code: " + json.error.code + "\nMessage: " + json.error.message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }                    
                } catch (Exception error)
                {
                    MessageBox.Show(error.ToString());
                }
            }
        }

        public bool CheckInputs()
        {
            checkName.IsChecked = inputName.Text.Length >= 3;
            checkSurname.IsChecked = inputSurname.Text.Length >= 3;
            checkPatronymic.IsChecked = inputPatronymic.Text.Length >= 3;
            checkLogin.IsChecked = inputLogin.Text.Length >= 3;
            checkPassword.IsChecked = inputPassword.Password.Length >= 4;
            checkPasswordConf.IsChecked = inputPassword.Password == inputPasswordConfirm.Password;
            checkRole.IsChecked = inputRole.SelectedIndex >= 0;
            //good or bad???
            if (checkName.IsChecked == true && checkSurname.IsChecked == true && checkPatronymic.IsChecked == true && checkLogin.IsChecked == true && checkPasswordConf.IsChecked == true && checkRole.IsChecked == true)
            {
                return true;
            }

            return false;
        }

        public void Back(object sender, RoutedEventArgs e)
        {
            Hide();
            new AdminPage().Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
