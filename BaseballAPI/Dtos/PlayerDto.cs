using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BaseballAPI.Dtos;

public class PlayerDto
{
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
   

}