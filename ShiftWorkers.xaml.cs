using Global;
using System;
using System.Windows;
using RestSharp;
using System.Text.Json;
using System.Collections.Generic;

namespace WPFApp
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string login { get; set; }
        public string status { get; set; }
        public string group { get; set; }
    }
    public class JSONData
    {
        public List<User> user { get; set; }
    }
    public class DeleteData
    { public MessageData data { get; set; } }

    public class MessageData
    {
        public int id { get; set; }
        public string status { get; set; }
    }
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
            request.AlwaysMultipartFormData = true;
            var response = globals.client.Execute(request);

            var json = JsonSerializer.Deserialize<JSONData>(response.Content);

            for (int i = 0; i < json.user.Count; i++)
            {
                WorkersList.Items.Add("ID: " + json.user[i].id + " | Name: " + json.user[i].name + " | Status: " + json.user[i].status + " | Group: " + json.user[i].group);
                workersId.Add(int.Parse(json.user[i].id.ToString())); //trash
            }
        }

        public void Clear()
        {
            WorkersList.Items.Clear();
        }

        public void DeleteWorker(object sender, RoutedEventArgs e)
        {
            var request = new RestRequest(globals.getUsersURL + "/" + workersId[WorkersList.SelectedIndex], method: Method.Delete);
            request.AddHeader("Authorization", "Bearer " + Globals.userToken);
            var response = globals.client.Execute(request);

            var json = JsonSerializer.Deserialize<DeleteData>(response.Content);

            MessageBox.Show("ID: " + json.data.id +  "\nStatus: " + json.data.status);

            Clear();
            GetWorkers();
        }

        public void ReturnWorker(object sender, RoutedEventArgs e)
        {
            var request = new RestRequest(globals.getUsersURL + "/" + workersId[WorkersList.SelectedIndex] + "/back", method: Method.Delete);
            request.AddHeader("Authorization", "Bearer " + Globals.userToken);
            var response = globals.client.Execute(request);

            var json = JsonSerializer.Deserialize<DeleteData>(response.Content);

            MessageBox.Show("ID: " + json.data.id + "\nStatus: " + json.data.status);

            Clear();
            GetWorkers();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
