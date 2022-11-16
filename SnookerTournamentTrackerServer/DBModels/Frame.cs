using System;
using System.Collections.Generic;

namespace ConnectionLibrary.DBModels;

public partial class Frame
{
    public int Id { get; set; }

    public int MatchId { get; set; }

    public int? WinnerId { get; set; }

    public virtual ICollection<Brake> Brakes { get; } = new List<Brake>();

    public virtual ICollection<FrameEntity> FrameEntities { get; } = new List<FrameEntity>();

    public virtual Match Match { get; set; } = null!;

    public virtual User? Winner { get; set; }
}
