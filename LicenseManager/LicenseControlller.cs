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

                string private_key = rsaWrite.ToXmlString(false);
                string public_key = rsaWrite.ToXmlString(true);

                return new string[] { private_key, public_key };
            }
            catch (Exception)
            {

                return new string[] { };
            }


        }


        public static bool VerifyLicense(out LicenseModel lic_model)
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


                return VerifyLicenseKey(lic_model.Key, lic_model);

            }
            catch (Exception)
            {
                return false;
            }

        }

        public static string GenerateLicense(LicenseModel lic_model)
        {
            lic_model.Key = GenerateLicenseKey(lic_model);

            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("License");
            xmlDoc.AppendChild(rootNode);

            XmlNode childNode = xmlDoc.CreateElement("Product");
            childNode.InnerText = "QUVAM - Quick Variant Maker";
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

        private static string GenerateLicenseKey(LicenseModel lic_model)
        {
            try
            {
                string rsaPrivateKey = "<RSAKeyValue><Modulus>3CzIhvqD1or7WmsSZHiwSo1SQCO+EybV6nIcZBD9Iqx3RshJHVmbYGzqmbwA/bGB9p44CTaMHD7K+5EexuQjtqAiXp60pYJsXZpFgU3jYgasA5T7PIohBqAUbxAzPrWqDtGhqzC92+KgigVZB1b9a+xEIiKg34L2IEBj/afqN7k=</Modulus><Exponent>AQAB</Exponent><P>+OMZ1Y3fi4bPrqKO90mT2UREd7WzH/ziqGWNtF60v28S+KFilW6e0b7tIECnKVaiXjvAexquDqoDAcRER4KCow==</P><Q>4neej0IH+rJN+Sg9J3dWSndRqhReOTbCYlSqVgokWLMCFe/OivAGP5SodqPK3A0bwo8gQVNLFNwTxsQAWFFd8w==</Q><DP>N8TDfCuvJJePn8UDHfwZqfx3Dw/i1E8ZBrzCtODnxWGBMb8P1QYVhlAu2CREkKm99jmTVsJSsCx+Qf4VgqSG2w==</DP><DQ>JUkYe/GdKrNMjycG2oaVWHFIqr3rvXO8kT/rQ3sr/MaMI1x2Hv3hqXoqOk5BSfWGioPSBa9W/zo0r1b5z5Cl7Q==</DQ><InverseQ>2odjhYLY6v+4BsifoKZ7vCkWwlxY7eSxoc/4zzIdC6J80BvGj1SR5CFoYxMc/JimqwrJ1ooUsup+1gXvs4LFZg==</InverseQ><D>OiyK/z8JvpISP52yymEpE0mrxc6r1huYwc1MuxPSDmtLSKR6zQp0B1I/2kbWbDGOiT7cx3JmKsBcmYOKgjqdRJIJvl3lll0gnXDQAzdlABAyPnkFMMhIsFPp9kfNZnPtDY0AuX/B6JX8o0jb/OgGlukALMt3/26QB7BnmcgNPxE=</D></RSAKeyValue>";

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

        private static bool VerifyLicenseKey(string license, LicenseModel lic_model)
        {
            try
            {
                string publickey = "<RSAKeyValue><Modulus>3CzIhvqD1or7WmsSZHiwSo1SQCO+EybV6nIcZBD9Iqx3RshJHVmbYGzqmbwA/bGB9p44CTaMHD7K+5EexuQjtqAiXp60pYJsXZpFgU3jYgasA5T7PIohBqAUbxAzPrWqDtGhqzC92+KgigVZB1b9a+xEIiKg34L2IEBj/afqN7k=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

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
