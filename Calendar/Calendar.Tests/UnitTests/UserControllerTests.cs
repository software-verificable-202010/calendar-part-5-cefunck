using NUnit.Framework;
using Calendar.Controllers;
using System.Collections.Generic;
using System;

namespace Calendar.Tests.UnitTests
{
    class UserControllerTests
    {
        #region Constants
        private const string nullUserName = null;
        private const string empty = "";
        private const string aSpace = " ";
        private const string moreThanOneSpace = "   ";
        private const string validUserName = "usuario1";
        private const string invalidUserName = "an invalid username";
        #endregion


        #region Fields
        private UserController userController;
        #endregion


        #region Properties
        #endregion


        #region Methods
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
        public void SourceUserName_SomeUserName_ReturnsSameUserName()
        {
            userController.SourceUserName = validUserName;
            Assert.AreEqual(validUserName, userController.SourceUserName);
        }

        [Test]
        public void UserController_SomeUserName_SourceUserNamePropertyReturnsSameUserName()
        {
            userController = new UserController(validUserName);
            Assert.AreEqual(validUserName, userController.SourceUserName);
        }

        [Test]
        public void IsValid_ValidUserName_ReturnsTrue()
        {
            userController.SourceUserName = validUserName;
            Assert.IsTrue(userController.IsValidUserName);
        }

        [TestCase(nullUserName)]
        [TestCase(empty)]
        [TestCase(aSpace)]
        [TestCase(moreThanOneSpace)]
        [TestCase(invalidUserName)]
        public void IsValid_InvalidUserName_ReturnsFalse(string invalidUserName)
        {
            userController.SourceUserName = invalidUserName;
            Assert.IsFalse(userController.IsValidUserName);
        }

        [Test]
        public void GetValidUserNamesOf_UserNamesLists_ReturnsOnlyValidUserNames()
        {   
            List<string> UserNames = new List<string>()
            {
                nullUserName,
                empty,
                aSpace,
                validUserName,
                moreThanOneSpace,
                invalidUserName
            };

            List<string> validUserNames = new List<string>()
            {
                validUserName
            };
            
            Assert.AreEqual(validUserNames, userController.GetValidUserNamesOf(UserNames));
        }

        [Test]
        public void ExistsInvalidUserName_UserNamesListWithoutInvalidUserName_ReturnsFalse()
        {
            List<string> UserNames = new List<string>()
            {
                validUserName
            };

            Assert.IsFalse(userController.ExistsInvalidUserName(UserNames));
        }

        [Test]
        public void ExistsInvalidUserName_NullUserNamesList_ThrowsArgumentNullException()
        {
            List<string> nullUserNames = null;

            Assert.That(
                () => userController.ExistsInvalidUserName(nullUserNames), 
                Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ExistsInvalidUserName_UserNamesListWithInvalidUserName_ReturnsTrue()
        {
            List<string> UserNames = new List<string>()
            {
                validUserName,
                invalidUserName
            };

            Assert.IsTrue(userController.ExistsInvalidUserName(UserNames));
        }

        #endregion
    }
}
