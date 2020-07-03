using NUnit.Framework;
using Calendar.Controllers;
using Moq;
using System;

namespace Calendar.Tests
{
    class SessionControllerTests
    {
        #region Constants
        private const string nullValue = null;
        private const string empty = "";
        private const string aSpace = " ";
        private const string moreThanOneSpace = "   ";
        private const string anInvalidUserName = "an invalid username";
        private const string aValidUserName = "a valid username";
        #endregion


        #region Fields
        private Mock<UserController> mockUserController;
        private SessionController sessionController;
        #endregion


        #region Properties
        #endregion


        #region Methods
        [SetUp]
        public void SetUp()
        {
            mockUserController = new Mock<UserController>();
            sessionController = new SessionController();
        }

        [TearDown]
        public void TearDown()
        {
            mockUserController = null;
            sessionController = null;
        }

        [Test]
        public void CurrentUserName_SomeCurrentUserName_ReturnsSameCurrentUserName()
        {
            sessionController.CurrentUserName = aValidUserName;
            Assert.AreEqual(aValidUserName, sessionController.CurrentUserName);
        }

        [Test]
        public void IsSessionLogoned_ValidCurrentUser_ReturnsTrue()
        {
            sessionController.CurrentUserName = aValidUserName;
            Assert.IsTrue(sessionController.IsSessionLogoned());
        }

        [Test]
        public void IsSessionLogoned_NullCurrentUser_ReturnsFalse()
        {
            sessionController.CurrentUserName = null;
            Assert.IsFalse(sessionController.IsSessionLogoned());
        }

        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(anInvalidUserName)]
        public void LogOn_InvalidUserName_CurrentUserNameIsStillNull(string invalidUserName)
        {
            mockUserController
                .SetupGet(userController => userController.IsValidUserName)
                .Returns(false);

            sessionController.LogOn(mockUserController.Object);

            Assert.IsFalse(sessionController.IsSessionLogoned());
        }

        [Test]
        public void LogOn_NullUserController_ThrowsArgumentNullException()
        {
            UserController nullUserController = null;

            Assert.That(
                () => sessionController.LogOn(nullUserController),
                Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void LogOn_ValidUserName_CurrentUserNameIs()
        {
            mockUserController
                .SetupGet(userController => userController.IsValidUserName)
                .Returns(true);

            mockUserController
                .SetupGet(userController => userController.SourceUserName)
                .Returns(aValidUserName);

            sessionController.LogOn(mockUserController.Object);

            Assert.AreEqual(aValidUserName, sessionController.CurrentUserName);
        }
        #endregion
    }
}
