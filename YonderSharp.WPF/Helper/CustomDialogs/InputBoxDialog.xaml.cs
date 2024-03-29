﻿using System;
using System.Windows;

namespace YonderSharp.WPF.Helper.CustomDialogs
{
    /// <summary>
    /// Interaction logic for InputBoxDialog.xaml
    /// </summary>
    public partial class InputBoxDialog : Window
    {

        /* How to use me:
         
            InputBoxDialog heroNameDialog = new InputBoxDialog(title, question);
            heroNameDialog.ShowDialog();
            if (!(heroNameDialog.DialogResult.HasValue && heroNameDialog.DialogResult.Value))
            {
                return;
            }

            string heroName = heroNameDialog.Answer;
        */


        public InputBoxDialog(string windowTitle, string question, string defaultAnswer = "")
        {
            InitializeComponent();
            Title = windowTitle;
            lblQuestion.Content = question;
            txtAnswer.Text = defaultAnswer;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }

        public string Answer
        {
            get
            {
                return txtAnswer.Text;
            }
        }
    }
}
