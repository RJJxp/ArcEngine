﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArcEngineProgram
{
    public partial class FormDistance : Form
    {
        public FormDistance()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GlobalData.dist = Convert.ToDouble(textBox1.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FormDistance_Load(object sender, EventArgs e)
        {

        }
    }
}
