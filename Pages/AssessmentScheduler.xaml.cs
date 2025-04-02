using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C971.ViewModels;

namespace C971.Pages;

public partial class AssessmentScheduler : ContentPage
{
    private readonly AddAssessmentViewModel _addAssessmentVm;
    public AssessmentScheduler(AddAssessmentViewModel addAssessmentVm)
    {
        InitializeComponent();
        _addAssessmentVm = addAssessmentVm;
        BindingContext = addAssessmentVm;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        _addAssessmentVm.LoadAssessments();
    }
}