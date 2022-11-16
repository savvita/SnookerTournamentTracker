using System;
using System.Collections.Generic;

namespace ConnectionLibrary.DBModels;

public partial class Place
{
    public int Id { get; set; }

    public string Place1 { get; set; } = null!;

    public virtual ICollection<Prize> Prizes { get; } = new List<Prize>();
}
