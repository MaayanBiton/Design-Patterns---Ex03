﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;
using System.Threading;

namespace FacebookApp
{
    public partial class FacebookForm : Form
    {
        public FacebookForm()
        {
            this.InitializeComponent();
            FacebookAppFacade = new FacebookAppFacade();
            FacebookAppFacade.AppSettings = AppSettings.LoadFromFile();
            this.checkBoxRememberMe.Checked = FacebookAppFacade.AppSettings.RememberMe;
        }

        public FacebookAppFacade FacebookAppFacade { get; set; }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            this.FacebookAppFacade.AppSettings.RememberMe = this.checkBoxRememberMe.Checked;

            if (this.FacebookAppFacade.AppSettings.RememberMe)
            {
                this.FacebookAppFacade.AppSettings.LastAccessToken = this.FacebookAppFacade.m_LoginResult.AccessToken;
            }
            else
            {
                this.FacebookAppFacade.AppSettings.LastAccessToken = null;
            }

            this.FacebookAppFacade.AppSettings.SaveToFile();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (this.FacebookAppFacade.AppSettings.RememberMe && !string.IsNullOrEmpty(this.FacebookAppFacade.AppSettings.LastAccessToken))
            {
                this.FacebookAppFacade.m_LoginResult = FacebookService.Connect(this.FacebookAppFacade.AppSettings.LastAccessToken);
                FacebookAppFacade.m_LoggedInUserProxy = new UserProxy(this.FacebookAppFacade.m_LoginResult.LoggedInUser);
                this.setFormInfo();
            }
        }

        private void fetchUserInfo()
        {
            try
            {
                this.userBindingSource.DataSource = FacebookAppFacade.m_LoggedInUserProxy.LoggedInUser;
                this.checkinsBindingSource.DataSource = FacebookAppFacade.m_LoggedInUserProxy.LoggedInUser.Checkins;
                this.setVisible();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void setFormInfo()
        {
            this.Text = "Logged in as " + this.FacebookAppFacade.m_LoggedInUserProxy.LoggedInUser.Name;
            this.fetchUserInfo();
            new Thread(this.fetchPosts).Start();
        }

        private void fetchPosts()
        {
            this.postAdapterBindingSource.DataSource = FacebookAppFacade.GetPosts();
            if (this.FacebookAppFacade.m_LoggedInUserProxy.LoggedInUser.Posts.Count == 0)
            {
                MessageBox.Show("No Posts to retrieve :(");
            }
        }

        private void setVisible()
        {
            foreach (Control control in this.Controls)
            {
                control.Visible = true;
            }
        }

        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            try
            {
                FacebookAppFacade.LoginAndInit();
                this.setFormInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonPost_Click(object sender, EventArgs e)
        {
            FacebookAppFacade.PostStatus(this.textBoxPost.Text);
        }

        private void checkBoxRememberMe_CheckedChanged(object sender, EventArgs e)
        {
            FacebookAppFacade.AppSettings.RememberMe = this.checkBoxRememberMe.Checked;
        }

        private void buttonSortLikes_Click(object sender, EventArgs e)
        {
            try
            {
                FacebookAppFacade.SortLikes(this.comboBoxSortOption.SelectedIndex);
                this.showChosenFeatureOutput();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void showChosenFeatureOutput()
        {
            this.listBoxFriends.Items.Clear();
            foreach (User friend in FacebookAppFacade)
            {
                this.listBoxFriends.Items.Add(friend.Name);
            }  
        }

        private void matchInitializer(MatchFinder i_Feature)
        {
            this.genderListInit(i_Feature);
            i_Feature.AgeRangeSelected = this.comboBoxAges.SelectedItem.ToString();
        }

        private void genderListInit(MatchFinder i_MatchFinder)
        {
            if (this.checkBoxMale.Checked)
            {
                i_MatchFinder.SelectedGender.Add("Male", true);
            }
            else
            {
                i_MatchFinder.SelectedGender.Add("Male", false);
            }

            if (this.checkBoxFemale.Checked)
            {
                i_MatchFinder.SelectedGender.Add("Female", true);
            }
            else
            {
                i_MatchFinder.SelectedGender.Add("Female", false);
            }
        }

        private void buttonMatch_Click(object sender, EventArgs e)
        {
            try
            {
                FacebookAppFacade.FindMatch(this.checkBoxFemale.Checked, this.checkBoxMale.Checked, this.comboBoxAges.SelectedItem.ToString());
                this.showChosenFeatureOutput();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            FileFormatForm fileFormat = new FileFormatForm(FacebookAppFacade.m_LoggedInUserProxy);
            fileFormat.ShowDialog();
            FacebookAppFacade.m_LoggedInUserProxy.SaveToFileString = this.textBoxToFile.Text;

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FacebookAppFacade.m_LoggedInUserProxy.FileName = this.saveFileDialog1.FileName;
                FacebookAppFacade.m_LoggedInUserProxy.SaveInfoToFile(fileFormat);
            }
        }
    }
}