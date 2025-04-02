using C971.Classes;
using C971.Helpers;
using C971.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace C971.ViewModels;

public partial class EditTermViewModel(Term term , AddTermViewModel addTermView, DatabaseHelper dbHelper) : ObservableObject
{
    [ObservableProperty]
    private Term _selectedTerm = new()
    {
        Id = term.Id,
        Name = term.Name,
        StartDate = term.StartDate.Date,
        EndDate = term.EndDate.Date
    };
    
    [RelayCommand]
    private async Task SaveTerm()
    {
        if (string.IsNullOrWhiteSpace(SelectedTerm.Name))
        {
            await Application.Current.MainPage.DisplayAlert("Name Error", "Name is required!", "OK");
            return;
        }
        
        if (SelectedTerm.StartDate >= SelectedTerm.EndDate)
        {
            await Application.Current.MainPage.DisplayAlert("End-Date Error", "End date must be after start date!", "OK");
            return;
        }
        
        var courses = await dbHelper.GetCoursesByTermIdAsync(SelectedTerm.Id);
        
        if (!courses.Any())
        {
            await dbHelper.SaveTermAsync(SelectedTerm);
            addTermView.LoadTerms();
            await Shell.Current.GoToAsync("..");
            return;
        }

        var earliestCourseStartDate = courses.Min(c => c.StartDate);
        var latestCourseEndDate = courses.Max(c => c.EndDate);

        if (SelectedTerm.StartDate > earliestCourseStartDate || SelectedTerm.EndDate < latestCourseEndDate)
        {
            await Application.Current.MainPage.DisplayAlert("Date Error", 
                $"The term dates must include all course dates. The earliest course start date is {earliestCourseStartDate:M/d/yyyy}, and the latest course end date is {latestCourseEndDate:M/d/yyyy}.", 
                "OK");
            return;
        }
        
        term.Name = SelectedTerm.Name;
        term.StartDate = SelectedTerm.StartDate.Date;
        term.EndDate = SelectedTerm.EndDate.Date;
        
        await dbHelper.SaveTermAsync(SelectedTerm);

        addTermView.LoadTerms();
        
        await Shell.Current.GoToAsync("..");
    }
}