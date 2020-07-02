using NUnit.Framework;
using Calendar.Controllers;
using Moq;
using System;

namespace Calendar.Tests
{
    class SessionControllerTests
    {
        #region Constants
        const string nullValue = null;
        const string empty = "";
        const string aSpace = " ";
        const string moreThanOneSpace = "   ";
        const string anInvalidUsername = "an invalid username";
        const string aValidUsername = "a valid username";
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
        public void CurrentUser_SomeCurrentUser_ReturnsSameCurrentUser()
        {
            // Arrange
            sessionController.CurrentUserName = aValidUsername;

            // Act & Assert
            Assert.AreEqual(aValidUsername, sessionController.CurrentUserName);
        }

        [Test]
        public void IsSessionLogoned_ValidCurrentUser_ReturnsTrue()
        {
            // Arrange
            sessionController.CurrentUserName = aValidUsername;

            // Act & Assert
            Assert.IsTrue(sessionController.IsSessionLogoned());
        }

        [Test]
        public void IsSessionLogoned_NullCurrentUser_ReturnsFalse()
        {
            // Arrange
            sessionController.CurrentUserName = null;

            // Act & Assert
            Assert.IsFalse(sessionController.IsSessionLogoned());
        }

        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(anInvalidUsername)]
        public void LogOn_InvalidUsername_CurrentUsernameIsStillNull(string invalidUsername)
        {
            // Arrange
            mockUserController
                .SetupGet(userController => userController.IsValidUserName)
                .Returns(false);

            // Act
            sessionController.LogOn(mockUserController.Object);

            //Assert
            Assert.AreEqual(nullValue, sessionController.CurrentUserName);
        }

        [Test]
        public void LogOn_NullUserController_ThrowArgumentNullException()
        {
            // Arrange
            UserController nullUserController = null;

            // Act & Assert
            Assert.That(() => sessionController.LogOn(nullUserController), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void LogOn_ValidUsername_CurrentUsernameIs()
        {
            // Arrange
            mockUserController
                .SetupGet(userController => userController.IsValidUserName)
                .Returns(true);

            mockUserController
                .SetupGet(userController => userController.SourceUserName)
                .Returns(aValidUsername);

            // Act
            sessionController.LogOn(mockUserController.Object);

            //Assert
            Assert.AreEqual(aValidUsername, sessionController.CurrentUserName);
        }
        #endregion
    }
}
