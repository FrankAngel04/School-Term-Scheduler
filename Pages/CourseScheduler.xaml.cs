using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C971.Classes;
using C971.ViewModels;
using Microsoft.Maui.Controls;

namespace C971.Pages;

public partial class CourseScheduler : ContentPage
{
    private readonly AddCourseViewModel _addCourseVm;
    private readonly AddTermViewModel _addTermVm;
    public CourseScheduler(AddCourseViewModel addCourseVm, AddTermViewModel addTermVm)
    {
        InitializeComponent();
        _addCourseVm = addCourseVm;
        _addTermVm = addTermVm;
        BindingContext = _addCourseVm;
    }

    private async void AddCourseButton_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddCourse(_addCourseVm));
    }

    private async void EditTermButton_OnClicked(object? sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new EditTerm(new EditTermViewModel(_addCourseVm.SelectedTerm, _addCourseVm.AddTermVm, _addCourseVm.DbHelper)));
    }
    
    private async void DeleteTermButton_OnClicked(object? sender, EventArgs e)
    {
        bool isConfirmed = await Application.Current.MainPage.DisplayAlert(
            "Confirm Delete", 
            $"Are you sure you want to delete '{_addCourseVm.SelectedTerm.Name}'?", 
            "Yes", 
            "No"
        );

        if (isConfirmed)
        {
            await _addCourseVm.DbHelper.DeleteTermAsync(_addCourseVm.SelectedTerm);
            _addTermVm.LoadTerms();
            
            await Shell.Current.GoToAsync("..");
        }
    }
}
