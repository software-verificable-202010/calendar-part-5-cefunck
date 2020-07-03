using NUnit.Framework;
using Calendar.Controllers;
using System.Collections.Generic;
using System;

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
            
            userController.SourceUserName = validUserName;

            
            Assert.AreEqual(validUserName, userController.SourceUserName);
        }

        [Test]
        public void UserController_SomeUsername_ReturnsSameUsername()
        {
            
            userController = new UserController(validUserName);

            
            Assert.AreEqual(validUserName, userController.SourceUserName);
        }

        [Test]
        public void IsValid_ValidUsername_ReturnsTrue()
        {
            
            userController.SourceUserName = validUserName;

            
            Assert.IsTrue(userController.IsValidUserName);
        }

        [TestCase(nullValue)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(invalidUsername)]
        public void IsValid_InvalidUsername_ReturnsFalse(string invalidUsername)
        {
            
            userController.SourceUserName = invalidUsername;

            
            Assert.IsFalse(userController.IsValidUserName);
        }

        [Test]
        public void GetValidUserNamesOf_UserNamesList_()
        {
            
            List<string> UserNames = new List<string>()
            {
                nullValue,
                empty,
                aSpace,
                validUserName,
                moreThanOneSpace,
                invalidUsername
            };

            List<string> validUserNames = new List<string>()
            {
                validUserName
            };

            
            Assert.AreEqual(validUserNames, userController.GetValidUserNamesOf(UserNames));
        }

        [Test]
        public void ExistsInvalidUsername_UserNamesListWithoutInvalidUsername_ReturnsFalse()
        {

            List<string> UserNames = new List<string>()
            {
                validUserName
            };

            Assert.IsFalse(userController.ExistsInvalidUserName(UserNames));
        }

        [Test]
        public void ExistsInvalidUsername_NullUserNamesList_ReturnsFalse()
        {
            
            List<string> nullUserNames = null;

            
            Assert.That(() => userController.ExistsInvalidUserName(nullUserNames), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ExistsInvalidUsername_UserNamesListWithInvalidUsername_ReturnsTrue()
        {
            
            List<string> UserNames = new List<string>()
            {
                validUserName,
                invalidUsername
            };

            
            Assert.IsTrue(userController.ExistsInvalidUserName(UserNames));
        }

        #endregion
    }
}
