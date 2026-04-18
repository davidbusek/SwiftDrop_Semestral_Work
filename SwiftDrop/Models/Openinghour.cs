using System;
using System.Collections.Generic;

namespace SwiftDrop.Models;

public partial class Openinghour
{
    public int Id { get; set; }

    public int RestaurantId { get; set; }

    public string DayOfWeek { get; set; } = null!;

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? ClosingTime { get; set; }

    public virtual Restaurant Restaurant { get; set; } = null!;
}
