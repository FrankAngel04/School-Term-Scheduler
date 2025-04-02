using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using C971.ViewModels;

namespace C971.Pages;

public partial class EditCourse : ContentPage
{
    public EditCourse(EditCourseViewModel editCourseView)
    {
        InitializeComponent();
        BindingContext = editCourseView;
    }
    
    private void InstructorPhone_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry)
        {
            string phoneNumber = Regex.Replace(entry.Text, @"\D", "");

            if (phoneNumber.Length > 0)
            {
                if (phoneNumber.Length > 3 && phoneNumber.Length <= 6)
                {
                    phoneNumber = $"({phoneNumber.Substring(0, 3)}){phoneNumber.Substring(3)}";
                }
                else if (phoneNumber.Length > 6 && phoneNumber.Length <= 10)
                {
                    phoneNumber = $"({phoneNumber.Substring(0, 3)}){phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6)}";
                }
                else if (phoneNumber.Length > 10)
                {
                    phoneNumber = $"({phoneNumber.Substring(0, 3)}){phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6, 4)}";
                }
                else
                {
                    phoneNumber = $"({phoneNumber}";
                }
            }

            entry.Text = phoneNumber;
            entry.CursorPosition = phoneNumber.Length;
        }
    }
}