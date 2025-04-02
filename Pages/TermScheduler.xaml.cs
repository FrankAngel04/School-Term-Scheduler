using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C971.Classes;
using C971.Helpers;
using C971.ViewModels;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;

namespace C971.Pages;

public partial class TermScheduler : ContentPage
{
    private readonly AddTermViewModel _addTermVm;
    private readonly DatabaseHelper _dbHelper;
    
    public TermScheduler(AddTermViewModel addTermVm)
    {
        InitializeComponent();
        _addTermVm = addTermVm;
        BindingContext = _addTermVm;
        _dbHelper = new DatabaseHelper();
        
        SeedDataIfNecessary();
    }
    
    private async void AddTermButton_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddTerm(_addTermVm));
    }
    
    private async void SeedDataIfNecessary()
    {
        var existingTerms = await _dbHelper.GetTermsAsync();
        if (existingTerms.Count == 0)
        {
            await SeedDataAsync();
            _addTermVm.LoadTerms();
        }
    }

    private async Task SeedDataAsync()
    {
        var term = new Term
        {
            Name = "Term 1",
            StartDate = new DateTime(2025, 1, 1).Date,
            EndDate = new DateTime(2025, 6, 30).Date
        };
        await _dbHelper.SaveTermAsync(term);

        var course = new Course
        {
            Name = "Course 101",
            Status = "Not Started",
            StartDate = new DateTime(2025, 1, 1).Date,
            EndDate = new DateTime(2025, 1, 31).Date,
            Notify = true,
            Instructor = "Anika Patel",
            InstructorEmail = "anika.patel@strimeuniversity.edu",
            InstructorPhone = "(555)123-4567",
            TermId = term.Id
        };
        await _dbHelper.SaveCourseAsync(course);

        var objective = new Assessment
        {
            Name = "Course Test",
            StartDate = new DateTime(2025, 1, 5).Date,
            EndDate = new DateTime(2025, 3, 5).Date,
            Type = "Objective Assessment",
            CourseId = course.Id
        };
        await _dbHelper.SaveAssessmentAsync(objective);

        var performance = new Assessment
        {
            Name = "Course Project",
            StartDate = new DateTime(2025, 1, 10).Date,
            EndDate = new DateTime(2025, 3, 10).Date,
            Type = "Performance Assessment",
            CourseId = course.Id
        };
        await _dbHelper.SaveAssessmentAsync(performance);
    }
}