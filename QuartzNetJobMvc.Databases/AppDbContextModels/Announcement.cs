using System;
using System.Collections.Generic;

namespace QuartzNetJobMvc.Databases.AppDbContextModels;

public partial class Announcement
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsSent { get; set; }
}
