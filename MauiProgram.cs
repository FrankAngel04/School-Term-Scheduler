using C971.Pages;
using C971.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Plugin.LocalNotification;

namespace C971;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Register ViewModels
        builder.Services.AddSingleton<AddTermViewModel>();
        builder.Services.AddSingleton<EditTermViewModel>();
        builder.Services.AddSingleton<AddCourseViewModel>();
        builder.Services.AddSingleton<EditCourseViewModel>();
        builder.Services.AddSingleton<AddAssessmentViewModel>();
        builder.Services.AddSingleton<EditAssessmentViewModel>();

        // Register pages
        builder.Services.AddTransient<TermScheduler>();
        builder.Services.AddTransient<AddTerm>();
        builder.Services.AddTransient<EditTerm>();
        builder.Services.AddTransient<AddCourse>();
        builder.Services.AddTransient<EditCourse>();
        builder.Services.AddTransient<CourseDetails>();
        builder.Services.AddTransient<CourseScheduler>();
        builder.Services.AddTransient<AddAssessment>();
        builder.Services.AddTransient<EditAssessment>();
        builder.Services.AddTransient<AssessmentDetails>();
        builder.Services.AddTransient<AssessmentScheduler>();
        
        return builder.Build();
    }
}