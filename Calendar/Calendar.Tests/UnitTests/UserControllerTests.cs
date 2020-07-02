using NUnit.Framework;
using Calendar.Controllers;
using System.Collections.Generic;

namespace Calendar.Tests.UnitTests
{
    class UserControllerTests
    {
        #region Constants
        private const string validUserName = "usuario1";
        private const string nullValue = null;
        private const string empty = "";
        private const string aSpace = " ";
        private const string moreThanOneSpace = "   ";
        private const string invalidUsername = "an invalid username";
        #endregion


        #region Fields
        private UserController userController;
        #endregion


        #region Properties
        #endregion


        #region Methdos
        [SetUp]
        public void SetUp()
        {
            userController = new UserController();
        }

        [TearDown]
        public void TearDown()
        {
            userController = null;
        }

        [Test]
        public void SourceUsername_SomeUsername_ReturnsSameUsername()
        {
            // Arrange
            userController.SourceUsername = validUserName;

            // Act & Assert
            Assert.AreEqual(validUserName, userController.SourceUsername);
        }

        [Test]
        public void IsValid_ValidUsername_ReturnsTrue()
        {
            // Arrange
            userController.SourceUsername = validUserName;

            // Act & Assert
            Assert.IsTrue(userController.IsValid);
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(invalidUsername)]
        public void IsValid_InvalidUsername_ReturnsFalse(string invalidUsername)
        {
            // Arrange
            userController.SourceUsername = invalidUsername;

            // Act & Assert
            Assert.IsFalse(userController.IsValid);
        }

        [Test]
        public void GetValidUsernamesOf_UsernamesList_()
        {
            // Arrange
            List<string> usernames = new List<string>()
            {
                nullValue,
                empty,
                aSpace,
                validUserName,
                moreThanOneSpace,
                invalidUsername
            };

            List<string> validUsernames = new List<string>()
            {
                validUserName
            };

            // Act & Assert
            Assert.AreEqual(validUsernames, userController.GetValidUsernamesOf(usernames));
        }

        [Test]
        public void ExistsInvalidUsername_UsernamesListWithoutInvalidUsername_ReturnsFalse()
        {

            List<string> usernames = new List<string>()
            {
                validUserName
            };

            Assert.IsFalse(userController.ExistsInvalidUsername(usernames));
        }

        [Test]
        public void ExistsInvalidUsername_UsernamesListWithInvalidUsername_ReturnsTrue()
        {
            // Arrange
            List<string> usernames = new List<string>()
            {
                validUserName,
                invalidUsername
            };

            // Act & Assert
            Assert.IsTrue(userController.ExistsInvalidUsername(usernames));
        }

        #endregion
    }
}
