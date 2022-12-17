using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaseballModel.Models;


namespace BaseballAPI.LoadData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly BaseballStatsContext _context;

        public PlayerController(BaseballStatsContext context)
        {
            _context = context;
        }

        // GET: api/Players

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers() => await _context.Players.Take(100).ToListAsync();

        // GET: api/Players/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Player>> Getplayer(int id)
        {
            Player? player = await _context.Players.FindAsync(id);

            return player == null ? NotFound() : player;
        }

        // PUT: api/Players/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutPlayer(int id, Player player)
        {
            if (id != player.Id)
            {
                return BadRequest();
            }

            _context.Entry(player).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Playerexists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // POST: api/Players
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Player>> Postplayer(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getplayer", new { id = player.Id }, player);
        }

        // DELETE: api/Players/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteplayer(int id)
        {
            Player? player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Playerexists(int id) => _context.Players.Any(e => e.Id == id);
    }
}

