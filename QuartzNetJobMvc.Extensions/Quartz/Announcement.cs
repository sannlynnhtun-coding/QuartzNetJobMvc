namespace QuartzNetJobMvc.Extensions.Quartz;

public class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsSent { get; set; } = false;
}