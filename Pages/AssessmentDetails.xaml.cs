using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C971.ViewModels;

namespace C971.Pages;

public partial class AssessmentDetails : ContentPage
{
    private readonly EditAssessmentViewModel _editAssessmentVm;
    private readonly AddAssessmentViewModel _addAssessmentVm;
    
    public AssessmentDetails(EditAssessmentViewModel editAssessmentVm, AddAssessmentViewModel addAssessmentVm)
    {
        InitializeComponent();
        _editAssessmentVm = editAssessmentVm;
        _addAssessmentVm = addAssessmentVm;
        BindingContext = _editAssessmentVm;
    }

    private async void DeleteAssessmentButton_OnClicked(object? sender, EventArgs e)
    {
        bool isConfirmed = await Application.Current.MainPage.DisplayAlert(
            "Confirm Delete", 
            $"Are you sure you want to delete '{_editAssessmentVm.SelectedAssessment.Type}'?", 
            "Yes", 
            "No"
        );

        if (isConfirmed)
        {
            await _addAssessmentVm.DbHelper.DeleteAssessmentAsync(_editAssessmentVm.SelectedAssessment);
            _addAssessmentVm.LoadAssessments();
            
            await Shell.Current.GoToAsync("..");
        }
    }

    private async void EditAssessmentButton_OnClicked(object? sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new EditAssessment(new EditAssessmentViewModel(_editAssessmentVm.SelectedCourse ,_editAssessmentVm.SelectedAssessment, _addAssessmentVm, _addAssessmentVm.DbHelper)));
    }
}