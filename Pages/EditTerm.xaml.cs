using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C971.ViewModels;
using Microsoft.Maui.Controls;

namespace C971.Pages;

public partial class EditTerm : ContentPage
{
    public EditTerm(EditTermViewModel editTermView)
    {
        InitializeComponent();
        BindingContext = editTermView;
    }
}