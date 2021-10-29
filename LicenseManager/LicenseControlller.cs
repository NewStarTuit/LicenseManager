using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LicenseManager
{

    public class LicenseModel
    {
        public string Status { get; set; }
        public string Key { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Product { get; set; }
        public string Version { get; set; }
        public string LicenseOwner { get; internal set; }
    }

    class LicenseControlller
    {
        public static string[] GenerateNewKeys()
        {
            try
            {
                var rsaWrite = new RSACryptoServiceProvider();

                string public_key = rsaWrite.ToXmlString(false);
                string private_key = rsaWrite.ToXmlString(true);

                return new string[] { public_key, private_key};
            }
            catch (Exception)
            {

                return new string[] { };
            }


        }


        public static bool VerifyLicense(out LicenseModel lic_model, string pubKey)
        {
            lic_model = new LicenseModel();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("license.lic");
                XmlNode userNodes = xmlDoc.SelectSingleNode("//License/Product");
                lic_model.Product = userNodes.InnerText;

                userNodes = xmlDoc.SelectSingleNode("//License/LicenseOwner");
                lic_model.LicenseOwner = userNodes.InnerText;

                userNodes = xmlDoc.SelectSingleNode("//License/Key");
                lic_model.Key = userNodes.InnerText;

                userNodes = xmlDoc.SelectSingleNode("//License/ExpireDate");
                lic_model.ExpireDate = DateTime.Parse(userNodes.InnerText);

                if ((lic_model.ExpireDate.Date - DateTime.Now.Date).TotalDays <= 0)
                {
                    lic_model.Status = "License Expired!";
                }
                else
                    lic_model.Status = "License Active!";


                return VerifyLicenseKey(lic_model.Key, lic_model, pubKey);

            }
            catch (Exception)
            {
                return false;
            }

        }

        public static string GenerateLicense(LicenseModel lic_model, string prvKey)
        {
            lic_model.Key = GenerateLicenseKey(lic_model, prvKey);

            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("License");
            xmlDoc.AppendChild(rootNode);

            XmlNode childNode = xmlDoc.CreateElement("Product");
            childNode.InnerText = lic_model.Product;
            rootNode.AppendChild(childNode);

            childNode = xmlDoc.CreateElement("Version");
            childNode.InnerText = lic_model.Version;
            rootNode.AppendChild(childNode);

            childNode = xmlDoc.CreateElement("Organization");
            childNode.InnerText = "DataGaze LLC";
            rootNode.AppendChild(childNode);

            childNode = xmlDoc.CreateElement("LicenseOwner");
            childNode.InnerText = lic_model.LicenseOwner;
            rootNode.AppendChild(childNode);

            childNode = xmlDoc.CreateElement("Key");
            childNode.InnerText = lic_model.Key;
            rootNode.AppendChild(childNode);

            childNode = xmlDoc.CreateElement("ExpireDate");
            childNode.InnerText = lic_model.ExpireDate.ToString("yyyy/MM/dd");
            rootNode.AppendChild(childNode);

            string file = "license.lic";
            xmlDoc.Save(file);

            return lic_model.Key;

        }

        private static string GenerateLicenseKey(LicenseModel lic_model, string rsaPrivateKey)
        {
            try
            {
                
                string message = lic_model.LicenseOwner + "#" + lic_model.ExpireDate.ToString("dd.MM.yyyy");

                var converter = new ASCIIEncoding();
                byte[] plainText = converter.GetBytes(message);

                var rsaWrite = new RSACryptoServiceProvider();

                rsaWrite.FromXmlString(rsaPrivateKey);

                byte[] signature = rsaWrite.SignData(plainText, new SHA1CryptoServiceProvider());

                return Convert.ToBase64String(signature);

            }
            catch (Exception)
            {
                return null;
            }



        }

        private static bool VerifyLicenseKey(string license, LicenseModel lic_model, string publickey)
        {
            try
            {
             
                string message = lic_model.LicenseOwner + "#" + lic_model.ExpireDate.ToString("dd.MM.yyyy");

                var converter = new ASCIIEncoding();
                byte[] plainText = converter.GetBytes(message);

                var rsaRead = new RSACryptoServiceProvider();
                rsaRead.FromXmlString(publickey);

                if (rsaRead.VerifyData(plainText,
                                       new SHA1CryptoServiceProvider(),
                                       Convert.FromBase64String(license)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                return false;
            }

        }

    }


}
