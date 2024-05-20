using System;
using System.Windows;
using Global;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AdminPage : Window
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        public void InitializeProfile(Globals.JSONUser user)
        {
            userLogin.Text = user.user[0].login;
            userName.Text = user.user[0].name;
            userSurname.Text = user.user[0].login;
            userPatronymic.Text = user.user[0].login;
            userRole.Text = user.user[0].group;
            userId.Text = user.user[0].id.ToString();
        }

        //switch (sender.Name) { case "btn1": break; case "btn2": break; case "btn3": break; }
        private void goToShiftWorkers(object sender, RoutedEventArgs e)
        {
            ShiftWorkers shiftWorkers = new ShiftWorkers();

            shiftWorkers.Show();

            Hide();
        }
        private void goToWorkShifts(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void goToOrders(object sender, RoutedEventArgs e)
        {
            Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
