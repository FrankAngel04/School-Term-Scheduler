using System;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace C971.Classes;

public partial class Term : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [ObservableProperty] 
    private string name;

    [ObservableProperty] 
    private DateTime startDate;

    [ObservableProperty] 
    private DateTime endDate;
}