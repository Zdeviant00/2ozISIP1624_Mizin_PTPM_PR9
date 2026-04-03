using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mizin_WPF_PR9.Pages;
using System;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest3
    {
        private AuthPage _authPage;

        [TestInitialize]
        public void Setup()
        {
            _authPage = new AuthPage();
        }

        // ========== TestCase_AuthPage_1 + TestCase_AuthPage_4 ==========
        // Тест на авторизацию успешно всеми активными пользователями из БД
        [TestMethod]
        public void AuthTestSuccess()
        {
            // Пользователь 1: Менеджер А (основной тестовый сценарий)
            Assert.IsTrue(_authPage.Auth("Vladlena@gmail.com", "07i7Lb"),
                "Менеджер А должен успешно авторизоваться");

            // Пользователь 2: Администратор
            Assert.IsTrue(_authPage.Auth("Elizor@gmail.com", "yntiRS"),
                "Администратор должен успешно авторизоваться");

            // Пользователь 3: Менеджер С
            Assert.IsTrue(_authPage.Auth("Adam@gmail.com", "7SP9CV"),
                "Менеджер С должен успешно авторизоваться");

            // Пользователь 5: Администратор
            Assert.IsTrue(_authPage.Auth("Yli@gmail.com", "GwffgE"),
                "Администратор должен успешно авторизоваться");

            // Пользователь 6: Администратор
            Assert.IsTrue(_authPage.Auth("Vasilisa@gmail.com", "d7iKKV"),
                "Администратор должен успешно авторизоваться");

            // Пользователь 7: Менеджер А
            Assert.IsTrue(_authPage.Auth("Galina@gmail.com", "8KC7wJ"),
                "Менеджер А должен успешно авторизоваться");

            // Пользователь 8: Администратор
            Assert.IsTrue(_authPage.Auth("Miron@gmail.com", "x58OAN"),
                "Администратор должен успешно авторизоваться");

            // Пользователь 9: Менеджер С
            Assert.IsTrue(_authPage.Auth("Vseslava@gmail.com", "fhDSBf"),
                "Менеджер С должен успешно авторизоваться");

            // Пользователь 11: Менеджер А
            Assert.IsTrue(_authPage.Auth("Anisa@gmail.com", "Wh4OYm"),
                "Менеджер А должен успешно авторизоваться");

            // Пользователь 12: Менеджер С
            Assert.IsTrue(_authPage.Auth("Feafan@gmail.com", "Kc1PeS"),
                "Менеджер С должен успешно авторизоваться");

            // Пользователь 14: Тестовый (добавлен при регистрации)
            Assert.IsTrue(_authPage.Auth("void@example.com", "a12345"),
                "Тестовый пользователь должен успешно авторизоваться");

            // ========== TestCase_AuthPage_4: Регистронезависимый логин ==========
            Assert.IsTrue(_authPage.Auth("VLADLENA@GMAIL.COM", "07i7Lb"),
                "Логин должен быть регистронезависимым");

            Assert.IsTrue(_authPage.Auth("vladlena@gmail.com", "07i7Lb"),
                "Логин должен быть регистронезависимым (нижний регистр)");
        }
    }
}