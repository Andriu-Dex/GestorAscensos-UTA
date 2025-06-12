using System;
using System.Collections.Generic;

namespace SGA.Infrastructure.TempModels;

public partial class UserType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
