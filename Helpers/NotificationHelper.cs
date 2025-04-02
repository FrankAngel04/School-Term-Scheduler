using Plugin.LocalNotification;
using System;
using System.Text;
using C971.Classes;
using Plugin.LocalNotification.AndroidOption;

namespace C971.Helpers
{
    public static class NotificationHelper
    {
        public static async Task ScheduleCourseNotification(List<Course> courses, DateTime date)
        {
            var notifyTime = new DateTime(date.Year, date.Month, date.Day, 7, 0, 0);

            var coursesStartingToday = courses.Where(c => c.StartDate.Date == date.Date && c.Notify).Select(c => c.Name).ToList();
            var coursesEndingToday = courses.Where(c => c.EndDate.Date == date.Date && c.Notify).Select(c => c.Name).ToList();

            if (coursesStartingToday.Any() || coursesEndingToday.Any())
            {
                var title = "Courses";
                var description = new StringBuilder();
                bool hasPreviousSection = false;

                if (coursesStartingToday.Any())
                {
                    description.AppendLine("Courses Starting Today:");
                    foreach (var course in coursesStartingToday)
                    {
                        description.AppendLine($"{course}");
                    }
                    hasPreviousSection = true;
                }

                if (coursesEndingToday.Any())
                {
                    if (hasPreviousSection) description.AppendLine();
                    description.AppendLine("Courses Ending Today:");
                    foreach (var course in coursesEndingToday)
                    {
                        description.AppendLine($"{course}");
                    }
                }

                var notification = new NotificationRequest
                {
                    NotificationId = date.DayOfYear * 100 + 1,
                    Title = title,
                    Description = description.ToString(),
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime
                    }
                };

                await LocalNotificationCenter.Current.Show(notification);
            }
        }

        public static async Task ScheduleAssessmentNotification(List<Assessment> assessments, DateTime date)
        {
            var notifyTime = new DateTime(date.Year, date.Month, date.Day, 7, 0, 0);

            var assessmentsStartingToday = assessments.Where(a => a.StartDate.Date == date.Date && a.Notify).Select(a => new { a.Name, a.Type }).ToList();
            var assessmentsEndingToday = assessments.Where(a => a.EndDate.Date == date.Date && a.Notify).Select(a => new { a.Name, a.Type }).ToList();

            if (assessmentsStartingToday.Any() || assessmentsEndingToday.Any())
            {
                var title = "Assessments";
                var description = new StringBuilder();
                bool hasPreviousSection = false;

                if (assessmentsStartingToday.Any())
                {
                    description.AppendLine("Assessments Starting Today:");
                    foreach (var assessment in assessmentsStartingToday)
                    {
                        description.AppendLine($"({assessment.Type}) {assessment.Name}");
                    }
                    hasPreviousSection = true;
                }

                if (assessmentsEndingToday.Any())
                {
                    if (hasPreviousSection) description.AppendLine();
                    description.AppendLine("Assessments Ending Today:");
                    foreach (var assessment in assessmentsEndingToday)
                    {
                        description.AppendLine($"({assessment.Type}) {assessment.Name}");
                    }
                }

                var notification = new NotificationRequest
                {
                    NotificationId = date.DayOfYear * 100 + 2,
                    Title = title,
                    Description = description.ToString(),
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime
                    }
                };

                await LocalNotificationCenter.Current.Show(notification);
            }
        }

        
        public static void CancelCourseNotifications(Course course)
        {
            int startNotificationId = course.Id * 1000 + 1;
            int endNotificationId = course.Id * 1000 + 2;

            LocalNotificationCenter.Current.Cancel(startNotificationId);
            LocalNotificationCenter.Current.Cancel(endNotificationId);
        }

        public static void CancelAssessmentNotifications(Assessment assessment)
        {
            int startNotificationId = assessment.Id * 1000 + 101;
            int endNotificationId = assessment.Id * 1000 + 102;

            LocalNotificationCenter.Current.Cancel(startNotificationId);
            LocalNotificationCenter.Current.Cancel(endNotificationId);
        }
        
        public static void CancelGroupedCourseNotifications(string type)
        {
            int notificationId = 1;
            LocalNotificationCenter.Current.Cancel(notificationId);
        }

        public static void CancelGroupedAssessmentNotifications(string type)
        {
            int notificationId = 2;
            LocalNotificationCenter.Current.Cancel(notificationId);
        }
    }
}
