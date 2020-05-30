using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class MainWindow : Window
    {
        AesController aesController;
        public MainWindow()
        {
            InitializeComponent();
            aesController = new AesController();
        }

        private void Dec_Click(object sender, RoutedEventArgs e)
        {
            var decText = WillDecryptText.Text;
            var keyText = KeyText.Text;

            if(keyText==null || keyText.Length==0)
            {
                keyText = "This is my key";
                KeyText.Text = keyText;
            }
            var genericResponse = aesController.Decrypt(decText, keyText);
            Result.Text = genericResponse.ResultValue;

        }

        private void Enc_Click(object sender, RoutedEventArgs e)
        {
            var encText = WillEncryptText.Text;
            var keyText = KeyText.Text;

            if (keyText == null || keyText.Length == 0)
            {
                keyText = "This is my key";
                KeyText.Text = keyText;
            }
            var genericResponse = aesController.Encrypt(encText, keyText);
            Result.Text = genericResponse.ResultValue;
        }
    }
}
