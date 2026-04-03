using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mizin_WPF_PR9.Pages;
using System;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest4
    {
        private AuthPage _authPage;

        [TestInitialize]
        public void Setup()
        {
            _authPage = new AuthPage();
        }

        // ========== TestCase_AuthPage_1_negative ==========
        // Удаленный пользователь (не может войти)
        [TestMethod]
        public void AuthTest_DeletedUser()
        {
            Assert.IsFalse(_authPage.Auth("Kar@gmail.com", "6QF1WB"),
                "Удаленный пользователь не должен авторизоваться");

            Assert.IsFalse(_authPage.Auth("Victoria@gmail.com", "9mlPQJ"),
                "Второй удаленный пользователь не должен авторизоваться");
        }

        // ========== TestCase_AuthPage_5 ==========
        // Регистрозависимый пароль (неверный регистр = отказ)
        [TestMethod]
        public void AuthTest_WrongPasswordCase()
        {
            Assert.IsFalse(_authPage.Auth("Vladlena@gmail.com", "07i7lB"),
                "Пароль должен быть регистрозависимым");

            Assert.IsFalse(_authPage.Auth("Vladlena@gmail.com", "07I7LB"),
                "Пароль должен быть регистрозависимым (все заглавные)");

            Assert.IsFalse(_authPage.Auth("Vladlena@gmail.com", "07i7lb"),
                "Пароль должен быть регистрозависимым (все строчные)");
        }

        // ========== Пустые поля ==========
        [TestMethod]
        public void AuthTest_EmptyFields()
        {
            Assert.IsFalse(_authPage.Auth("", ""),
                "Авторизация с пустыми полями должна быть отклонена");

            Assert.IsFalse(_authPage.Auth("Vladlena@gmail.com", ""),
                "Авторизация с пустым паролем должна быть отклонена");

            Assert.IsFalse(_authPage.Auth("", "07i7Lb"),
                "Авторизация с пустым логином должна быть отклонена");
        }

        // ========== Несуществующий логин ==========
        [TestMethod]
        public void AuthTest_NonExistentLogin()
        {
            Assert.IsFalse(_authPage.Auth("nonexistent@gmail.com", "password123"),
                "Несуществующий логин должен быть отклонен");

            Assert.IsFalse(_authPage.Auth("fakeuser@test.com", "test123"),
                "Фейковый логин должен быть отклонен");
        }

        // ========== Неверный пароль ==========
        [TestMethod]
        public void AuthTest_WrongPassword()
        {
            Assert.IsFalse(_authPage.Auth("Vladlena@gmail.com", "wrongpassword"),
                "Неверный пароль должен быть отклонен");

            Assert.IsFalse(_authPage.Auth("Elizor@gmail.com", "wrongpass"),
                "Неверный пароль для администратора должен быть отклонен");
        }

        // ========== Поля с пробелами ==========
        [TestMethod]
        public void AuthTest_WhitespaceFields()
        {
            Assert.IsFalse(_authPage.Auth(" ", " "),
                "Поля с пробелами должны быть отклонены");

            Assert.IsFalse(_authPage.Auth("   ", "test123"),
                "Логин с пробелами должен быть отклонен");
        }
    }
}