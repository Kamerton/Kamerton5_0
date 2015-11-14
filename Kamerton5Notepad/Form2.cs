using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kamerton50_x86
{
    public partial class Form2 : Form
    {
        Form opener;
        public Form2(Form parentForm)
        {
            InitializeComponent();
            opener = parentForm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            opener.Close();
            this.Close();
        }
    }
}
