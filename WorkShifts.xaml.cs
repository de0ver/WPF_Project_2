using System;
using System.Windows;
using Global;
using RestSharp;
using System.Text.Json;
using System.Collections.Generic;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for WorkShifts.xaml
    /// </summary>
    public partial class WorkShifts : Window
    {
        Globals globals = new Globals();
        private int[] active_shift = new int[2] { -999, -999 }; //work_shift_id, active_shift_id
        private List<int> allShiftsId = new List<int>();
        public WorkShifts()
        {
            InitializeComponent();
            GetAllShifts();
        }

        private void GetAllShifts() //get all workshifts from database
        {
            var request = new RestRequest(globals.WorkShiftURL, method: Method.Get);
            request.AddHeader("Authorization", "Bearer " + Globals.userToken);

            var response = globals.client.Execute(request);

            var json = JsonSerializer.Deserialize<List<Globals.HTTPMessageShifts.MessageShifts>>(response.Content);

            for (int i = 0; i < json.Count; i++)
            {
                allWorkShifts.Items.Add("Id: " + json[i].id + "\nStart: " + json[i].start + "\nEnd: " + json[i].end + "\nActive: " + (json[i].active == 1 ? "True" : "False"));
                allShiftsId.Add(json[i].id);
                if (json[i].active == 1)
                {
                    active_shift[0] = json[i].id;
                    active_shift[1] = i;
                }
            }
        }

        private void Clear() //clear all for update data
        {
            allWorkShifts.Items.Clear();
            allWorkShifts.SelectedIndex = -1;
            allShiftsId.Clear();
            active_shift[0] = active_shift[1] = -999;
            GetAllShifts();
        }

        private void CreateWorkShift(object sender, RoutedEventArgs e) //create new workshift in database
        {
            var request = new RestRequest(globals.WorkShiftURL, method: Method.Post);

            request.AddHeader("Authorization", "Bearer " + Globals.userToken);
            request.AddHeader("Content-Type", "application/json");

            string start = "", end = "";

            if (getStartDate.SelectedDate.HasValue)
                start = getStartDate.SelectedDate.Value.ToString("yyyy-MM-dd");

            if (getEndDate.SelectedDate.HasValue)
                end = getEndDate.SelectedDate.Value.ToString("yyyy-MM-dd");

            if (getStartTime.Value.HasValue)
            {
                if (getStartTime.Text.IndexOf(':') == 1)
                {
                    start += " 0" + getStartTime.Text;
                }
                else
                {
                    start += " " + getStartTime.Text;
                }
            }

            if (getEndTime.Value.HasValue)
            {
                if (getEndTime.Text.IndexOf(':') == 1)
                {
                    end += " 0" + getEndTime.Text;
                }
                else
                {
                    end += " " + getEndTime.Text;
                }
            }

            var body = "{\n    \"start\": \"" + start + "\",\n    \"end\": \"" + end + "\"\n}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            try
            {
                var response = globals.client.ExecutePost(request);

                if (response != null)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        var json = JsonSerializer.Deserialize<Globals.HTTPMessageShifts.MessageCreateWorkShift>(response.Content);

                        MessageBox.Show("Start: " + json.start + "\nEnd: " + json.end + "\nUpdated at: " + json.updated_at + "\nCreated at: " + json.created_at + "\nId :" + json.id);
                    }
                    else
                    {
                        MessageBox.Show("Error: " + response.StatusCode + "\nStatus: " + response.StatusDescription);
                    }
                }
                else
                {
                    MessageBox.Show("Error: " + response.StatusCode + "\nStatus: " + response.StatusDescription);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }

            Clear();
        }

        private void Back(object sender, RoutedEventArgs e) //go to adminpage
        {
            Close();
        }
        
        private void CloseShift(object sender, RoutedEventArgs e) //set active 0 for selected shift in database
        {
            if (active_shift[0] != -1 && active_shift[1] != -1 && allWorkShifts.SelectedIndex != -1)
            {
                var request = new RestRequest(globals.WorkShiftURL + "/" + allShiftsId[allWorkShifts.SelectedIndex] + "/close", method: Method.Get);
                request.AddHeader("Authorization", "Bearer " + Globals.userToken);

                try
                {
                    var response = globals.client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Закрыта!");
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.ToString());
                }
            }

            Clear();
        }

        private void OpenShift(object sender, RoutedEventArgs e) //set active 1 for selected shift in database
        {
            if (active_shift[1] >= 0)
            {
                MessageBox.Show("Закройте смену " + active_shift[0] + " прежде чем открывать другую!");
            }
            else
            {
                var request = new RestRequest(globals.WorkShiftURL + "/" + allShiftsId[allWorkShifts.SelectedIndex] + "/open", method: Method.Get);
                request.AddHeader("Authorization", "Bearer " + Globals.userToken);

                try
                {
                    var response = globals.client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Открыта!");
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.ToString());
                }
            }

            Clear();
        }

        private void allWorkShifts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            currentShiftWorkers.Items.Clear();
            currentShiftOrders.Items.Clear();

            if (active_shift[1] == allWorkShifts.SelectedIndex)
            {
                CloseWorkShift.Visibility = currentShiftWorkers.Visibility = currentShiftOrders.Visibility = currentWorkersLabel.Visibility = currentOrdersLabel.Visibility = Visibility.Visible;
                OpenWorkShift.Visibility = Visibility.Hidden;

                GetWorkersFromShift(allShiftsId[allWorkShifts.SelectedIndex]);
                GetOrdersFromShift(allShiftsId[allWorkShifts.SelectedIndex]);
            }
            else
            {
                CloseWorkShift.Visibility = currentShiftWorkers.Visibility = currentShiftOrders.Visibility = currentWorkersLabel.Visibility = currentOrdersLabel.Visibility = Visibility.Hidden; 
                OpenWorkShift.Visibility = Visibility.Visible;
            }
        }

        private void GetWorkersFromShift(int shiftId) //get workers from active shift
        {
            if (shiftId >= 0)
            {
                var request = new RestRequest(globals.WorkShiftURL + "/" + allShiftsId[allWorkShifts.SelectedIndex] + "/user", method: Method.Get);
                request.AddHeader("Authorization", "Bearer " + Globals.userToken);

                try
                {
                    var response = globals.client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var json = JsonSerializer.Deserialize<List<Globals.HTTPMessageShifts.MessageWorkers>>(response.Content);

                        for (int i = 0; i < json.Count; i++)
                        {
                            currentShiftWorkers.Items.Add("Id: " + json[i].id + "\nName: " + json[i].name + "\nStatus: " + json[i].status + "\nShiftId: " + json[i].pivot.work_shift_id);
                        }
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.ToString());
                }
            }
        }

        private void GetOrdersFromShift(int shiftId) //get orders from active shift
        {
            if (shiftId >= 0)
            {
                var request = new RestRequest(globals.OrderURL + "/taken", method: Method.Get);
                request.AddHeader("Authorization", "Bearer " + Globals.userToken);

                try
                {
                    var response = globals.client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var json = JsonSerializer.Deserialize<List<Globals.HTTPMessageShifts.MessageOrders>>(response.Content);

                        for (int i = 0; i < json.Count; i++)
                        {
                            currentShiftOrders.Items.Add("Id: " + json[i].id + "\nTable: " + json[i].table + "\nStatus: " + json[i].status + "\nWorker: " + json[i].shift_workers + "\nPrice: " + json[i].price);
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
}
