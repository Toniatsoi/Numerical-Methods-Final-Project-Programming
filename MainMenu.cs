using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CafeteriaGUI
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }
        public void loadform(object Form)
        {
            if (this.mainPanel.Controls.Count > 0)
                this.mainPanel.Controls.RemoveAt(0);
            Form f = Form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.mainPanel.Controls.Add(f);
            this.mainPanel.Tag = f;
            f.Show();
         
        }

        private void dashboardButton_Click(object sender, EventArgs e)
        {
            loadform(new graphicalMethod());
            
        }

        private void accountButton_Click(object sender, EventArgs e)
        {
            loadform(new incrementalMethod());
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
           loadform(new bisectionMethod());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult yes = MessageBox.Show("Are you sure you want to log out?", " Log out Notification", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (yes == DialogResult.Yes)
            {
                Login form = new Login();
                this.Close();
                form.Show();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            loadform(new MainMenu());
        }

        

        private void mainPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void sidePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void headerPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            loadform(new MainMenu());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            loadform(new regual_falsiMethod());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            loadform(new fixed_positionMethod());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            loadform(new newtonMethod());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            loadform(new secantMethod());
        }
    }
}
