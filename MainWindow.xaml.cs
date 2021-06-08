using MBMaster.Models;
using MBMaster.Models.COMHandler;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MBMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private MBHandler _mbHandler;
        public MBHandler MBHandler
        {
            get { return _mbHandler; }
            set 
            { 
                _mbHandler = value;
                NotifyProperyChanged("MBHandler");
            }
        }

        private MBRequest _mbReq;

        public MBRequest MBReq
        {
            get { return _mbReq; }
            set
            { 
                _mbReq = value;
                NotifyProperyChanged("MBReq");
            }
        }




        // Notify property changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyProperyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            MBHandler = new MBHandler(this);
        }
    }

}
