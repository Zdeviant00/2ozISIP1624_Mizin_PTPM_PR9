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
using System.Text.RegularExpressions;

namespace Mizin_WPF_PR9.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }
        // ========== РЕФАКТОРИНГ: Метод регистрации (будет вызываться из тестов) ==========
        public bool Register(string fio, string login, string password, string confirmPassword)
        {
            // 1. Проверка заполнения всех полей
            if (string.IsNullOrEmpty(fio) || string.IsNullOrEmpty(login) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            using (var db = new Mizin_2ЗISIP1624_PR9_Entities())
            {
                // 2. Проверка наличия логина в БД
                if (db.User.Any(u => u.Login == login))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // 3. Проверка формата пароля
                if (!IsValidPassword(password))
                {
                    return false;
                }

                // 4. Проверка совпадения паролей
                if (password != confirmPassword)
                {
                    MessageBox.Show("Пароли не совпадают!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // 5. Регистрация нового пользователя
                User newUser = new User
                {
                    FIO = fio,
                    Login = login,
                    Password = password,
                    Role = "Менеджер А" // По умолчанию
                };

                db.User.Add(newUser);
                db.SaveChanges();

                MessageBox.Show("Пользователь успешно зарегистрирован!",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
        }

        // ========== Вспомогательный метод проверки пароля ==========
        private bool IsValidPassword(string password)
        {
            // а) Минимум 6 символов
            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // б) Только английская раскладка
            Regex englishPattern = new Regex(@"^[a-zA-Z0-9!@#$%^&*(),.?""':{}|<>]+$");
            if (!englishPattern.IsMatch(password))
            {
                MessageBox.Show("Пароль должен содержать только английские символы!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // в) Хотя бы одна цифра
            if (!password.Any(char.IsDigit))
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // ========== Обработчик кнопки "Регистрация" ==========
        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            // Вызываем рефакторенный метод
            bool result = Register(
                TextBoxFIO.Text,
                TextBoxLogin.Text,
                PasswordBox.Password,
                PasswordBoxConfirm.Password
            );

            if (result)
            {
                // Возврат на страницу авторизации после успешной регистрации
                NavigationService.Navigate(new AuthPage());
            }
        }

        // ========== Обработчик кнопки "Отмена" ==========
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            // Очистка всех полей
            TextBoxFIO.Clear();
            TextBoxLogin.Clear();
            PasswordBox.Clear();
            PasswordBoxConfirm.Clear();
            TextBlockError.Visibility = Visibility.Collapsed;

            // Возврат на страницу авторизации
            NavigationService.Navigate(new AuthPage());
        }
    }
}