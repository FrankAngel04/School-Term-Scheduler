using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using C971.Classes;
using C971.Helpers;
using C971.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;

namespace C971.ViewModels;

public partial class AddCourseViewModel: ObservableObject
{
    public readonly AddTermViewModel AddTermVm;
    public readonly DatabaseHelper DbHelper;

    public AddCourseViewModel(Term term, AddTermViewModel addTermVm)
    {
        _selectedTerm = term;
        AddTermVm = addTermVm;
        DbHelper = new DatabaseHelper();
        LoadCourses();
    }
    
    [ObservableProperty]
    private Term _selectedTerm;
    
    [ObservableProperty] 
    private static ObservableCollection<Course> _courseList = [];
    
    [ObservableProperty] 
    private Course _newCourse = new() {StartDate = DateTime.Now.Date, EndDate = DateTime.Now.AddDays(1).Date, Status = "Not Started"};
    
    
    [ObservableProperty]
    private List<string> _statusList = ["Not Started", "Plan To Take", "In Progress", "Dropped", "Complete"];
    
    [ObservableProperty]
    private string _selectedStatus = "Not Started";

    public async void LoadCourses()
    {
        var coursesForTerm = await DbHelper.GetCoursesByTermIdAsync(SelectedTerm.Id);
        CourseList = new ObservableCollection<Course>(coursesForTerm);
    }
    
    partial void OnSelectedStatusChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            NewCourse.Status = value;
        }
        else
        {
            NewCourse.Status = "Not Started";
        }
    }
    
    [RelayCommand]
    private async Task AddCourse()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NewCourse.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Course Name Error", "Course name is required!", "OK");
                return;
            }
            
            if (NewCourse.StartDate >= NewCourse.EndDate)
            {
                await Application.Current.MainPage.DisplayAlert("End Date Error", $"End date must be after start date {NewCourse.StartDate:M/d/yyyy}!", "OK");
                return;
            }

            if (NewCourse.StartDate < SelectedTerm.StartDate || NewCourse.StartDate > SelectedTerm.EndDate)
            {
                await Application.Current.MainPage.DisplayAlert("Date Error", $"Course Start dates must be within the term date {SelectedTerm.StartDate:M/d/yyyy} and {SelectedTerm.EndDate:M/d/yyyy}!", "OK");
                return;
            }
            
            if (NewCourse.EndDate < SelectedTerm.StartDate || NewCourse.EndDate > SelectedTerm.EndDate)
            {
                await Application.Current.MainPage.DisplayAlert("Date Error", $"Course End dates must be within the term date {SelectedTerm.StartDate:M/d/yyyy} and {SelectedTerm.EndDate:M/d/yyyy}!", "OK");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(NewCourse.Instructor))
            {
                await Application.Current.MainPage.DisplayAlert("Instructor Name Error", "Instructor name is required!", "OK");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(NewCourse.InstructorPhone) || Regex.Replace(NewCourse.InstructorPhone, @"\D", "").Length != 10)
            {
                await Application.Current.MainPage.DisplayAlert("Phone Error", "10 digit phone is required! Ex: (xxx)xxx-xxxx", "OK");
                return;
            }
            
            const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrWhiteSpace(NewCourse.InstructorEmail) || !Regex.IsMatch(NewCourse.InstructorEmail, emailPattern))
            {
                await Application.Current.MainPage.DisplayAlert("Email Error", "Email is required! Ex: Example@domain.com", "OK");
                return;
            }
            
            NewCourse.StartDate = NewCourse.StartDate.Date;
            NewCourse.EndDate = NewCourse.EndDate.Date;
            
            NewCourse.TermId = SelectedTerm.Id;
            
            await DbHelper.SaveCourseAsync(NewCourse);
            LoadCourses();
            
            if (NewCourse.Notify)
            {
                var today = DateTime.Now.Date;
                NotificationHelper.CancelGroupedCourseNotifications("combined");
                await NotificationHelper.ScheduleCourseNotification(await DbHelper.GetCoursesAsync(), today);
            }
            
            NewCourse = new Course
            {
                Name = string.Empty,
                Status = "Not Started",
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.AddDays(1).Date,
                Notify = false,
                Instructor = string.Empty,
                InstructorPhone = string.Empty,
                InstructorEmail = string.Empty,
                Notes = string.Empty
            };

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
        }
    }
    
    [RelayCommand]
    private async Task DeleteCourse(Course course)
    {
        bool isConfirmed = await Application.Current.MainPage.DisplayAlert(
            "Confirm Delete", 
            $"Are you sure you want to delete '{course.Name}'?", 
            "Yes", 
            "No"
        );

        if (isConfirmed)
        {
            await DbHelper.DeleteCourseAsync(course);
            LoadCourses();
        }
    }
    
    [RelayCommand]
    private async Task EditCourse(Course course)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new EditCourse(new EditCourseViewModel(SelectedTerm, course, this, DbHelper)));
    }

    [RelayCommand]
    private async Task TapCourse(Course course)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new CourseDetails(new EditCourseViewModel(SelectedTerm, course, this, DbHelper), this));
    }
}