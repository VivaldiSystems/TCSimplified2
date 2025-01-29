using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security.Cryptography;  // Add this to the top of utility.cs
using System.IO;

namespace RibbonWin
{

    public static class Globals
    {
        //public static string DBConnection = "";

        //User Information Globals
        public static string StoreNo = "1";
        public static string StoreName = "";
        public static string StoreCity = "";
        public static string UserName = "";
        public static string UserID = "";
        public static bool EnableCustomer = false;
    }



    class Utility
    {
        private static StringBuilder sbListControls;
        public void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {

            if (sender is TextBox textBox)
            {
                // Use a regular expression to format the phone number
                if (Regex.IsMatch(textBox.Text, @"^\d{10}$"))
                {
                    textBox.Text = Regex.Replace(textBox.Text, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");
                }

            }



        }




        public static bool Validate(Window w)
        {
            //Validate for Required
            Boolean blnSave = true;


            foreach (var tb in Utility.FindVisualChildren<TextBox>(w).Where(tb => tb.Text == String.Empty))
            {
                if (tb.Tag != null && tb.Tag.ToString().ToLower().IndexOf("required") > -1)
                {
                    tb.Background = Brushes.Yellow;
                    blnSave = false;
                }

            }

            foreach (var tb in Utility.FindVisualChildren<TextBox>(w).Where(tb => tb.Text != String.Empty))
            {

                if (tb.Tag != null && tb.Tag.ToString().ToLower().IndexOf("phone") > -1)
                {

                    tb.Text = FormatPhoneNumber(tb.Text);


                }
            }

            return blnSave;

        }

        public static bool IsDigit(string svalue)
        {
            string numbers = "0123456789";
            if (numbers.Contains(svalue))
                return true;
            else
                return false;
        }
        private static string FormatPhoneNumber(string phoneNumber)
        {
            // Use a regular expression to format the phone number
            if (Regex.IsMatch(phoneNumber, @"^\d{10}$"))
            {
                return Regex.Replace(phoneNumber, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");
            }
            else
            {
                // Handle invalid phone number format

                return phoneNumber;
            }
        }
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var children = child as T;
                if (children != null)
                {
                    yield return children;
                }

                foreach (var childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }

        public static StringBuilder GetVisualTreeInfo(Visual element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(String.Format("Element {0} is null !", element.ToString()));
            }

            sbListControls = new StringBuilder();

            GetControlsList(element, 0);

            return sbListControls;
        }

        private static void GetControlsList(Visual control, int level)
        {
            const int indent = 4;
            int ChildNumber = VisualTreeHelper.GetChildrenCount(control);

            for (int i = 0; i <= ChildNumber - 1; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(control, i);

                sbListControls.Append(new string(' ', level * indent));
                sbListControls.Append(v.GetType());
                sbListControls.Append(Environment.NewLine);

                if (VisualTreeHelper.GetChildrenCount(v) > 0)
                {
                    GetControlsList(v, level + 1);
                }
            }
        }


        public static bool IsInjection(string inputText)
        {





            List<string> list = new List<string>();

            list.Add("insert");
            list.Add("select");
            list.Add("update");
            list.Add("drop");
            list.Add("join");
            list.Add("delete");
            list.Add("alter");
            list.Add("create");
            list.Add("truncate");
            list.Add(" or ");
            list.Add("'");


            inputText = inputText.ToLower();

            foreach (string s in list)
            {
                if (inputText.IndexOf(s) > -1)
                {
                    return true;
                }


            }


            return false;

        }


        public static byte[] GenerateRandomKey(int keySize)
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                //byte[] key = new byte[keySize / 8];
                //randomNumberGenerator.GetBytes(key);
                byte[] key = { 22, 14, 4, 22, 44, 124, 42, 55, 25, 66, 22, 33, 24, 41, 54, 65, 23, 43, 23, 22, 14, 4, 22, 44, 124, 42, 55, 25, 66, 22, 33, 24 };
                return key;
            }
        }

        // Encrypts a string using AES
        public static string Encrypt(string plainText)
        {
            int keySize = 256;
            byte[] key = GenerateRandomKey(keySize);


            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = keySize;
                aesAlg.Key = key;
                aesAlg.IV = new byte[aesAlg.BlockSize / 8]; // IV size is same as the block size

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Decrypts a string using AES
        public static string Decrypt(string cipherText)
        {

            int keySize = 256;
            byte[] key = GenerateRandomKey(keySize);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = keySize;
                aesAlg.Key = key;
                aesAlg.IV = new byte[aesAlg.BlockSize / 8]; // IV size is same as the block size

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    } // for class
} // for namespace
