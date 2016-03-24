using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDatabaseRepositories
{
    public class chatMessageRepository
    {
        ChatDataBaseEntities context;
        private List<chatMessage> buffer = new List<chatMessage>();
        public List<chatMessage> Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }
        public chatMessageRepository(ChatDataBaseEntities exContext)
        {
            context = exContext;
        }
        public void Load()
        {

            foreach (chatMessage message in context.chatMessages)
            {
                bool IsNotIn = true;
                foreach (chatMessage buffermessage in Buffer)
                {
                    if (buffermessage.message_id == message.message_id)
                    {
                        IsNotIn = false;
                    }
                }

                if (IsNotIn)
                {
                    Buffer.Add(message);
                }

            }
        }



        public void DownLoad()
        {
            foreach (chatMessage message in Buffer)
            {
                bool isNotIn = true;
                foreach (chatMessage contextmessage in context.chatMessages)
                {
                    if (contextmessage.message_id == message.message_id)
                    {
                        isNotIn = false;
                    }

                    if (isNotIn)
                    {
                        context.chatMessages.Add(message);
                    }
                }
            }
            context.SaveChanges();
        }

        public List<chatMessage> GetAllmessages()
        {
            return Buffer;
        }

        public List<chatMessage> GetLastMessages(int number)
        {
            List<chatMessage> orderedList = buffer;
            MessageDateComparer comparer = new MessageDateComparer();
            orderedList.Sort(comparer);
            orderedList.Reverse();

            List<chatMessage> resultList = new List<chatMessage>();
            for (int i = 0; i < number; i++)
            {
                resultList.Add(orderedList[i]);
            }

            return resultList;
        }

        public List<chatMessage> GetMessagesBefore(DateTime myDate)
        {
            List<chatMessage> orderedList = buffer;
            MessageDateComparer comparer = new MessageDateComparer();
            orderedList.Sort(comparer);
            orderedList.Reverse();

            return orderedList.FindAll(x => (x.post_time < myDate));


        }

        public List<chatMessage> GetMessagesAfter(DateTime myDate)
        {
            List<chatMessage> orderedList = buffer;
            MessageDateComparer comparer = new MessageDateComparer();
            orderedList.Sort(comparer);
            orderedList.Reverse();

            return orderedList.FindAll(x => (x.post_time > myDate));
        }

        public List<chatMessage> GetMessagesBetween(DateTime earlyDate, DateTime lateDate)
        {
            List<chatMessage> orderedList = buffer;
            MessageDateComparer comparer = new MessageDateComparer();
            orderedList.Sort(comparer);
            orderedList.Reverse();

            return orderedList.FindAll(x => ((x.post_time > earlyDate) && (x.post_time < lateDate)));

        }

        public List<chatMessage> GetMessagesByUser(chatingUser user)
        {
            List<chatMessage> orderedList = buffer;
            MessageDateComparer comparer = new MessageDateComparer();
            orderedList.Sort(comparer);
            orderedList.Reverse();

            return Buffer.FindAll(x => x.user_id == user.user_id);
        }

        public void RecordMessage(chatMessage msg)
        {
            Buffer.Add(msg);
        }
    }
}
