using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _page;

        public string page
        {
            get { return _page; }
            set
            {
                _page = value;
                OnPropertyChanged("page");
            }
        }
        //tt

        public MainViewModel()
        {
            _page = "ChatPage.xaml";
        }
    }
}
