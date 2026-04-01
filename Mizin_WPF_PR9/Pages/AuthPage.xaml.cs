using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mizin_WPF_PR9.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

                // ========== Обработчик изменения текста в поле Логин (для плейсхолдера) ==========
        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (WatermarkLogin != null)
            {
                WatermarkLogin.Visibility = string.IsNullOrEmpty(TextBoxLogin.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        // ========== Обработчик изменения пароля (для плейсхолдера) ==========
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (WatermarkPassword != null)
            {
                WatermarkPassword.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        // ========== РЕФАКТОРИНГ: Метод авторизации (будет вызываться из тестов) ==========
        public bool Auth(string login, string password)
        {
            // Проверка заполнения полей
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            using (var db = new Mizin_2ЗISIP1624_PR9_Entities())
            {
                // Поиск пользователя (логин — регистронезависимый)
                var user = db.User.FirstOrDefault(u =>
                    u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    MessageBox.Show("Пользователь не найден!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // Проверка пароля (регистрозависимый!)
                if (user.Password != password)
                {
                    MessageBox.Show("Неверный пароль!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // Проверка на удаленного пользователя
                if (user.Role == "Удален")
                {
                    MessageBox.Show("Пользователь удален!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // Успешная авторизация
                MessageBox.Show($"Здравствуйте, {user.Role} {user.FIO}!",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                return true;
            }
        }

        // ========== Обработчик кнопки "Вход" ==========
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            // Вызываем рефакторенный метод
            bool result = Auth(TextBoxLogin.Text, PasswordBox.Password);

            if (result)
            {
                // Здесь можно добавить переход на главную страницу
                // NavigationService.Navigate(new MainPage());
            }
        }

        // ========== Обработчик кнопки "Регистрация" ==========
        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }
    }
}