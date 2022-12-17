using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BaseballModel.Models;

public partial class Team
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string TeamName { get; set; } = null!;

    public int TeamRank { get; set; }

    [InverseProperty("Team")]
    public virtual ICollection<Player> Players { get; } = new List<Player>();
}
