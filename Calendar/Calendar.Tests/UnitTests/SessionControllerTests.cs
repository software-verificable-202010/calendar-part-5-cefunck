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
            
            sessionController.CurrentUserName = aValidUsername;

            
            Assert.AreEqual(aValidUsername, sessionController.CurrentUserName);
        }

        [Test]
        public void IsSessionLogoned_ValidCurrentUser_ReturnsTrue()
        {
            
            sessionController.CurrentUserName = aValidUsername;

            
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
        [TestCase(anInvalidUsername)]
        public void LogOn_InvalidUsername_CurrentUsernameIsStillNull(string invalidUsername)
        {
            
            mockUserController
                .SetupGet(userController => userController.IsValidUserName)
                .Returns(false);

            
            sessionController.LogOn(mockUserController.Object);

            Assert.AreEqual(nullValue, sessionController.CurrentUserName);
        }

        [Test]
        public void LogOn_NullUserController_ThrowArgumentNullException()
        {
            
            UserController nullUserController = null;

            
            Assert.That(() => sessionController.LogOn(nullUserController), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void LogOn_ValidUsername_CurrentUsernameIs()
        {
            
            mockUserController
                .SetupGet(userController => userController.IsValidUserName)
                .Returns(true);

            mockUserController
                .SetupGet(userController => userController.SourceUserName)
                .Returns(aValidUsername);

            
            sessionController.LogOn(mockUserController.Object);

            Assert.AreEqual(aValidUsername, sessionController.CurrentUserName);
        }
        #endregion
    }
}
