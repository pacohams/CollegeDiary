﻿using System;
using CD.Helper;
using CD.Models;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;

namespace CD.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddMarkToSubject
    {
        private Subject _subject;
        readonly FireBaseHelperMark fireBaseHelper = new FireBaseHelperMark();

        public AddMarkToSubject(Subject subject)
        {
            _subject = subject;
            InitializeComponent();
        }

        // check if the weight of the current CA is not exceeding the overall weight of the CA
        public async Task<bool> Check_CA_Weight(Subject subject, int weight)
        {
            var marks_belonging_to_subject = await fireBaseHelper.GetMarksForSubject(subject.SubjectID);
            double total_CA_all_Marks = 0;

            foreach (Mark m in marks_belonging_to_subject)
            {
                if (m.Category.Equals("Continuous Assessment")) 
                {
                    total_CA_all_Marks += m.Weight; 
                }
            }
            if (total_CA_all_Marks + weight > subject.CA)
            {
                await DisplayAlert("Mark not added", "The assignment percentage is exceeding the total CA percentage", "OK");
                return false;
            }

            return true;
        }

        [Obsolete]
        private async void Save_Mark(object sender, EventArgs e)
        {
            bool validate = true;

            if (string.IsNullOrEmpty(this.mark_name.Text) || string.IsNullOrEmpty(this.weight.Text)
                || string.IsNullOrEmpty(this.result.Text))
            {
                validate = false;
            }

            if (validate) { validate = await Check_CA_Weight(_subject, int.Parse(this.weight.Text)); }

            if (validate)
            {
                int result = Int32.Parse(this.result.Text);
                int weight = Int32.Parse(this.weight.Text);
                var mark = await fireBaseHelper.GetMark(mark_name.Text);
                await fireBaseHelper.AddMark(_subject.SubjectID, mark_name.Text, result, weight, "Continuous Assessment");
                await DisplayAlert("Success", "Your result had been recorded", "OK");

                // refresh the page to show the added mark to the subject
                await Navigation.PushAsync(new SubjectSelected(_subject), false);
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
                await PopupNavigation.RemovePageAsync(this);
            }
            else
            {
                await DisplayAlert("Result not added", "", "OK");
            }
        }

        [Obsolete]
        private void Cancel_Mark(object sender, EventArgs e)
        {
            PopupNavigation.RemovePageAsync(this);
        }
    }
}