using System;
using System.Collections.ObjectModel;
using C971.Classes;
using C971.Helpers;
using C971.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; 

namespace C971.ViewModels;

public partial class AddTermViewModel : ObservableObject
{
    private readonly DatabaseHelper _dbHelper;

    public AddTermViewModel()
    {
        _dbHelper = new DatabaseHelper();
        LoadTerms();
    }
    
    [ObservableProperty]
    private static ObservableCollection<Term> _termList = [];

    [ObservableProperty] 
    private Term _newTerm = new() { StartDate = DateTime.Now.Date, EndDate = DateTime.Now.AddMonths(6).Date };

    public async void LoadTerms()
    {
        var terms = await _dbHelper.GetTermsAsync();
        TermList = new ObservableCollection<Term>(terms);
    }
    
    [RelayCommand]
    private async Task AddTerm()
    {
        if (string.IsNullOrWhiteSpace(NewTerm.Name))
        {
            await Application.Current.MainPage.DisplayAlert("Name Error", "Name is required!", "OK");
            return;
        }
        
        if (NewTerm.StartDate >= NewTerm.EndDate)
        {
            await Application.Current.MainPage.DisplayAlert("End-Date Error", $"End date must be after start date {NewTerm.StartDate:M/d/yyyy}!", "OK");
            return;
        }
        
        NewTerm.StartDate = NewTerm.StartDate.Date;
        NewTerm.EndDate = NewTerm.EndDate.Date; 
        
        await _dbHelper.SaveTermAsync(NewTerm);
        LoadTerms();
        
        NewTerm = new Term
        {
            Name = string.Empty,
            StartDate = DateTime.Now.Date,
            EndDate = DateTime.Now.AddMonths(6).Date
        };

        await Shell.Current.GoToAsync("..");
    }
    
    [RelayCommand]
    private async Task DeleteTerm(Term term)
    {
        bool isConfirmed = await Application.Current.MainPage.DisplayAlert(
            "Confirm Delete", 
            $"Are you sure you want to delete '{term.Name}'?", 
            "Yes", 
            "No"
        );

        if (isConfirmed)
        {
            await _dbHelper.DeleteTermAsync(term);
            LoadTerms();
        }
    }
    
    [RelayCommand]
    private async Task EditTerm(Term term)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new EditTerm(new EditTermViewModel(term, this, _dbHelper)));
    }

    [RelayCommand]
    private async Task TapTerm(Term term)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new CourseScheduler(new AddCourseViewModel(term, this), this));
    }
}
