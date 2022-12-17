using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BaseballModel.Models;

[Table("Player")]
public partial class Player
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    public int Age { get; set; }

    public int GamesPlayed { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string TeamName { get; set; } = null!;

    public int TeamId { get; set; }

    [ForeignKey("TeamId")]
    [InverseProperty("Players")]
    public virtual Team Team { get; set; } = null!;
}
