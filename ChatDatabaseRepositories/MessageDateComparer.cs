using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDatabaseRepositories
{
    public class MessageDateComparer : IComparer<chatMessage>
    {
        public int Compare(chatMessage x, chatMessage y)
        {
            if (x.post_time == y.post_time)
            {
                return 0;
            }
            else
            {
                if (x.post_time < y.post_time)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }

        }
    }
}
