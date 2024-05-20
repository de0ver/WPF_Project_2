using System;
using System.Windows;
using Global;
using RestSharp;
using System.Text.Json;

 
namespace WPFApp
{
    /// <summary>
    /// Interaction logic for WorkShifts.xaml
    /// </summary>
    public partial class WorkShifts : Window
    {
        Globals globals = new Globals();
        public WorkShifts()
        {
            InitializeComponent();
        }

        private void GetData()
        {

        }

        private void CreateWorkShift(object sender, RoutedEventArgs e)
        {
            var request = new RestRequest(globals.addWorkShift, method: Method.Post);

            request.AddHeader("Authorization", "Bearer " + Globals.userToken);
            request.AddHeader("Content-Type", "application/json");

            string start = "", end = "";

            if (getStartDate.SelectedDate.HasValue)
                start = getStartDate.SelectedDate.Value.ToString("yyyy-MM-dd");

            if (getEndDate.SelectedDate.HasValue)
                end = getEndDate.SelectedDate.Value.ToString("yyyy-MM-dd");

            if (getStartTime.Value.HasValue)
                start += " " + getStartTime.Text;

            if (getEndTime.Value.HasValue)
                end += " " + getEndTime.Text;

            var body = "{\n    \"start\": \"" + start + "\",\n    \"end\": \"" + end + "\"\n}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            try
            {
                var response = globals.client.ExecutePost(request);

                if (response != null)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        var json = JsonSerializer.Deserialize<Globals.HTTPMessageCreateWork>(response.Content);

                        MessageBox.Show("Start: " + json.start + "\nEnd: " + json.end + "\nUpdated at: " + json.updated_at + "\nCreated at: " + json.created_at + "\nId :" + json.id);
                    } else
                    {
                        MessageBox.Show("Error: " + response.StatusCode + "\nStatus: " + response.StatusDescription);
                    }
                } else
                {
                    MessageBox.Show("Error: " + response.StatusCode + "\nStatus: " + response.StatusDescription);
                }
            } catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
