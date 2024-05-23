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
            userLogin.Text += login;
            userName.Text += name;
            userSurname.Text += surname;
            userPatronymic.Text += patronymic;
            switch (role_id)
            {
                case 1:
                    userRole.Text += "Администратор";
                    break;
                case 2:
                    userRole.Text += "Официант";
                    break;
                case 3:
                    userRole.Text += "Повар";
                    break;
            }

            userId.Text += user_id.ToString();
        }

        //switch (sender.Name) { case "btn1": break; case "btn2": break; case "btn3": break; }
        private void goToShiftWorkers(object sender, RoutedEventArgs e)
        {
            ShiftWorkers shiftWorkers = new ShiftWorkers();

            shiftWorkers.ShowDialog();
        }
        private void goToWorkShifts(object sender, RoutedEventArgs e)
        {
            WorkShifts workShifts = new WorkShifts();

            workShifts.ShowDialog();
        }
        private void goToOrders(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
