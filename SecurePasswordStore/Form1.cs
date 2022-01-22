/*
 * Jonas : https://stackoverflow.com/a/62018445/7275403
 * original Q : https://stackoverflow.com/questions/12657792/how-to-securely-save-username-password-local
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecurePasswordStore
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Login_Click_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                Properties.Settings.Default.username = textBox1.Text;
                Properties.Settings.Default.password = EncryptString(ToSecureString(textBox2.Text));
                Properties.Settings.Default.Save();
            }
            else if (checkBox1.Checked == false)
            {
                Properties.Settings.Default.username = "";
                Properties.Settings.Default.password = "";
                Properties.Settings.Default.Save();
            }
            MessageBox.Show("{\"data\": \"some data\"}", "Login Message Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void Decrypt_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";
            SecureString password = DecryptString(textBox3.Text); // DecryptString(Properties.Settings.Default.password);
            string readable = ToInsecureString(password);
            textBox4.AppendText(readable + Environment.NewLine);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //textBox1.Text = "UserName";
            //textBox2.Text = "Password";
            if (Properties.Settings.Default.username != string.Empty)
            {
                textBox1.Text = Properties.Settings.Default.username;
                checkBox1.Checked = true;
                SecureString password = DecryptString(Properties.Settings.Default.password);
                string readable = ToInsecureString(password);
                textBox2.Text = readable;
            }
            groupBox1.Select();
        }


        static byte[] entropy = Encoding.Unicode.GetBytes("MyOwnSalt");

        public static string EncryptString(SecureString input)
        {
            byte[] encryptedData = ProtectedData.Protect(Encoding.Unicode.GetBytes(ToInsecureString(input)), entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(Convert.FromBase64String(encryptedData), entropy, DataProtectionScope.CurrentUser);
                return ToSecureString(Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        public static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }

        private void Encrypt_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            Properties.Settings.Default.password = EncryptString(ToSecureString(textBox2.Text));
            textBox3.AppendText(Properties.Settings.Default.password.ToString() + Environment.NewLine);
        }

    }
}
