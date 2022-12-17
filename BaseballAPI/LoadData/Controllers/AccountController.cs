using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BaseballAPI.Dtos;
using BaseballModel.Models;

namespace BaseballAPI.LoadData.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase {
    private readonly UserManager<BaseballUser> _userManager;
    private readonly JwtHandler _jwtHandler;

    public AccountController(UserManager<BaseballUser> userManager, JwtHandler jwtHandler) {
        _userManager = userManager;
        _jwtHandler = jwtHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        BaseballUser? user = await _userManager.FindByNameAsync(loginRequest.UserName);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password)) {
            return Unauthorized(new LoginResponse {
                Success = false,
                Message = "Invalid Username or Password."
            });
        }

        JwtSecurityToken secToken = await _jwtHandler.GetTokenAsync(user);
        string? jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
        return Ok(new LoginResponse {
            Success = true,
            Message = "Login successful",
            Token = jwt
        });
    }
}
