using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C971.ViewModels;

namespace C971.Pages;

public partial class CourseDetails : ContentPage
{
    private readonly EditCourseViewModel _editCourseVm;
    private readonly AddCourseViewModel _addCourseVm;
    
    public CourseDetails(EditCourseViewModel editCourseVm, AddCourseViewModel addCourseVm)
    {
        InitializeComponent();
        _editCourseVm = editCourseVm;
        _addCourseVm = addCourseVm;
        BindingContext = _editCourseVm;
    }

    private async void EditCourseButton_OnClicked(object? sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new EditCourse(new EditCourseViewModel(_editCourseVm.SelectedTerm, _editCourseVm.SelectedCourse, _addCourseVm, _addCourseVm.DbHelper)));
    }

    private async void DeleteCourseButton_OnClicked(object? sender, EventArgs e)
    {
        bool isConfirmed = await Application.Current.MainPage.DisplayAlert(
            "Confirm Delete", 
            $"Are you sure you want to delete '{_editCourseVm.SelectedCourse.Name}'?", 
            "Yes", 
            "No"
        );

        if (isConfirmed)
        {
            await _addCourseVm.DbHelper.DeleteCourseAsync(_editCourseVm.SelectedCourse);
            _addCourseVm.LoadCourses();
            
            await Shell.Current.GoToAsync("..");
        }
    }

    private async void AssessmentButton_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AssessmentScheduler(new AddAssessmentViewModel(_editCourseVm.SelectedCourse, "Default")));
    }
}