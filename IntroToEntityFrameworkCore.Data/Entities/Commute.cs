namespace IntroToEntityFrameworkCore.Data.Entities;

public class Commute : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    public CommuteType Type { get; set; }
}