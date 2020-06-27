using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AesSource
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Key
        private string _key;
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                if (_key != value)
                {
                    _key = value;
                    this.OnPropertyChanged();
                }
            }
        }
        #endregion

        #region WillEncryptText
        private string _willEncryptText;
        public string WillEncryptText
        {
            get
            {
                return _willEncryptText;
            }
            set
            {
                if (_willEncryptText != value)
                {
                    _willEncryptText = value;
                    this.OnPropertyChanged();
                }
            }
        }
        #endregion

        #region WillDecryptText
        private string _willDecryptText;
        public string WillDecryptText
        {
            get
            {
                return _willDecryptText;
            }
            set
            {
                if (_willDecryptText != value)
                {
                    _willDecryptText = value;
                    this.OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Result
        private string _result;
        public string Result
        {
            get
            {
                return _result;
            }
            set
            {
                if (_result != value)
                {
                    _result = value;
                    this.OnPropertyChanged();
                }
            }
        }
        #endregion

        #region IsErrorOccured
        private bool _isErrorOccured;
        public bool IsErrorOccured
        {
            get
            {
                return _isErrorOccured;
            }
            set
            {
                if(value!=_isErrorOccured)
                {
                    _isErrorOccured = value;
                    this.OnPropertyChanged();
                }
            }
        }
        #endregion



        AesController aesController;


        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            aesController = new AesController();
            Key = "This is my key";
        }

        private void Dec_Click(object sender, RoutedEventArgs e)
        {

            IsErrorOccured = false;
            if (Key == null || Key.Length == 0)
            {
                Key = "This is my key";                
            }
            var genericResponse = aesController.Decrypt(WillDecryptText, Key);
            if(!genericResponse.IsSuccess)
            {
                IsErrorOccured = true;
                var errorList = String.Empty;
                foreach (var error in genericResponse.Errors)
                {
                    errorList += error;
                }
                Result = errorList;
                return;
            }
            Result = genericResponse.ResultValue;

        }

        private void Enc_Click(object sender, RoutedEventArgs e)
        {
            IsErrorOccured = false;

            if (Key == null || Key.Length == 0)
            {
                Key = "This is my key";
            }
            var genericResponse = aesController.Encrypt(WillEncryptText, Key);
            if (!genericResponse.IsSuccess)
            {
                IsErrorOccured = true;
                var errorList = String.Empty;
                foreach (var error in genericResponse.Errors)
                {
                    errorList += error;
                }
                Result = errorList;
                return;
            }
            Result = genericResponse.ResultValue;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
