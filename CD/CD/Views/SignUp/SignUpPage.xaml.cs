﻿using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using CD.Views.Login;
using CD.Helper;
using Xamarin.Forms;
using System;
using System.Text.RegularExpressions;
using com.sun.tools.javac.util;
using System.Linq;

namespace CD.Views.SignUp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage: ContentPage
    {
        IFirebaseRegister auth;
        readonly FireBaseHelperStudent firebaseStudent = new FireBaseHelperStudent();

        public SignUpPage()
        {
            InitializeComponent();
            auth = DependencyService.Get<IFirebaseRegister>();
        }
        private async void LoginPage(object sender, System.EventArgs e)
        {
            // not allowing the user to use the back button from the phone
            Application.Current.MainPage = new LogIn();
            await Navigation.PopToRootAsync(true);
        }
        private async void RegiterNewUser(object sender, EventArgs e)
        {
            signup_button.IsEnabled = false;
            bool validate = true;
            string pattern = null;
            pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
            if (string.IsNullOrEmpty(NameEntry.Text) && validate)
            {
                await DisplayAlert("Incorrect name", "Please enter a name to continue", "OK");
                validate = false;
            }
            if (string.IsNullOrEmpty(College_University.Text) && validate)
            {
                await DisplayAlert("Incorrect institute", "Please enter your institute to continue", "OK");
                validate = false;
            }
            // cheking if  the email is valid
            if (validate)
            {
                if (string.IsNullOrEmpty(SignUpEmailEntry.Text))
                {
                    await DisplayAlert("Incorrect email", "Please enter your email", "OK");
                    validate = false;
                }
                else if (!Regex.IsMatch(this.SignUpEmailEntry.Text, pattern) && validate)
                {
                    await DisplayAlert("Incorrect email", "Please enter a valid email", "OK");
                    validate = false;
                }
            }
            if  (validate)
            {
                if (string.IsNullOrEmpty(PasswordEntry.Text) && string.IsNullOrEmpty(ConfirmPasswordEntry.Text))
                {
                    await DisplayAlert("Incorrect passwords", "Please enter a password", "Ok");
                    validate = false;
                }
                if (!passwordMatch(PasswordEntry.Text, ConfirmPasswordEntry.Text) && validate)
                {
                    await DisplayAlert("Incorrect passwords", "The passwords entered do not match. \nThe password requires at least 6 characters", "Ok");
                    validate = false;
                }
                if (validate && !string.IsNullOrEmpty(PasswordEntry.Text) && PasswordEntry.Text.Length < 6)
                {
                    await DisplayAlert("Incorrect password", "The password must have at least 6 characters", "ok");
                    validate = false;
                }
            }
            
            if (validate)
            {
                //System.Console.WriteLine("=====================================" + SignUpEmailEntry.Text + " " + PasswordEntry.Text);
                string Token = await auth.RegisterWithEmailAndPassword(SignUpEmailEntry.Text, PasswordEntry.Text);
                if (!string.IsNullOrEmpty(Token) && Token != "existing")
                {
                    await DisplayAlert("Account created", "Please verify your email", "ok");
                    //App.UserUID = authDeleteAccount.UserUID();
                    AddUserDetails(NameEntry.Text, College_University.Text, SignUpEmailEntry.Text);
                    App.UserUID = "";
                    Application.Current.MainPage = new LogIn();
                    await Navigation.PopToRootAsync(true);
                }
                else if (Token == "existing")
                {
                    await DisplayAlert("Email already used", "This email is already used \nIf you don't remeber your password go to 'Forgot Password' page", "ok");
                    await Navigation.PushAsync(new LogIn());
                }
                else
                {
                    await DisplayAlert("Error", "Please try again", "ok");
                }
            }
            signup_button.IsEnabled = true;
        }

        private bool passwordMatch(string password1, string password2)
        {
            return password1 == password2;
        }

        private async void AddUserDetails(string Name, string UC, string Email)
        {
            await firebaseStudent.AddStudent(App.UserUID, Name, UC, Email);
        }
    }
}