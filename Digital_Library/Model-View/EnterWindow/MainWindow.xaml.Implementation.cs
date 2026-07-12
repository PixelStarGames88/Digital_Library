using Digital_Library.Models;
using System.Windows;
using System.Windows.Controls;

namespace Digital_Library;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private void Login_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = Person_Choice.SelectedItem as ComboBoxItem;
        if (selectedItem == null)
        {
            MessageBox.Show("Необходимо выбрать пользователя");
            return;
        }

        string role = selectedItem.Content.ToString() ?? throw new NullReferenceException();
        string password = Input_Password.Password;

        if (AccessManager.CheckPassword(role, password))
        {
            try
            {
                var permissions = AccessManager.GetPermissions(role);

                MainAppWindow nextWindow = new MainAppWindow(role, permissions);
                nextWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при открытии приложения: " + ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        else
        {
            MessageBox.Show("Неверный пароль");
        }
    }
    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void Info_Click(object sender, RoutedEventArgs e)
    {
        string infoText = "Цифровая библиотека учебно-методических изданий\n" +
                          "Программа разработана для выбора учебно-методических изданий кафедры систем автоматизированного проектирования и управления" +
        "\nРазработчики: \nВасянин И.А.\n" +
                          "Клещева П.А.\n" +
                          "Красова Ю.Р.\n" +
                          "Жидких В.М.\n";

        MessageBox.Show(infoText, "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
