using System.Text.RegularExpressions;
using C971.Classes;
using C971.Helpers;
using C971.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;


namespace C971.ViewModels;

public partial class EditCourseViewModel(Term term, Course course, AddCourseViewModel addCourseVm, DatabaseHelper dbHelper) : ObservableObject
{
    [ObservableProperty]
    private Term _selectedTerm = term;
    
    [ObservableProperty]
    private Course _selectedCourse = new()
    {
        Id = course.Id,
        Name = course.Name,
        Status = course.Status,
        StartDate = course.StartDate.Date,
        EndDate = course.EndDate.Date,
        Notify = course.Notify,
        Instructor = course.Instructor,
        InstructorPhone = course.InstructorPhone,
        InstructorEmail = course.InstructorEmail,
        Notes = course.Notes,
        TermId = course.TermId
    };
    
    [ObservableProperty]
    private List<string> _statusList = ["Not Started", "Plan To Take", "In Progress", "Dropped", "Complete"];
    
    [RelayCommand]
    private async Task SaveCourse()
    {
        if (string.IsNullOrWhiteSpace(SelectedCourse.Name))
        {
            await Application.Current.MainPage.DisplayAlert("Course Name Error", "Course name is required!", "OK");
            return;
        }
        
        if (SelectedCourse.StartDate >= SelectedCourse.EndDate)
        {
            await Application.Current.MainPage.DisplayAlert("End Date Error", $"End date must be after start date {SelectedCourse.StartDate:M/d/yyyy}!", "OK");
            return;
        }

        if (SelectedCourse.StartDate < SelectedTerm.StartDate || SelectedCourse.StartDate > SelectedTerm.EndDate)
        {
            await Application.Current.MainPage.DisplayAlert("Date Error", $"Course Start dates must be within the term date {SelectedTerm.StartDate:M/d/yyyy} and {SelectedTerm.EndDate:M/d/yyyy}!", "OK");
            return;
        }
            
        if (SelectedCourse.EndDate < SelectedTerm.StartDate || SelectedCourse.EndDate > SelectedTerm.EndDate)
        {
            await Application.Current.MainPage.DisplayAlert("Date Error", $"Course End dates must be within the term date {SelectedTerm.StartDate:M/d/yyyy} and {SelectedTerm.EndDate:M/d/yyyy}!", "OK");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(SelectedCourse.Instructor))
        {
            await Application.Current.MainPage.DisplayAlert("Instructor Name Error", "Instructor name is required!", "OK");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(SelectedCourse.InstructorPhone) || Regex.Replace(SelectedCourse.InstructorPhone, @"\D", "").Length != 10)
        {
            await Application.Current.MainPage.DisplayAlert("Phone Error", "10 digit phone is required! Ex: (xxx)xxx-xxxx", "OK");
            return;
        }
        
        const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (string.IsNullOrWhiteSpace(SelectedCourse.InstructorEmail) || !Regex.IsMatch(SelectedCourse.InstructorEmail, emailPattern))
        {
            await Application.Current.MainPage.DisplayAlert("Email Error", "Email is required! Ex: Example@domain.com", "OK");
            return;
        }
        
        course.Name = SelectedCourse.Name;
        course.Status = SelectedCourse.Status;
        course.StartDate = SelectedCourse.StartDate.Date;
        course.EndDate = SelectedCourse.EndDate.Date;
        course.Notify = SelectedCourse.Notify;
        course.Instructor = SelectedCourse.Instructor;
        course.InstructorPhone = SelectedCourse.InstructorPhone;
        course.InstructorEmail = SelectedCourse.InstructorEmail;
        course.Notes = SelectedCourse.Notes;
        
        await dbHelper.SaveCourseAsync(SelectedCourse);
        
        if (SelectedCourse.Notify)
        {
            var today = DateTime.Now.Date;
            NotificationHelper.CancelGroupedCourseNotifications("combined");
            await NotificationHelper.ScheduleCourseNotification(await dbHelper.GetCoursesAsync(), today);
        }
        else
        {
            NotificationHelper.CancelCourseNotifications(SelectedCourse);
        }
        
        addCourseVm.LoadCourses();
        await Shell.Current.GoToAsync("..");
    }
    
    [RelayCommand]
    private async Task SaveNotes()
    {
        await dbHelper.SaveCourseAsync(SelectedCourse);
        addCourseVm.LoadCourses();
    }
    
    [RelayCommand]
    private async Task ShareNotes()
    {
        if (!string.IsNullOrWhiteSpace(SelectedCourse.Notes))
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = SelectedCourse.Notes,
                Title = "Share Course Notes"
            });
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("No Notes", "There are no notes to share.", "OK");
        }
    }
}