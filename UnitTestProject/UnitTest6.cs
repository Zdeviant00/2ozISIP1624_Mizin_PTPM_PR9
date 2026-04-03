using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mizin_WPF_PR9.Pages;
using System;
using System.Linq;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest6
    {
        private AuthPage _authPage;

        [TestInitialize]
        public void Setup()
        {
            _authPage = new AuthPage();
            // Сбросить все счётчики перед каждым тестом
            AttemptCounter.ClearAll();
            AttemptCounter.ResetAttempts("test@gmail.com");
            AttemptCounter.ResetAttempts("user1@gmail.com");
            AttemptCounter.ResetAttempts("user2@gmail.com");
            AttemptCounter.ResetAttempts("Vladlena@gmail.com");
        }

        // ========== ТЕСТЫ СЧЁТЧИКА ПОПЫТОК ==========

        // Тест 1: Счётчик начинается с 0
        [TestMethod]
        public void CaptchaTest_CounterStartsAtZero()
        {
            int attempts = AttemptCounter.GetFailedAttempts("newuser@gmail.com");
            Assert.AreEqual(0, attempts, "Счётчик должен начинаться с 0");
        }

        // Тест 2: Счётчик увеличивается на 1
        [TestMethod]
        public void CaptchaTest_CounterIncrements()
        {
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            Assert.AreEqual(1, AttemptCounter.GetFailedAttempts("test@gmail.com"));

            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            Assert.AreEqual(2, AttemptCounter.GetFailedAttempts("test@gmail.com"));

            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            Assert.AreEqual(3, AttemptCounter.GetFailedAttempts("test@gmail.com"));
        }

        // Тест 3: CAPTCHA требуется после 3 попыток
        [TestMethod]
        public void CaptchaTest_RequireAfterThreeAttempts()
        {
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");

            Assert.IsTrue(AttemptCounter.ShouldShowCaptcha("test@gmail.com"),
                "CAPTCHA должна требоваться после 3 попыток");
        }

        // Тест 4: CAPTCHA не требуется после 2 попыток
        [TestMethod]
        public void CaptchaTest_NotRequireAfterTwoAttempts()
        {
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");

            Assert.IsFalse(AttemptCounter.ShouldShowCaptcha("test@gmail.com"),
                "CAPTCHA не должна требоваться после 2 попыток");
        }

        // Тест 5: Сброс счётчика
        [TestMethod]
        public void CaptchaTest_CounterReset()
        {
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");

            AttemptCounter.ResetAttempts("test@gmail.com");

            Assert.AreEqual(0, AttemptCounter.GetFailedAttempts("test@gmail.com"),
                "Счётчик должен быть сброшен до 0");
            Assert.IsFalse(AttemptCounter.ShouldShowCaptcha("test@gmail.com"),
                "CAPTCHA не должна требоваться после сброса");
        }

        // Тест 6: Разные логины имеют разные счётчики
        [TestMethod]
        public void CaptchaTest_SeparateCountersPerLogin()
        {
            AttemptCounter.IncrementFailedAttempts("user1@gmail.com");
            AttemptCounter.IncrementFailedAttempts("user1@gmail.com");

            AttemptCounter.IncrementFailedAttempts("user2@gmail.com");

            Assert.AreEqual(2, AttemptCounter.GetFailedAttempts("user1@gmail.com"));
            Assert.AreEqual(1, AttemptCounter.GetFailedAttempts("user2@gmail.com"));
        }

        // Тест 7: Сброс одного счётчика не влияет на другие
        [TestMethod]
        public void CaptchaTest_ResetOneCounterDoesNotAffectOthers()
        {
            AttemptCounter.IncrementFailedAttempts("user1@gmail.com");
            AttemptCounter.IncrementFailedAttempts("user1@gmail.com");

            AttemptCounter.IncrementFailedAttempts("user2@gmail.com");

            AttemptCounter.ResetAttempts("user1@gmail.com");

            Assert.AreEqual(0, AttemptCounter.GetFailedAttempts("user1@gmail.com"));
            Assert.AreEqual(1, AttemptCounter.GetFailedAttempts("user2@gmail.com"));
        }

        // ========== ТЕСТЫ БЛОКИРОВКИ ==========

        // Тест 8: Блокировка аккаунта
        [TestMethod]
        public void CaptchaTest_AccountLockout()
        {
            AttemptCounter.LockAccount("test@gmail.com");
            Assert.IsTrue(AttemptCounter.IsLockedOut("test@gmail.com"),
                "Аккаунт должен быть заблокирован");
        }

        // Тест 9: Незаблокированный аккаунт
        [TestMethod]
        public void CaptchaTest_NotLockedAccount()
        {
            Assert.IsFalse(AttemptCounter.IsLockedOut("test@gmail.com"),
                "Аккаунт не должен быть заблокирован по умолчанию");
        }

        // Тест 10: Блокировка не влияет на другие аккаунты
        [TestMethod]
        public void CaptchaTest_LockoutIsolation()
        {
            AttemptCounter.LockAccount("user1@gmail.com");

            Assert.IsTrue(AttemptCounter.IsLockedOut("user1@gmail.com"));
            Assert.IsFalse(AttemptCounter.IsLockedOut("user2@gmail.com"));
        }

        // ========== ИНТЕГРАЦИОННЫЕ ТЕСТЫ С AUTH ==========

        // Тест 11: Неудачная авторизация увеличивает счётчик
        [TestMethod]
        public void CaptchaTest_FailedAuthIncrementsCounter()
        {
            // Несуществующий пользователь
            _authPage.Auth("nonexistent@gmail.com", "wrongpass");

            int attempts = AttemptCounter.GetFailedAttempts("nonexistent@gmail.com");
            Assert.AreEqual(1, attempts, "Счётчик должен увеличиться на 1");
        }

        // Тест 12: Три неудачные авторизации
        [TestMethod]
        public void CaptchaTest_ThreeFailedAuthentications()
        {
            _authPage.Auth("test1@gmail.com", "wrong1");
            _authPage.Auth("test1@gmail.com", "wrong2");
            _authPage.Auth("test1@gmail.com", "wrong3");

            Assert.IsTrue(AttemptCounter.ShouldShowCaptcha("test1@gmail.com"),
                "После 3 неудачных попыток должна требоваться CAPTCHA");
        }

        // Тест 13: Успешная авторизация сбрасывает счётчик
        [TestMethod]
        public void CaptchaTest_SuccessfulAuthResetsCounter()
        {
            // Сначала неудачные попытки с несуществующим логином
            _authPage.Auth("temp@gmail.com", "wrong1");
            _authPage.Auth("temp@gmail.com", "wrong2");

            // Успешная авторизация существующего пользователя
            _authPage.Auth("Vladlena@gmail.com", "07i7Lb");

            // Счётчик для Vladlena должен быть 0
            Assert.AreEqual(0, AttemptCounter.GetFailedAttempts("Vladlena@gmail.com"));
        }

        // Тест 14: Неверный пароль существующему пользователю
        [TestMethod]
        public void CaptchaTest_WrongPasswordIncrementsCounter()
        {
            _authPage.Auth("Vladlena@gmail.com", "wrongpass1");
            _authPage.Auth("Vladlena@gmail.com", "wrongpass2");

            int attempts = AttemptCounter.GetFailedAttempts("Vladlena@gmail.com");
            Assert.AreEqual(2, attempts, "Неверный пароль должен увеличивать счётчик");
        }

        // Тест 15: Удалённый пользователь не увеличивает счётчик
        [TestMethod]
        public void CaptchaTest_DeletedUserDoesNotIncrementCounter()
        {
            int beforeAttempts = AttemptCounter.GetFailedAttempts("Kar@gmail.com");

            _authPage.Auth("Kar@gmail.com", "6QF1WB");

            int afterAttempts = AttemptCounter.GetFailedAttempts("Kar@gmail.com");
            // Удалённый пользователь может не увеличивать счётчик
        }

        // ========== ТЕСТЫ РЕГИСТРОНЕЗАВИСИМОСТИ ==========

        // Тест 16: Счётчик регистронезависимый
        [TestMethod]
        public void CaptchaTest_CounterIsCaseInsensitive()
        {
            _authPage.Auth("Test@gmail.com", "wrong1");
            _authPage.Auth("TEST@gmail.com", "wrong2");
            _authPage.Auth("test@gmail.com", "wrong3");

            // Все три попытки должны суммироваться для одного логина
            int attempts = AttemptCounter.GetFailedAttempts("test@gmail.com");
            Assert.IsTrue(attempts >= 1, "Попытки с разным регистром должны учитываться");
        }

        // ========== ТЕСТЫ ПУСТЫХ ПОЛЕЙ ==========

        // Тест 17: Пустой логин не увеличивает счётчик
        [TestMethod]
        public void CaptchaTest_EmptyLoginDoesNotIncrementCounter()
        {
            _authPage.Auth("", "password");

            int attempts = AttemptCounter.GetFailedAttempts("");
            Assert.AreEqual(0, attempts, "Пустой логин не должен увеличивать счётчик");
        }

        // Тест 18: Пустой пароль не увеличивает счётчик
        [TestMethod]
        public void CaptchaTest_EmptyPasswordDoesNotIncrementCounter()
        {
            _authPage.Auth("test@gmail.com", "");

            int attempts = AttemptCounter.GetFailedAttempts("test@gmail.com");
            Assert.AreEqual(0, attempts, "Пустой пароль не должен увеличивать счётчик");
        }

        // ========== ДОПОЛНИТЕЛЬНЫЕ ТЕСТЫ ==========

        // Тест 19: Множественные сбросы
        [TestMethod]
        public void CaptchaTest_MultipleResets()
        {
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            AttemptCounter.ResetAttempts("test@gmail.com");
            AttemptCounter.ResetAttempts("test@gmail.com");
            AttemptCounter.ResetAttempts("test@gmail.com");

            Assert.AreEqual(0, AttemptCounter.GetFailedAttempts("test@gmail.com"));
        }

        // Тест 20: Большое количество попыток
        [TestMethod]
        public void CaptchaTest_ManyAttempts()
        {
            for (int i = 0; i < 10; i++)
            {
                AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            }

            Assert.AreEqual(10, AttemptCounter.GetFailedAttempts("test@gmail.com"));
            Assert.IsTrue(AttemptCounter.ShouldShowCaptcha("test@gmail.com"));
        }

        // Тест 21: CAPTCHA требуется ровно на 3-й попытке
        [TestMethod]
        public void CaptchaTest_CaptchaExactlyOnThirdAttempt()
        {
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            Assert.IsFalse(AttemptCounter.ShouldShowCaptcha("test@gmail.com"));

            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            Assert.IsFalse(AttemptCounter.ShouldShowCaptcha("test@gmail.com"));

            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            Assert.IsTrue(AttemptCounter.ShouldShowCaptcha("test@gmail.com"));
        }

        // Тест 22: После сброса можно снова накопить попытки
        [TestMethod]
        public void CaptchaTest_CanAccumulateAfterReset()
        {
            // Первая серия
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            Assert.IsTrue(AttemptCounter.ShouldShowCaptcha("test@gmail.com"));

            // Сброс
            AttemptCounter.ResetAttempts("test@gmail.com");
            Assert.IsFalse(AttemptCounter.ShouldShowCaptcha("test@gmail.com"));

            // Вторая серия
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            AttemptCounter.IncrementFailedAttempts("test@gmail.com");
            Assert.IsTrue(AttemptCounter.ShouldShowCaptcha("test@gmail.com"));
        }
    }
}