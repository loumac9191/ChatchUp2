using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatDatabaseRepositories;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;

namespace ChatDatabaseRepositoriesTest
{
    [TestClass]
    public class chatingUserRepositoryTest
    {
        [TestMethod]
        public void Test_chatingUserRepos_LoadsAllUsersFromDatabaseTotheBuffer_whenLoadsMethodIsCalled()
        {
            //Arrange
            chatingUser user1 = new chatingUser() { user_id = 1, user_name = "Idriss" };
            chatingUser user2 = new chatingUser() { user_id = 2, user_name = "Oyeniyi" };
            chatingUser user3 = new chatingUser() { user_id = 3, user_name = "Olaoluwa" };
            List<chatingUser> expectedList = new List<chatingUser>() { user1, user2, user3 };

            var query = new List<chatingUser>() { user1, user2, user3 }.AsQueryable();

            var chatingUserTable = new Mock<DbSet<chatingUser>>();
            chatingUserTable.As<IQueryable<chatingUser>>().Setup(m => m.Provider).Returns(query.Provider);
            chatingUserTable.As<IQueryable<chatingUser>>().Setup(m => m.Expression).Returns(query.Expression);
            chatingUserTable.As<IQueryable<chatingUser>>().Setup(m => m.ElementType).Returns(query.ElementType);
            chatingUserTable.As<IQueryable<chatingUser>>().Setup(m => m.GetEnumerator()).Returns(() => query.GetEnumerator());

            Mock<ChatDataBaseEntities> context = new Mock<ChatDataBaseEntities>();
            context.Setup(x => x.chatingUsers).Returns(chatingUserTable.Object);
            chatingUserRepository userRepos = new chatingUserRepository(context.Object);

            //Act
            userRepos.Load();
            //Assert
            List<chatingUser> testList = userRepos.Buffer;

            CollectionAssert.AreEqual(expectedList, testList);
        }

        [TestMethod]
        public void Test_chatingUserRepos_LoadsOnlyTheUsersThatArenotAlreadyInTheBuffer_whenLoadsMethodIsCalled()
        {
            //Arrange
            chatingUser user1 = new chatingUser() { user_id = 1, user_name = "Idriss" };
            chatingUser user2 = new chatingUser() { user_id = 2, user_name = "Oyeniyi" };
            chatingUser user3 = new chatingUser() { user_id = 3, user_name = "Olaoluwa" };
            chatingUser user4 = new chatingUser() { user_id = 4, user_name = "Adegun" };


            var query = new List<chatingUser>() { user1, user2, user3 }.AsQueryable();//list A 3 users

            var chatingUserTable = new Mock<DbSet<chatingUser>>();
            chatingUserTable.As<IQueryable<chatingUser>>().Setup(m => m.Provider).Returns(query.Provider);
            chatingUserTable.As<IQueryable<chatingUser>>().Setup(m => m.Expression).Returns(query.Expression);
            chatingUserTable.As<IQueryable<chatingUser>>().Setup(m => m.ElementType).Returns(query.ElementType);
            chatingUserTable.As<IQueryable<chatingUser>>().Setup(m => m.GetEnumerator()).Returns(() => query.GetEnumerator());

            var query2 = new List<chatingUser>() { user1, user2, user3, user4 }.AsQueryable(); //listB 4 users

            var chatingUserTableB = new Mock<DbSet<chatingUser>>();
            chatingUserTableB.As<IQueryable<chatingUser>>().Setup(m => m.Provider).Returns(query2.Provider);
            chatingUserTableB.As<IQueryable<chatingUser>>().Setup(m => m.Expression).Returns(query2.Expression);
            chatingUserTableB.As<IQueryable<chatingUser>>().Setup(m => m.ElementType).Returns(query2.ElementType);
            chatingUserTableB.As<IQueryable<chatingUser>>().Setup(m => m.GetEnumerator()).Returns(() => query2.GetEnumerator());

            Mock<ChatDataBaseEntities> context = new Mock<ChatDataBaseEntities>();

            context.Setup(x => x.chatingUsers).Returns(chatingUserTable.Object);
            chatingUserRepository userRepos = new chatingUserRepository(context.Object);
            //Act
            userRepos.Load();
            //ActB
            context.Setup(x => x.chatingUsers).Returns(chatingUserTableB.Object);
            userRepos.Load();
            //Assert
            List<chatingUser> testList = userRepos.Buffer;

            List<chatingUser> expectedList = new List<chatingUser>() { user1, user2, user3, user4 };
            CollectionAssert.AreEqual(expectedList, testList);
        }

        [TestMethod]
        public void Test_chatingUserRepos_CallsSaveChangeMethode_WhenDownLoadsMethodIsCalled()
        {
            //Arrange
            Mock<ChatDataBaseEntities> context = new Mock<ChatDataBaseEntities>();
            chatingUserRepository userRepos = new chatingUserRepository(context.Object);
            //Act
            userRepos.DownLoad();
            //Assert
            context.Verify(x => x.SaveChanges());
        }
        [TestMethod]
        public void Test_chatingUserRepos_ReturnsAListOfAllUserInBuffer_WhenGetAllUserMethodeIsCalled()
        {
            //Arrange
            chatingUser user1 = new chatingUser() { user_id = 1, user_name = "Idriss" };
            chatingUser user2 = new chatingUser() { user_id = 2, user_name = "Oyeniyi" };
            chatingUser user3 = new chatingUser() { user_id = 3, user_name = "Olaoluwa" };

            List<chatingUser> expectedValue = new List<chatingUser>() { user1, user2, user3 };

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatingUserRepository userRepos = new chatingUserRepository(context);

            userRepos.Buffer.Add(user1);
            userRepos.Buffer.Add(user2);
            userRepos.Buffer.Add(user3);

            //Act
            var testValue = userRepos.GetAllUsers();

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatingUserRepos_ReturnsAnEmptyList_WhenGetAllUserMethodeIsCalledAndTheBufferIsEmpty()
        {
            //Arrange
            List<chatingUser> expectedValue = new List<chatingUser>() { };

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatingUserRepository userRepos = new chatingUserRepository(context);

            //Act
            var testValue = userRepos.GetAllUsers();

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatingUserRepos_ReturnsmyUser_WhenGetUserByUserNameMethodeIsCalled_GivenMyUserUserNameInArgument()
        {
            //Arrange
            chatingUser myUser = new chatingUser() { user_name = "Idriss" };
            chatingUser expectedValue = myUser;

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatingUserRepository userRepos = new chatingUserRepository(context);
            userRepos.Buffer.Add(myUser);


            //Act
            var testValue = userRepos.GetUserByUserName("Idriss");

            //Assert
            Assert.AreSame(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatingUserRepos_ReturnsmyUser_WhenGetUsersBySignedInDateMethodeIsCalled_GivenMyUserSignedInDateInArgument()
        {
            //Arrange
            DateTime date = new DateTime(1918, 11, 11);
            chatingUser myUser = new chatingUser() { sign_in_date = date };
            List<chatingUser> expectedValue = new List<chatingUser>() { myUser };

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatingUserRepository userRepos = new chatingUserRepository(context);
            userRepos.Buffer.Add(myUser);


            //Act
            var testValue = userRepos.GetUsersBySignedInDate(date);

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatingUserRepos_ReturnsAListofthreechatingUsers_WhenGetUsersBySignedInDateMethodeIsCalled_GivenTheMutualSignedInDateInArgument()
        {
            //Arrange
            DateTime date = new DateTime(1918, 11, 11);

            chatingUser myUser1 = new chatingUser() { sign_in_date = date };
            chatingUser myUser2 = new chatingUser() { sign_in_date = date };
            chatingUser myUser3 = new chatingUser() { sign_in_date = date };
            List<chatingUser> expectedValue = new List<chatingUser>() { myUser1, myUser2, myUser3 };

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatingUserRepository userRepos = new chatingUserRepository(context);

            userRepos.Buffer.Add(myUser1);
            userRepos.Buffer.Add(myUser2);
            userRepos.Buffer.Add(myUser3);



            //Act
            var testValue = userRepos.GetUsersBySignedInDate(date);

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);

        }
    }
}
