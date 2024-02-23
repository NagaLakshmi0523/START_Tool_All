using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace START_Tool
{
    public partial class MessageForm : Form
    {
        public MessageForm()
        {
            InitializeComponent();
        }

        private void MessageForm_Load(object sender, EventArgs e)
        {

        }
        static MessageForm newMessageBox;
        static string Button_id;

        public static string ShowBox(string txtMessage)
        {
            newMessageBox = new MessageForm();
            newMessageBox.lblMessage.Text = txtMessage;
            newMessageBox.ShowDialog();
            return Button_id;
        }

        public static string ShowBox(string txtMessage, string txtTitle)
        {
            newMessageBox = new MessageForm();
            newMessageBox.lblMessage.Text = txtMessage;
            newMessageBox.Text = txtTitle;
            newMessageBox.ShowDialog();
            return Button_id;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // Button_id = "1";
            this.Dispose();
            //this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            newMessageBox.Dispose();
            Button_id = "2";
        }
    }
}
