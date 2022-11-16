using System;
using System.Collections.Generic;

namespace ConnectionLibrary.DBModels;

public partial class UserRole
{
    public int Id { get; set; }

    public string UserRole1 { get; set; } = null!;

    public virtual ICollection<User> Users { get; } = new List<User>();
}
