//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using BaseballAPI.Dtos;
using BaseballModel.Models;

namespace BaseballAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeamsController : ControllerBase
{
    private readonly BaseballStatsContext _context;

    public TeamsController(BaseballStatsContext context)
    {
        _context = context;
    }

    // GET: api/Teams
   // [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Team>>> GetTeams() => await _context.Teams.OrderBy(c => c.TeamName).ToListAsync();

    // GET: api/Teams/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetTeam(int id)
    {
        var TeamDTO = await _context.Teams
            .Select(c => new
            {
                c.Id,
                c.TeamName,
                c.TeamRank
                
            })
            .SingleOrDefaultAsync(c => c.Id == id);


        return TeamDTO == null ? NotFound() : Ok(TeamDTO);
    }

    [HttpGet("TeamPlayers/{id:int}")]
    public async Task<ActionResult> GetTeamPlayers(int id)
    {
        var TeamDTO = await _context.Teams
            .Where(c => c.Id == id)
            .Select(c => new 
            {
                 c.Id,
                 c.TeamName,
                 c.Players
            }).SingleOrDefaultAsync();

        return TeamDTO == null ? NotFound() : Ok(TeamDTO);
    }
   
    [HttpGet("Players")]
    public async Task<ActionResult<PlayerDto>> GetPlayers()
    {
        List<PlayerDto> playerDto = await _context.Players
           
            .Select(c => new PlayerDto
            {
                Id = c.Id,
                Name = c.Name,
                TeamId = c.TeamId,
                TeamName = c.Team.TeamName
            }).ToListAsync();

        return Ok(playerDto);
    }

    // PUT: api/Teams/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutTeam(int id, Team Team)
    {
        if (id != Team.Id)
        {
            return BadRequest();
        }

        _context.Entry(Team).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TeamExists(id))
            {
                return NotFound();
            }

            throw;
        }
        return NoContent();
    }

    // POST: api/Teams
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Team>> PostTeam(Team Team)
    {
        _context.Teams.Add(Team);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTeam", new { id = Team.Id }, Team);
    }

    // DELETE: api/Teams/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTeam(int id)
    {
        Team? Team = await _context.Teams.FindAsync(id);
        if (Team == null)
        {
            return NotFound();
        }

        _context.Teams.Remove(Team);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TeamExists(int id) => _context.Teams.Any(e => e.Id == id);
}