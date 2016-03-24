using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using ChatInterfaces;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace TestClient.ViewModels
{
    public class ChatPageViewModel : BaseViewModel
    {
        public event EventHandler SomethingHappened;
        DuplexChannelFactory<IChatService> channelFactory;
        IChatService server;
        List<string> usersLoggedIn;
        //ChatClientImpl chatImpl;

        public ChatPageViewModel()
        {
            channelFactory = new DuplexChannelFactory<IChatService>(new ChatClientImpl(), "ChatServiceEndpoint");
            server = channelFactory.CreateChannel();

            user = Environment.UserName.ToString(); 
            server.Login(user);

            //chatImpl = new ChatClientImpl();

            text = new ObservableCollection<string>();

            usersLoggedIn = server.UsernameInChat();
            userList = new ObservableCollection<string>();
            foreach (string u in usersLoggedIn)
            {
                userList.Add(user);
            }
        }

        public void DoSomething()
        {
            EventHandler handler = SomethingHappened;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        //UPDATE USERLIST
        //private void UpdateCommands()
        //{
        //    (OnPropertyChanged("userList")).RaiseCanExecuteChanged();
        //}

        private string _user;

        public string user
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("user");
            }
        }

        //SEND BUTTON
        private ICommand _sendButton;

        public ICommand sendButton
        {
            get 
            {
                if (_sendButton == null)
                {
                    _sendButton = new Command(SendButton, CanPressSendButton);
                }
                return _sendButton; 
            }
            set { _sendButton = value; }
        }

        public void SendButton()
        {
            server.SendMessage(messageToSend);            
            text.Add(String.Format("[{0}] {1}: {2}",DateTime.Now.ToString("HH:mm:ss"), user, messageToSend));
            messageToSend = "";
        }

        public bool CanPressSendButton()
        {
            return true;
        }

        //SEND MESSAGE IN TEXTBOX
        private string _messageToSend;

        public string messageToSend
        {
            get { return _messageToSend; }
            set 
            {
                _messageToSend = value;
                OnPropertyChanged("messageToSend");
            }
        }

        //MESSAGES THAT APPEAR IN THE MAIN LISTBOX
        //UNFINISHED
        private ObservableCollection<string> _text;

        public ObservableCollection<string> text
        {
            get { return _text; }
            set 
            { 
                _text = value;
                OnPropertyChanged("text");
            }
        }
        
        //LIST OF USERS
        private ObservableCollection<string> _userList;

        public ObservableCollection<string> userList
        {
            get { return _userList; }
            set 
            {
                _userList = value;
                OnPropertyChanged("userList");
            }
        }

        //METHOD FOR GETTING USER LIST  
        public void AllUsersCurrentlyLoggedIn()
        {
            //userList = server.UsernameInChat();
        }        

        public Random cs() {

            Random s = new Random();
            return s;
    }
    }
}
