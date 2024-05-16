using System;
using System.Windows;

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

        public void InitializeProfile(string login, string name, string surname, string patronymic, int role_id, int user_id)
        {
            userLogin.Content += login;
            userName.Content += name;
            userSurname.Content += surname;
            userPatronymic.Content += patronymic;
            switch (role_id)
            {
                case 1:
                    userRole.Content += "Администратор";
                    break;
                case 2:
                    userRole.Content += "Официант";
                    break;
                case 3:
                    userRole.Content += "Повар";
                    break;
            }

            userId.Content += user_id.ToString();
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
