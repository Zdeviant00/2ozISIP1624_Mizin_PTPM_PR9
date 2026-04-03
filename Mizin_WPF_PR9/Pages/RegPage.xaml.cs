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

        // ========== РЕФАКТОРИНГ: Метод регистрации (без MessageBox для тестов) ==========
        // Этот метод вызывается из тестов - поэтому нет MessageBox.Show()!
        public bool Register(string fio, string login, string password, string confirmPassword)
        {
            // 1. Проверка заполнения всех полей - без MessageBox!
            if (string.IsNullOrEmpty(fio) || string.IsNullOrEmpty(login) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                return false;  // Возвращаем false
            }

            using (var db = new Mizin_2ЗISIP1624_PR9_Entities())
            {
                // 2. Проверка наличия логина в БД
                if (db.User.Any(u => u.Login == login))
                {
                    return false;  // Возвращаем false
                }

                // 3. Проверка формата пароля
                if (!IsValidPassword(password))
                {
                    return false;
                }

                // 4. Проверка совпадения паролей
                if (password != confirmPassword)
                {
                    return false;  // Возвращаем false
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

                return true;  // Возвращаем true (без MessageBox!)
            }
        }

        // ========== Вспомогательный метод проверки пароля (без MessageBox) ==========
        private bool IsValidPassword(string password)
        {
            // а) Минимум 6 символов
            if (password.Length < 6)
            {
                return false;  // Без MessageBox!
            }

            // б) Только английская раскладка
            Regex englishPattern = new Regex(@"^[a-zA-Z0-9!@#$%^&*(),.?""':{}|<>]+$");
            if (!englishPattern.IsMatch(password))
            {
                return false;  // Без MessageBox!
            }

            // в) Хотя бы одна цифра
            if (!password.Any(char.IsDigit))
            {
                return false;  // Без MessageBox!
            }

            return true;
        }

        // ========== Обработчик кнопки "Регистрация" (с MessageBox для пользователя) ==========
        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            bool result = Register(
                TextBoxFIO.Text,
                TextBoxLogin.Text,
                PasswordBox.Password,
                PasswordBoxConfirm.Password
            );

            if (result)
            {
                MessageBox.Show("Пользователь успешно зарегистрирован!",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new AuthPage());
            }
            else
            {
                // Показываем сообщения об ошибках только в UI
                if (string.IsNullOrEmpty(TextBoxFIO.Text) || string.IsNullOrEmpty(TextBoxLogin.Text) ||
                    string.IsNullOrEmpty(PasswordBox.Password) || string.IsNullOrEmpty(PasswordBoxConfirm.Password))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    using (var db = new Mizin_2ЗISIP1624_PR9_Entities())
                    {
                        if (db.User.Any(u => u.Login == TextBoxLogin.Text))
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует!",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else if (PasswordBox.Password != PasswordBoxConfirm.Password)
                        {
                            MessageBox.Show("Пароли не совпадают!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else if (PasswordBox.Password.Length < 6)
                        {
                            MessageBox.Show("Пароль должен содержать минимум 6 символов!",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при регистрации. Проверьте формат пароля.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
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

            if (TextBlockError != null)
            {
                TextBlockError.Visibility = Visibility.Collapsed;
            }

            // Возврат на страницу авторизации
            NavigationService.Navigate(new AuthPage());
        }
    }
}