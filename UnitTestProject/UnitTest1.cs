using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Тренировочный тест на Assert

            // Assert.IsTrue — проверяет, что условие истинно
            Assert.IsTrue(2 + 2 == 4);

            // Assert.IsFalse — проверяет, что условие ложно
            Assert.IsFalse(2 + 2 == 5);

            // Assert.AreEqual — проверяет равенство значений
            Assert.AreEqual(10, 5 + 5);

            // Assert.AreNotEqual — проверяет неравенство
            Assert.AreNotEqual(10, 5 + 6);

            // Assert.IsNull — проверяет, что значение null
            Assert.IsNull(null);

            // Assert.IsNotNull — проверяет, что значение не null
            Assert.IsNotNull("Текст");
        }
    }
}