using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using BaseballAPI.LoadData.Controllers;
using BaseballModel.Models;
using Path = System.IO.Path;
using BaseballAPI.Data;

namespace BaseballAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SeedController : ControllerBase
{
    private readonly BaseballStatsContext _context;
    private readonly string _pathName;

    private readonly UserManager<BaseballUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    public SeedController(BaseballStatsContext context, IHostEnvironment environment, 
        UserManager<BaseballUser> userManager, RoleManager<IdentityRole> roleManager, 
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _pathName = Path.Combine(environment.ContentRootPath, "Data/Baseball.csv");
    }

    [HttpGet("Players")]
    public async Task<IActionResult> ImportCities()
    {
        Dictionary<string, Team> countries = await _context.Teams.AsNoTracking()
            .ToDictionaryAsync(c => c.TeamName);

        CsvConfiguration config = new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null
        };
        int playerCount = 0;
        using (StreamReader reader = new(_pathName))
        using (CsvReader csv = new(reader, config))
        {
            IEnumerable<BaseballCsv>? records = csv.GetRecords<BaseballCsv>();
            foreach (BaseballCsv record in records)
            {
                if (!countries.ContainsKey(record.TeamName))
                {
                    Console.WriteLine($"Not found country for {record.Name}");
                    return NotFound(record);
                }

                
                Player player = new()
                {
                    Name = record.Name,
                   // NameAscii = record.player_ascii,
                    Age = record.Age,
                    TeamName = record.TeamName,
                    GamesPlayed = (int) record.GamesPlayed,
                    //TeamId = Team[record.team].Id
                };
                _context.Players.Add(player);
                playerCount++;
            }
            await _context.SaveChangesAsync();
        }
        return new JsonResult(playerCount);
    }

    [HttpGet("Teams")]
    public async Task<IActionResult> ImportTeams()
    {
        // create a lookup dictionary containing all the countries already existing 
        // into the Database (it will be empty on first run).
        Dictionary<string, Team> TeamsByName = _context.Teams
            .AsNoTracking().ToDictionary(x => x.TeamName, StringComparer.OrdinalIgnoreCase);

        CsvConfiguration config = new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null
        };
        using (StreamReader reader = new(_pathName))
        using (CsvReader csv = new(reader, config))
        {
            IEnumerable<BaseballCsv>? records = csv.GetRecords<BaseballCsv>();
            foreach (BaseballCsv record in records)
            {
                if (TeamsByName.ContainsKey(record.TeamName))
                {
                    continue;
                }

                Team team = new()
                {
                    TeamName = record.TeamName,
                    TeamRank = record.TeamRank,
                    
                };
                await _context.Teams.AddAsync(team);
                TeamsByName.Add(record.TeamName, team);
            }

            await _context.SaveChangesAsync();
        }
        return new JsonResult(TeamsByName.Count);
    }

    [HttpGet("Users")]
    public async Task<IActionResult> CreateUsers()
    {
        const string roleUser = "RegisteredUser";
        const string roleAdmin = "Administrator";

        if (await _roleManager.FindByNameAsync(roleUser) is null)
        {
            await _roleManager.CreateAsync(new IdentityRole(roleUser));
        }
        if (await _roleManager.FindByNameAsync(roleAdmin) is null)
        {
            await _roleManager.CreateAsync(new IdentityRole(roleAdmin));
        }

        List<BaseballUser> addedUserList = new();
        (string name, string email) = ("admin", "admin@email.com");

        if (await _userManager.FindByNameAsync(name) is null)
        {
            BaseballUser userAdmin = new()
            {
                UserName = name,
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            await _userManager.CreateAsync(userAdmin, _configuration["DefaultPasswords:Administrator"]!);
            await _userManager.AddToRolesAsync(userAdmin, new[] { roleUser, roleAdmin });
            userAdmin.EmailConfirmed = true;
            userAdmin.LockoutEnabled = false;
            addedUserList.Add(userAdmin);
        }

        (string name, string email) registered = ("user", "user@email.com");

        if (await _userManager.FindByNameAsync(registered.name) is null)
        {
            BaseballUser user = new()
            {
                UserName = registered.name,
                Email = registered.email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            await _userManager.CreateAsync(user, _configuration["DefaultPasswords:RegisteredUser"]!);
            await _userManager.AddToRoleAsync(user, roleUser);
            user.EmailConfirmed = true;
            user.LockoutEnabled = false;
            addedUserList.Add(user);
        }

        if (addedUserList.Count > 0)
        {
            await _context.SaveChangesAsync();
        }

        return new JsonResult(new
        {
            addedUserList.Count,
            Users = addedUserList
        });

    }
}
