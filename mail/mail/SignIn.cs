using mail;
using MailKit;
using MailKit.Net.Imap;
using System;
using System.Net.Mail;
using System.Threading;
using System.Windows.Forms;

namespace mail

{
    public partial class SignIn : Form
    {
        public SignIn()
        {
            InitializeComponent();
        }

        bool IsValidEmail(string email)
        {
            try
            {
                MailAddress addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch { return false; }
        }

        private void btn_SignIn_Click(object sender, EventArgs e)
        {
            string imapServer = txt_IMAP.Text;
            int port = Convert.ToInt32(txt_Port.Text);

            string email = txt_Email.Text;
            string password = txt_Password.Text;

            if (string.IsNullOrWhiteSpace(imapServer))
            {
                MessageBox.Show("SMTP Server không được để trống");
                txt_IMAP.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Email không được để trống");
                txt_Email.Focus();
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Địa chỉ email không hợp lệ");
                txt_Email.Clear();
                txt_Password.Clear();
                txt_Email.Focus();
                return;
            }

            Loading frmLoading = new Loading();
            Thread loading = new Thread(new ThreadStart(() => {
                try { frmLoading.ShowDialog(); }
                catch { }
            }));

            try
            {
                loading.Start();
                MailBox frmMailBox = new MailBox(new MailRepository(
                    imapServer, port, chb_SSL.Checked, email, password
                ));
                loading.Abort();
                Hide();

                frmMailBox.TopMost = true;
                frmMailBox.ShowDialog();
                Close();
            }
            catch (Exception error)
            {
                loading.Abort();
                MessageBox.Show(error.Message);
            }
        }

        private void txt_Email_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btn_SignIn_Click(this, new EventArgs());
        }

        private void txt_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btn_SignIn_Click(this, new EventArgs());
        }

        private void btn_Show_Password_MouseDown(object sender, MouseEventArgs e)
        {
            txt_Password.UseSystemPasswordChar = false;
        }

        private void btn_Show_Password_MouseUp(object sender, MouseEventArgs e)
        {
            txt_Password.UseSystemPasswordChar = true;
        }

        
    }
}
