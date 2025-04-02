using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C971.ViewModels;

namespace C971.Pages;

public partial class AddAssessment : ContentPage
{
    public AddAssessment(AddAssessmentViewModel addAssessmentVm)
    {
        InitializeComponent();
        BindingContext = addAssessmentVm;
    }
}