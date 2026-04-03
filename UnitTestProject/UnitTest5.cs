using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mizin_WPF_PR9.Pages;
using System;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest5
    {
        private RegPage _regPage;

        [TestInitialize]
        public void Setup()
        {
            _regPage = new RegPage();
        }

        // ========== TestCase_RegPage_1 (позитивный) ==========
        // Успешная регистрация с корректными данными
        [TestMethod]
        public void RegisterTest_Success()
        {
            string uniqueLogin = $"test_{DateTime.Now.Ticks}@gmail.com";

            Assert.IsTrue(_regPage.Register("Тестов Пользователь", uniqueLogin, "test123", "test123"),
                "Регистрация с корректными данными должна пройти успешно");
        }

        // ========== TestCase_RegPage_1_negative ==========
        // Неполное заполнение полей
        [TestMethod]
        public void RegisterTest_EmptyFields()
        {
            Assert.IsFalse(_regPage.Register("", "", "", ""),
                "Регистрация с пустыми полями должна быть отклонена");

            Assert.IsFalse(_regPage.Register("Тестов", "", "test123", "test123"),
                "Регистрация с пустым логином должна быть отклонена");

            Assert.IsFalse(_regPage.Register("", "test@gmail.com", "test123", "test123"),
                "Регистрация с пустым ФИО должна быть отклонена");
        }

        // ========== Проверка пароля: длина ==========
        [TestMethod]
        public void RegisterTest_ShortPassword()
        {
            Assert.IsFalse(_regPage.Register("Тестов", "test@gmail.com", "12345", "12345"),
                "Пароль менее 6 символов должен быть отклонен");

            Assert.IsFalse(_regPage.Register("Тестов", "test@gmail.com", "123", "123"),
                "Пароль из 3 символов должен быть отклонен");
        }

        // ========== Проверка пароля: отсутствие цифр ==========
        [TestMethod]
        public void RegisterTest_PasswordNoDigits()
        {
            Assert.IsFalse(_regPage.Register("Тестов", "test@gmail.com", "abcdef", "abcdef"),
                "Пароль без цифр должен быть отклонен");

            Assert.IsFalse(_regPage.Register("Тестов", "test@gmail.com", "password", "password"),
                "Пароль 'password' без цифр должен быть отклонен");
        }

        // ========== Проверка пароля: русская раскладка ==========
        [TestMethod]
        public void RegisterTest_PasswordRussianLayout()
        {
            Assert.IsFalse(_regPage.Register("Тестов", "test@gmail.com", "пароль123", "пароль123"),
                "Пароль с русскими символами должен быть отклонен");
        }

        // ========== Несовпадающие пароли ==========
        [TestMethod]
        public void RegisterTest_PasswordsMismatch()
        {
            Assert.IsFalse(_regPage.Register("Тестов", "test@gmail.com", "test123", "test456"),
                "Несовпадающие пароли должны быть отклонены");

            Assert.IsFalse(_regPage.Register("Тестов", "test@gmail.com", "password1", "password2"),
                "Разные пароли должны быть отклонены");
        }

        // ========== Существующий логин ==========
        [TestMethod]
        public void RegisterTest_ExistingLogin()
        {
            Assert.IsFalse(_regPage.Register("Новый", "Vladlena@gmail.com", "newpass123", "newpass123"),
                "Существующий логин должен быть отклонен");

            Assert.IsFalse(_regPage.Register("Новый", "Elizor@gmail.com", "newpass123", "newpass123"),
                "Логин администратора должен быть отклонен");
        }
    }
}