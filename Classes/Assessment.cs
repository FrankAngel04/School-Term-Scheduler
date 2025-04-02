using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace C971.Classes;

public partial class Assessment : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public int CourseId { get; set; }

    [ObservableProperty] 
    private string type;

    [ObservableProperty] 
    private string name;

    [ObservableProperty] 
    private DateTime startDate;

    [ObservableProperty] 
    private DateTime endDate;

    [ObservableProperty]
    private bool notify;
}