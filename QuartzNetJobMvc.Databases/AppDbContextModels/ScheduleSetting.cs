using System;
using System.Collections.Generic;

namespace QuartzNetJobMvc.Databases.AppDbContextModels;

public partial class ScheduleSetting
{
    public int Id { get; set; }

    public string ServiceName { get; set; } = null!;

    public string JobType { get; set; } = null!;

    public string CronExpression { get; set; } = null!;
}
