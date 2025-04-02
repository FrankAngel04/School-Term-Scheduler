using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace C971.Classes;

public partial class Course : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public int TermId { get; set; }
    
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string status;

    [ObservableProperty]
    private DateTime startDate;
    
    [ObservableProperty]
    private DateTime endDate;

    [ObservableProperty]
    private bool notify;

    [ObservableProperty]
    private string instructor;

    [ObservableProperty]
    private string instructorPhone;

    [ObservableProperty]
    private string instructorEmail;

    [ObservableProperty]
    private string notes;
}
