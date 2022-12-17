namespace BaseballAPI.Data;
// ["city","city_ascii","lat","lng","country","iso2","iso3","admin_name","capital","population","id"]
public class BaseballCsv
{
    public string TeamName { get; set; } = null!;
    public int TeamRank { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public int GamesPlayed { get; set; }
    public int RunScored { get; set; } 
    public long id { get; set; }

}