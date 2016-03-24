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
             bool IsNotIn = true;
            foreach (chatingUser user in context.chatingUsers) 
            {
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

        public List<chatingUser> Buffer { 
                                            get{return buffer;} 
                                            set{buffer = value;} 
                                        }
    }
}
