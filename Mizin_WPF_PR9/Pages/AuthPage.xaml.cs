using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Mizin_WPF_PR9.Pages
{
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        // ========== Пуьличное свойство для доступа к полю логина из CaptchaPage ==========
        public string CurrentLogin => TextBoxLogin.Text;

        // ========== РЕФАКТОРИНГ: Метод авторизации (без MessageBox для тестов) ==========
        // Этот метод вызывается из тестов - поэтому нет MessageBox.Show()!
        public bool Auth(string login, string password)
        {
            // Проверка заполнения полей
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            using (var db = new Mizin_2ЗISIP1624_PR9_Entities())
            {
                // Поиск пользователя (регистронезависимый логин)
                var user = db.User.FirstOrDefault(u =>
                    u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    // === CAPTCHA: Увеличиваем счётчик и проверяем необходимость ===
                    AttemptCounter.IncrementFailedAttempts(login);
                    CheckCaptchaRequirement(login);
                    return false;
                }

                // Проверка пароля (регистрозависимый!)
                if (user.Password != password)
                {
                    // === CAPTCHA: Увеличиваем счётчик и проверяем необходимость ===
                    AttemptCounter.IncrementFailedAttempts(login);
                    CheckCaptchaRequirement(login);
                    return false;
                }

                // Проверка на удаленного пользователя
                if (user.Role == "Удален")
                {
                    return false;
                }

                // Успешная авторизация - сбросить счётчик
                AttemptCounter.ResetAttempts(login);
                return true;
            }
        }

        // ========== Вспомогательный метод для получения данных пользователя ==========
        private User GetUserByLogin(string login)
        {
            using (var db = new Mizin_2ЗISIP1624_PR9_Entities())
            {
                return db.User.FirstOrDefault(u =>
                    u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
            }
        }

        // ========== ПРОВЕРКА: Нужно ли показать CAPTCHA ==========
        private void CheckCaptchaRequirement(string login)
        {
            if (AttemptCounter.ShouldShowCaptcha(login))
            {
                MessageBox.Show("После 3 неудачных попыток требуется ввести CAPTCHA!",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService.Navigate(new CaptchaPage(this));
            }
        }

        // ========== Обработчик кнопки "Вход" (с MessageBox для пользователя) ==========
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxLogin.Text;
            string password = PasswordBox.Password;

            bool result = Auth(login, password);

            if (result)
            {
                // Успешная авторизация - выводим приветствие
                var user = GetUserByLogin(login);
                if (user != null)
                {
                    MessageBox.Show($"Здравствуйте, {user.Role} {user.FIO}!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                // Показываем сообщения об ошибках только в UI
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    var user = GetUserByLogin(login);

                    if (user == null)
                    {
                        MessageBox.Show("Пользователь не найден!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (user.Role == "Удален")
                    {
                        MessageBox.Show("Пользователь удален!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Неверный пароль!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // ========== Обработчик кнопки "Регистрация" ==========
        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

        // ========== Обработчики для плейсхолдеров ==========
        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (WatermarkLogin != null)
            {
                WatermarkLogin.Visibility = string.IsNullOrEmpty(TextBoxLogin.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (WatermarkPassword != null)
            {
                WatermarkPassword.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
    }
}