using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mizin_WPF_PR9.Pages;  
using System;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest2
    {
        // ========== Метод с несколькими проверками ==========
        [TestMethod]
        public void AuthTest()
        {
            var page = new AuthPage();

            // Проверка 1: Несуществующий пользователь
            Assert.IsFalse(page.Auth("test", "test"));

            // Проверка 2: Несуществующий пользователь
            Assert.IsFalse(page.Auth("user1", "12345"));

            // Проверка 3: Пустые поля
            Assert.IsFalse(page.Auth("", ""));

            // Проверка 4: Поля с пробелами
            Assert.IsFalse(page.Auth(" ", " "));
        }
    }
}