using System.Collections.ObjectModel;
using C971.Classes;
using C971.Helpers;
using C971.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;

namespace C971.ViewModels;

public partial class EditAssessmentViewModel(Course course, Assessment assessment, AddAssessmentViewModel addAssessmentVm, DatabaseHelper dbHelper) : ObservableObject
{
    [ObservableProperty]
    private Course _selectedCourse = course;
    
    [ObservableProperty]
    private Assessment _selectedAssessment = new()
    {
        Id = assessment.Id,
        Name = assessment.Name,
        StartDate = assessment.StartDate.Date,
        EndDate = assessment.EndDate.Date,
        Notify = assessment.Notify,
        Type = assessment.Type,
        CourseId = assessment.CourseId
    };
    
    [RelayCommand]
    private async Task SaveAssessment()
    {
        try
        {
            var existingAssessments = await dbHelper.GetAssessmentsByCourseIdAsync(SelectedAssessment.Id);
            
            if (existingAssessments.Any(a => a.Type == SelectedAssessment.Type))
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"{SelectedAssessment.Type} already exists for this course.", "OK");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(SelectedAssessment.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Course Name Error", "Assessment name is required!", "OK");
                return;
            }
            
            if (SelectedAssessment.StartDate >= SelectedAssessment.EndDate)
            {
                await Application.Current.MainPage.DisplayAlert("End Date Error", $"End date must be after start date {SelectedAssessment.StartDate:M/d/yyyy}!", "OK");
                return;
            }

            if (SelectedAssessment.StartDate < SelectedCourse.StartDate || SelectedAssessment.StartDate > SelectedCourse.EndDate)
            {
                await Application.Current.MainPage.DisplayAlert("Date Error", $"Assessment Start dates must be within the course date {SelectedCourse.StartDate:M/d/yyyy} and {SelectedCourse.EndDate:M/d/yyyy}!", "OK");
                return;
            }
            
            if (SelectedAssessment.EndDate < SelectedCourse.StartDate || SelectedAssessment.EndDate > SelectedCourse.EndDate)
            {
                await Application.Current.MainPage.DisplayAlert("Date Error", $"Assessment End dates must be within the course date {SelectedCourse.StartDate:M/d/yyyy} and {SelectedCourse.EndDate:M/d/yyyy}!", "OK");
                return;
            }
            
            assessment.Name = SelectedAssessment.Name;
            assessment.StartDate = SelectedAssessment.StartDate.Date;
            assessment.EndDate = SelectedAssessment.EndDate.Date;
            assessment.Notify = SelectedAssessment.Notify;
            
            await dbHelper.SaveAssessmentAsync(SelectedAssessment);

            if (SelectedAssessment.Notify)
            {
                var today = DateTime.Now.Date;
                NotificationHelper.CancelGroupedAssessmentNotifications("combined");
                await NotificationHelper.ScheduleAssessmentNotification(await dbHelper.GetAssessmentsAsync(), today);
            }
            else
            {
                NotificationHelper.CancelAssessmentNotifications(SelectedAssessment);
            }
            
            addAssessmentVm.LoadAssessments();
            
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
        }
    }
}