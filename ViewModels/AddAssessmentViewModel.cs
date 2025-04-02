using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using C971.Classes;
using C971.Helpers;
using C971.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;

namespace C971.ViewModels;

public partial class AddAssessmentViewModel : ObservableObject
{
    public readonly DatabaseHelper DbHelper;

    public AddAssessmentViewModel(Course course, string assessmentType)
    {
        _selectedCourse = course;
        DbHelper = new DatabaseHelper();
        _newAssessment.Type = assessmentType;
        LoadAssessments();
    }
    
    [ObservableProperty]
    private Course _selectedCourse;
    
    [ObservableProperty] 
    private static ObservableCollection<Assessment> _assessmentList = [];
    
    [ObservableProperty] 
    private Assessment _newAssessment = new() {StartDate = DateTime.Now.Date, EndDate = DateTime.Now.AddDays(1).Date};

    [ObservableProperty]
    private bool _isObjectiveAdded = false;
    
    [ObservableProperty]
    private bool _isPerformanceAdded = false;
    
    public async void LoadAssessments()
    {
        var assessments = await DbHelper.GetAssessmentsByCourseIdAsync(SelectedCourse.Id);
        AssessmentList = new ObservableCollection<Assessment>(assessments);
    }

    [RelayCommand]
    private async Task PickAssessment()
    {
        var result = await Application.Current.MainPage.DisplayActionSheet("Select Assessment Type", "Cancel", null, "Objective", "Performance");
        var existingAssessments = await DbHelper.GetAssessmentsByCourseIdAsync(SelectedCourse.Id);
        
        if (result == "Objective")
        {
            if (existingAssessments.Any(a => a.Type == "Objective Assessment"))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Objective Assessment already exists for this course.", "OK");
                return;
            }

            await Application.Current.MainPage.Navigation.PushAsync(new AddAssessment(new AddAssessmentViewModel(SelectedCourse, "Objective Assessment")));
        }
        else if (result == "Performance")
        {
            if (existingAssessments.Any(a => a.Type == "Performance Assessment"))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Performance Assessment already exists for this course.", "OK");
                return;
            }

            await Application.Current.MainPage.Navigation.PushAsync(new AddAssessment(new AddAssessmentViewModel(SelectedCourse, "Performance Assessment")));
        }
    }
    
    [RelayCommand]
    private async Task AddAssessment()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NewAssessment.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Course Name Error", "Assessment name is required!", "OK");
                return;
            }
            
            if (NewAssessment.StartDate >= NewAssessment.EndDate)
            {
                await Application.Current.MainPage.DisplayAlert("End Date Error", $"End date must be after start date {NewAssessment.StartDate:M/d/yyyy}!", "OK");
                return;
            }
            
            if (NewAssessment.StartDate < SelectedCourse.StartDate || NewAssessment.StartDate > SelectedCourse.EndDate)
            {
                await Application.Current.MainPage.DisplayAlert("Date Error", $"Assessment Start dates must be within the course date {SelectedCourse.StartDate:M/d/yyyy} and {SelectedCourse.EndDate:M/d/yyyy}!", "OK");
                return;
            }
            
            if (NewAssessment.EndDate < SelectedCourse.StartDate || NewAssessment.EndDate > SelectedCourse.EndDate)
            {
                await Application.Current.MainPage.DisplayAlert("Date Error", $"Assessment End dates must be within the course date {SelectedCourse.StartDate:M/d/yyyy} and {SelectedCourse.EndDate:M/d/yyyy}!", "OK");
                return;
            }
            
            NewAssessment.StartDate = NewAssessment.StartDate.Date;
            NewAssessment.EndDate = NewAssessment.EndDate.Date;
            
            NewAssessment.CourseId = SelectedCourse.Id;
            
            await DbHelper.SaveAssessmentAsync(NewAssessment);
            LoadAssessments();

            if (NewAssessment.Notify)
            {
                var today = DateTime.Now.Date;
                NotificationHelper.CancelGroupedAssessmentNotifications("combined");
                await NotificationHelper.ScheduleAssessmentNotification(await DbHelper.GetAssessmentsAsync(), today);
            }
            
            NewAssessment = new Assessment
            {
                Name = string.Empty,
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.AddDays(1).Date,
                Notify = false
            };

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
        }
    }
    
    [RelayCommand]
    private async Task DeleteAssessment(Assessment assessment)
    {
        bool isConfirmed = await Application.Current.MainPage.DisplayAlert(
            "Confirm Delete", 
            $"Are you sure you want to delete '{assessment.Type}'?", 
            "Yes", 
            "No"
        );

        if (isConfirmed)
        {
            await DbHelper.DeleteAssessmentAsync(assessment);
            LoadAssessments();
        }
    }
    
    [RelayCommand]
    private async Task EditAssessment(Assessment assessment)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new EditAssessment(new EditAssessmentViewModel(SelectedCourse, assessment, this, DbHelper)));
    }

    [RelayCommand]
    private async Task TapAssessment(Assessment assessment)
    {
       await Application.Current.MainPage.Navigation.PushAsync(new AssessmentDetails(new EditAssessmentViewModel(SelectedCourse ,assessment, this, DbHelper), this));
    }
}