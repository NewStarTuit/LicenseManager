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

namespace LicenseManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void generate_new_license(object sender, RoutedEventArgs e)
        {
            LicenseModel licenseModel = new LicenseModel
            {
                LicenseOwner = "Umidjon Mamajonov",
                ExpireDate = (DateTime)date_pick.SelectedDate,
                Product = "My Product",
                Version = "1.0.0"

            };
            string prvKey = private_key.Text;
            license_key.Text = LicenseControlller.GenerateLicense(licenseModel, prvKey);
        }

        private void verify_license_file(object sender, RoutedEventArgs e)
        {
            LicenseModel licenseModel = new LicenseModel();
            string pubKey = public_key.Text;
            if (LicenseControlller.VerifyLicense(out licenseModel, pubKey))
            {
                license_verify.Text = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}", licenseModel.Status,
                                                                                        licenseModel.Product,
                                                                                        licenseModel.LicenseOwner,
                                                                                        licenseModel.Key,
                                                                                        licenseModel.ExpireDate.ToString("dd.MM.yyyy"));
            }
            else
                license_verify.Text = "LICENSE INVALID";
        }

        private void generate_new_keys(object sender, RoutedEventArgs e)
        {
            string[] keys = LicenseControlller.GenerateNewKeys();
            public_key.Text = keys[0];
            private_key.Text = keys[1];

        }
    }
}
