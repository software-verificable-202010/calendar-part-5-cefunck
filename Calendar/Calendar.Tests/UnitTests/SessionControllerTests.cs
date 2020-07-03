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
        const string anInvalidUserName = "an invalid username";
        const string aValidUserName = "a valid username";
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

            Assert.AreEqual(nullValue, sessionController.CurrentUserName);
        }

        [Test]
        public void LogOn_NullUserController_ThrowsArgumentNullException()
        {
            
            UserController nullUserController = null;

            
            Assert.That(() => sessionController.LogOn(nullUserController), Throws.Exception.TypeOf<ArgumentNullException>());
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
