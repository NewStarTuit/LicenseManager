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
                Product = "Quick Mariant Maker",
                Version = "1.0.0"
            };

            license_key.Text = LicenseControlller.GenerateLicense(licenseModel);
        }

        private void verify_license_file(object sender, RoutedEventArgs e)
        {
            LicenseModel licenseModel = new LicenseModel();

            if (LicenseControlller.VerifyLicense(out licenseModel))
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
    }
}
