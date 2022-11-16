using System;
using System.Collections.Generic;

namespace ConnectionLibrary.DBModels;

public partial class PhoneNumber
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string PhoneNumber1 { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
