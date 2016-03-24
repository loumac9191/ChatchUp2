using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDatabaseRepositories
{
    public class chatingUserRepository
    {
        ChatDataBaseEntities context;
        private List<chatingUser> buffer = new List<chatingUser>();

        public chatingUserRepository(ChatDataBaseEntities exContext)
        {
            context = exContext;
        }
        public void Load()
        {

            foreach (chatingUser user in context.chatingUsers)
            {
                bool IsNotIn = true;
                foreach (chatingUser bufferUser in Buffer)
                {
                    if (bufferUser.user_id == user.user_id)
                    {
                        IsNotIn = false;
                    }
                }

                if (IsNotIn)
                {
                    Buffer.Add(user);
                }

            }
        }

        public List<chatingUser> Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }

        public void DownLoad()
        {
            foreach (chatingUser user in Buffer)
            {
                bool isNotIn = true;
                foreach (chatingUser contextUser in context.chatingUsers)
                {
                    if (contextUser.user_id == user.user_id)
                    {
                        isNotIn = false;
                    }

                    if (isNotIn)
                    {
                        context.chatingUsers.Add(user);
                    }
                }
            }
            context.SaveChanges();
        }

        public List<chatingUser> GetAllUsers()
        {
            return Buffer;
        }

        public chatingUser GetUserByUserName(string userName)
        {
            return Buffer.Find(x => x.user_name == userName);
        }

        public List<chatingUser> GetUsersBySignedInDate(DateTime date)
        {
            return Buffer.FindAll(x => x.sign_in_date == date); ;
        }
    }
}
