using C971.Helpers;
using C971.ViewModels;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;

namespace C971;

public partial class App : Application
{
    private DateTime _lastCheckedTime = DateTime.Now;
    private DateTime _lastCheckedDate = DateTime.Now.Date;
    private readonly DatabaseHelper _dbHelper = new();

    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();

        StartBackgroundNotificationCheck();
    }
    
    private void StartBackgroundNotificationCheck()
    {
        ScheduleNotificationsForExistingData();

        Device.StartTimer(TimeSpan.FromSeconds(5), () =>
        {
            CheckTime();
            return true;
        });
    }
    
    private async void ScheduleNotificationsForExistingData()
    {
        var today = DateTime.Now.Date;

        var allCourses = await _dbHelper.GetCoursesAsync();
        var allAssessments = await _dbHelper.GetAssessmentsAsync();

        NotificationHelper.CancelGroupedCourseNotifications("combined");
        await NotificationHelper.ScheduleCourseNotification(allCourses, today);

        NotificationHelper.CancelGroupedAssessmentNotifications("combined");
        await NotificationHelper.ScheduleAssessmentNotification(allAssessments, today);
    }
    
    private async void CheckTime()
    {
        DateTime currentTime = DateTime.Now;
        DateTime currentDate = currentTime.Date;

        if (currentTime < _lastCheckedTime && currentTime.TimeOfDay < new TimeSpan(7, 0, 0))
        {
            await Reset7AmNotification();
        }
        else if (currentDate != _lastCheckedDate)
        {
            await Reset7AmNotification();
        }

        _lastCheckedTime = currentTime;
        _lastCheckedDate = currentDate;
    }

    private async Task Reset7AmNotification()
    {
        var today = DateTime.Now.Date;

        var allCourses = await _dbHelper.GetCoursesAsync();
        var allAssessments = await _dbHelper.GetAssessmentsAsync();

        NotificationHelper.CancelGroupedCourseNotifications("combined");
        await NotificationHelper.ScheduleCourseNotification(allCourses, today);

        NotificationHelper.CancelGroupedAssessmentNotifications("combined");
        await NotificationHelper.ScheduleAssessmentNotification(allAssessments, today);
    }
}
