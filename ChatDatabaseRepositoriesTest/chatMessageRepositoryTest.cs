using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatDatabaseRepositories;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace ChatDatabaseRepositoriesTest
{
    [TestClass]
    public class chatMessageRepositoryTest
    {
        [TestMethod]
        public void Test_chatMessageRepos_LoadsAllmessagesFromDatabaseTotheBuffer_whenLoadsMethodIsCalled()
        {
            //Arrange
            chatMessage message1 = new chatMessage() { message_id = 1 };
            chatMessage message2 = new chatMessage() { message_id = 2 };
            chatMessage message3 = new chatMessage() { message_id = 3 };
            List<chatMessage> expectedList = new List<chatMessage>() { message1, message2, message3 };

            var query = new List<chatMessage>() { message1, message2, message3 }.AsQueryable();

            var chatMessageTable = new Mock<DbSet<chatMessage>>();
            chatMessageTable.As<IQueryable<chatMessage>>().Setup(m => m.Provider).Returns(query.Provider);
            chatMessageTable.As<IQueryable<chatMessage>>().Setup(m => m.Expression).Returns(query.Expression);
            chatMessageTable.As<IQueryable<chatMessage>>().Setup(m => m.ElementType).Returns(query.ElementType);
            chatMessageTable.As<IQueryable<chatMessage>>().Setup(m => m.GetEnumerator()).Returns(() => query.GetEnumerator());

            Mock<ChatDataBaseEntities> context = new Mock<ChatDataBaseEntities>();
            context.Setup(x => x.chatMessages).Returns(chatMessageTable.Object);
            chatMessageRepository messageRepos = new chatMessageRepository(context.Object);

            //Act
            messageRepos.Load();
            //Assert
            List<chatMessage> testList = messageRepos.Buffer;

            CollectionAssert.AreEqual(expectedList, testList);
        }

        [TestMethod]
        public void Test_chatMessageRepos_LoadsOnlyThemessagesThatArenotAlreadyInTheBuffer_whenLoadsMethodIsCalled()
        {
            //Arrange
            chatMessage message1 = new chatMessage() { message_id = 1 };
            chatMessage message2 = new chatMessage() { message_id = 2 };
            chatMessage message3 = new chatMessage() { message_id = 3 };
            chatMessage message4 = new chatMessage() { message_id = 4 };


            var query = new List<chatMessage>() { message1, message2, message3 }.AsQueryable();//list A 3 messages

            var chatMessageTable = new Mock<DbSet<chatMessage>>();
            chatMessageTable.As<IQueryable<chatMessage>>().Setup(m => m.Provider).Returns(query.Provider);
            chatMessageTable.As<IQueryable<chatMessage>>().Setup(m => m.Expression).Returns(query.Expression);
            chatMessageTable.As<IQueryable<chatMessage>>().Setup(m => m.ElementType).Returns(query.ElementType);
            chatMessageTable.As<IQueryable<chatMessage>>().Setup(m => m.GetEnumerator()).Returns(() => query.GetEnumerator());

            var query2 = new List<chatMessage>() { message1, message2, message3, message4 }.AsQueryable(); //listB 4 messages

            var chatMessageTableB = new Mock<DbSet<chatMessage>>();
            chatMessageTableB.As<IQueryable<chatMessage>>().Setup(m => m.Provider).Returns(query2.Provider);
            chatMessageTableB.As<IQueryable<chatMessage>>().Setup(m => m.Expression).Returns(query2.Expression);
            chatMessageTableB.As<IQueryable<chatMessage>>().Setup(m => m.ElementType).Returns(query2.ElementType);
            chatMessageTableB.As<IQueryable<chatMessage>>().Setup(m => m.GetEnumerator()).Returns(() => query2.GetEnumerator());

            Mock<ChatDataBaseEntities> context = new Mock<ChatDataBaseEntities>();

            context.Setup(x => x.chatMessages).Returns(chatMessageTable.Object);
            chatMessageRepository messageRepos = new chatMessageRepository(context.Object);
            //Act
            messageRepos.Load();
            //ActB
            context.Setup(x => x.chatMessages).Returns(chatMessageTableB.Object);
            messageRepos.Load();
            //Assert
            List<chatMessage> testList = messageRepos.Buffer;

            List<chatMessage> expectedList = new List<chatMessage>() { message1, message2, message3, message4 };
            CollectionAssert.AreEqual(expectedList, testList);
        }

        [TestMethod]
        public void Test_chatMessageRepos_CallsSaveChangeMethode_WhenDownLoadsMethodIsCalled()
        {
            //Arrange
            Mock<ChatDataBaseEntities> context = new Mock<ChatDataBaseEntities>();
            chatMessageRepository messageRepos = new chatMessageRepository(context.Object);
            //Act
            messageRepos.DownLoad();
            //Assert
            context.Verify(x => x.SaveChanges());
        }
        [TestMethod]
        public void Test_chatMessageRepos_ReturnsAListOfAllmessageInBuffer_WhenGetAllmessageMethodeIsCalled()
        {
            //Arrange
            chatMessage message1 = new chatMessage() { message_id = 1 };
            chatMessage message2 = new chatMessage() { message_id = 2 };
            chatMessage message3 = new chatMessage() { message_id = 3 };

            List<chatMessage> expectedValue = new List<chatMessage>() { message1, message2, message3 };

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatMessageRepository messageRepos = new chatMessageRepository(context);

            messageRepos.Buffer.Add(message1);
            messageRepos.Buffer.Add(message2);
            messageRepos.Buffer.Add(message3);

            //Act
            var testValue = messageRepos.GetAllmessages();

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatMessageRepos_ReturnsAnEmptyList_WhenGetAllmessageMethodeIsCalledAndTheBufferIsEmpty()
        {
            //Arrange
            List<chatMessage> expectedValue = new List<chatMessage>() { };

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatMessageRepository messageRepos = new chatMessageRepository(context);

            //Act
            var testValue = messageRepos.GetAllmessages();

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatMessageRepos_ReturnsAListOf3MessagesInDateDescendingOrder_WhenGetLastMessagesMethodeIsCalled_Given3InArgument()
        {
            //Arrange
            chatMessage message1 = new chatMessage() { message_id = 1, post_time = new DateTime(2016, 01, 01) };
            chatMessage message2 = new chatMessage() { message_id = 2, post_time = new DateTime(2016, 01, 02) };
            chatMessage message3 = new chatMessage() { message_id = 3, post_time = new DateTime(2016, 01, 03) };
            chatMessage message4 = new chatMessage() { message_id = 4, post_time = new DateTime(2016, 01, 04) };
            chatMessage message5 = new chatMessage() { message_id = 5, post_time = new DateTime(2016, 01, 05) };

            List<chatMessage> expectedValue = new List<chatMessage>() { message5, message4, message3 };


            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatMessageRepository messageRepos = new chatMessageRepository(context);
            messageRepos.Buffer.Add(message4);
            messageRepos.Buffer.Add(message2);
            messageRepos.Buffer.Add(message3);
            messageRepos.Buffer.Add(message1);
            messageRepos.Buffer.Add(message5);

            //Act
            var testValue = messageRepos.GetLastMessages(3);

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatMessageRepos_ReturnsAListOf2MessagesInDateDescendingOrder_WhenGetLastMessagesMethodeIsCalled_Given2InArgument()
        {
            //Arrange
            chatMessage message1 = new chatMessage() { message_id = 1, post_time = new DateTime(2016, 01, 01) };
            chatMessage message2 = new chatMessage() { message_id = 2, post_time = new DateTime(2016, 01, 02) };
            chatMessage message3 = new chatMessage() { message_id = 3, post_time = new DateTime(2016, 01, 03) };
            chatMessage message4 = new chatMessage() { message_id = 4, post_time = new DateTime(2016, 01, 04) };
            chatMessage message5 = new chatMessage() { message_id = 5, post_time = new DateTime(2016, 01, 05) };

            List<chatMessage> expectedValue = new List<chatMessage>() { message5, message4 };


            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatMessageRepository messageRepos = new chatMessageRepository(context);
            messageRepos.Buffer.Add(message4);
            messageRepos.Buffer.Add(message2);
            messageRepos.Buffer.Add(message3);
            messageRepos.Buffer.Add(message1);
            messageRepos.Buffer.Add(message5);

            //Act
            var testValue = messageRepos.GetLastMessages(2);

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatMessageRepos_ReturnsMyListOf3Messages_WhenGetMessagesBeforeMethodeIsCalled_GivenmyDateInArgument()
        {
            //Arrange
            chatMessage message1 = new chatMessage() { message_id = 1, post_time = new DateTime(2016, 01, 01) };
            chatMessage message2 = new chatMessage() { message_id = 2, post_time = new DateTime(2016, 01, 05) };
            chatMessage message3 = new chatMessage() { message_id = 3, post_time = new DateTime(2016, 01, 10) };
            chatMessage message4 = new chatMessage() { message_id = 4, post_time = new DateTime(2016, 01, 15) };
            chatMessage message5 = new chatMessage() { message_id = 5, post_time = new DateTime(2016, 01, 20) };

            List<chatMessage> expectedValue = new List<chatMessage>() { message3, message2, message1 };
            DateTime myDate = new DateTime(2016, 01, 14);

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatMessageRepository messageRepos = new chatMessageRepository(context);
            messageRepos.Buffer.Add(message4);
            messageRepos.Buffer.Add(message2);
            messageRepos.Buffer.Add(message3);
            messageRepos.Buffer.Add(message1);
            messageRepos.Buffer.Add(message5);

            //Act
            var testValue = messageRepos.GetMessagesBefore(myDate);

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatMessageRepos_ReturnsMyListOf2Messages_WhenGetMessagesBeforeMethodeIsCalled_GivenmyDateInArgument()
        {
            //Arrange
            chatMessage message1 = new chatMessage() { message_id = 1, post_time = new DateTime(2016, 01, 01) };
            chatMessage message2 = new chatMessage() { message_id = 2, post_time = new DateTime(2016, 01, 05) };
            chatMessage message3 = new chatMessage() { message_id = 3, post_time = new DateTime(2016, 01, 10) };
            chatMessage message4 = new chatMessage() { message_id = 4, post_time = new DateTime(2016, 01, 15) };
            chatMessage message5 = new chatMessage() { message_id = 5, post_time = new DateTime(2016, 01, 20) };

            List<chatMessage> expectedValue = new List<chatMessage>() { message2, message1 };
            DateTime myDate = new DateTime(2016, 01, 9);

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatMessageRepository messageRepos = new chatMessageRepository(context);
            messageRepos.Buffer.Add(message4);
            messageRepos.Buffer.Add(message2);
            messageRepos.Buffer.Add(message3);
            messageRepos.Buffer.Add(message1);
            messageRepos.Buffer.Add(message5);

            //Act
            var testValue = messageRepos.GetMessagesBefore(myDate);

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatMessageRepos_ReturnsMyListOf3Messages_WhenGetMessagesAfterMethodeIsCalled_GivenmyDateInArgument()
        {
            //Arrange
            chatMessage message1 = new chatMessage() { message_id = 1, post_time = new DateTime(2016, 01, 01) };
            chatMessage message2 = new chatMessage() { message_id = 2, post_time = new DateTime(2016, 01, 05) };
            chatMessage message3 = new chatMessage() { message_id = 3, post_time = new DateTime(2016, 01, 10) };
            chatMessage message4 = new chatMessage() { message_id = 4, post_time = new DateTime(2016, 01, 15) };
            chatMessage message5 = new chatMessage() { message_id = 5, post_time = new DateTime(2016, 01, 20) };

            List<chatMessage> expectedValue = new List<chatMessage>() { message5, message4, message3 };
            DateTime myDate = new DateTime(2016, 01, 9);

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatMessageRepository messageRepos = new chatMessageRepository(context);
            messageRepos.Buffer.Add(message4);
            messageRepos.Buffer.Add(message2);
            messageRepos.Buffer.Add(message3);
            messageRepos.Buffer.Add(message1);
            messageRepos.Buffer.Add(message5);

            //Act
            var testValue = messageRepos.GetMessagesAfter(myDate);

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatMessageRepos_ReturnsMyListOf3Messages_WhenGetMessagesBetweenMethodeIsCalled_GivenmyDateAAndmyDateBInArgument()
        {
            //Arrange
            chatMessage message1 = new chatMessage() { message_id = 1, post_time = new DateTime(2016, 01, 01) };
            chatMessage message2 = new chatMessage() { message_id = 2, post_time = new DateTime(2016, 01, 05) };
            chatMessage message3 = new chatMessage() { message_id = 3, post_time = new DateTime(2016, 01, 10) };
            chatMessage message4 = new chatMessage() { message_id = 4, post_time = new DateTime(2016, 01, 15) };
            chatMessage message5 = new chatMessage() { message_id = 5, post_time = new DateTime(2016, 01, 20) };

            List<chatMessage> expectedValue = new List<chatMessage>() { message4, message3, message2 };
            DateTime myDateA = new DateTime(2016, 01, 01);
            DateTime myDAteB = new DateTime(2016, 01, 20);

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatMessageRepository messageRepos = new chatMessageRepository(context);
            messageRepos.Buffer.Add(message4);
            messageRepos.Buffer.Add(message2);
            messageRepos.Buffer.Add(message3);
            messageRepos.Buffer.Add(message1);
            messageRepos.Buffer.Add(message5);

            //Act
            var testValue = messageRepos.GetMessagesBetween(myDateA, myDAteB);

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatMessageRepos_ReturnsAListOf3Messages_WhenGetMessagesByUserMethodeIsCalled_GivenmychatingUserInArgument()
        {
            //Arrange
            chatingUser myUser = new chatingUser() { user_id = 1, user_name = "Arkbird" };

            chatMessage message1 = new chatMessage() { message_id = 1, post_time = new DateTime(2016, 01, 01), user_id = 1 };
            chatMessage message2 = new chatMessage() { message_id = 2, post_time = new DateTime(2016, 01, 05), user_id = 1 };
            chatMessage message3 = new chatMessage() { message_id = 3, post_time = new DateTime(2016, 01, 10), user_id = 3 };
            chatMessage message4 = new chatMessage() { message_id = 4, post_time = new DateTime(2016, 01, 15), user_id = 2 };
            chatMessage message5 = new chatMessage() { message_id = 5, post_time = new DateTime(2016, 01, 20), user_id = 1 };

            List<chatMessage> expectedValue = new List<chatMessage>() { message5, message2, message1 };
            DateTime myDateA = new DateTime(2016, 01, 01);
            DateTime myDAteB = new DateTime(2016, 01, 20);

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatMessageRepository messageRepos = new chatMessageRepository(context);
            messageRepos.Buffer.Add(message4);
            messageRepos.Buffer.Add(message2);
            messageRepos.Buffer.Add(message3);
            messageRepos.Buffer.Add(message1);
            messageRepos.Buffer.Add(message5);

            //Act
            var testValue = messageRepos.GetMessagesByUser(myUser);

            //Assert
            CollectionAssert.AreEqual(expectedValue, testValue);
        }

        [TestMethod]
        public void Test_chatMessageRepos_AddmyMessageToTheBuffer_WhenRecordMessageMethodeIsCalled_GivenmyMessageInArgument()
        {
            //Arrange
            chatingUser myUser = new chatingUser() { user_id = 1, user_name = "Arkbird" };

            chatMessage myMessage = new chatMessage() { message_id = 1, post_time = new DateTime(2016, 01, 01), user_id = 1 };
            chatMessage expectedValue = myMessage;

            ChatDataBaseEntities context = new ChatDataBaseEntities();
            chatMessageRepository messageRepos = new chatMessageRepository(context);

            //Act
            messageRepos.RecordMessage(myMessage);

            //Assert
            Assert.AreSame(expectedValue, messageRepos.Buffer[0]);
        }

    }
}
