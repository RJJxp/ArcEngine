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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormSurvey formSurvey = new FormSurvey();
            this.Hide();
            if (formSurvey.ShowDialog() == DialogResult.OK)
                this.Visible = true;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormContour ContourForm = new FormContour();
            this.Hide();
            if (ContourForm.ShowDialog() == DialogResult.OK)
                this.Visible = true;
            
        }


        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
