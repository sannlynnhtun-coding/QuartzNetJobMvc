using System;
using System.Collections.Generic;

namespace QuartzNetJobMvc.Databases.AppDbContextModels;

public partial class ServiceSetting
{
    public int Id { get; set; }

    public string ServiceName { get; set; } = null!;

    public bool IsEnabled { get; set; }
}
