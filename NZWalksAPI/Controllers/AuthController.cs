using System.Net;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.Dto.Request.Auth;
using NZWalksAPI.Models.Dto.Response;
using NZWalksAPI.Services.Base;
using ServiceLayer;

namespace NZWalksAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMapper mapper, UserManager<User> userManager,
            IJwtService jwtService, ILogger<AuthController> logger)
        {
            _mapper = mapper;
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }

            var user = new User
            {
                Email = registerRequest.Email,
                UserName = registerRequest.Username
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("Đăng ký không thành công: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest("Đăng ký không thành công");
            }

            result = await _userManager.AddToRolesAsync(user, registerRequest.Role);
            if (!result.Succeeded)
            {
                _logger.LogError("Không thể gán vai trò cho người dùng: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest("Không thể gán vai trò cho người dùng");
            }

            return Ok(new { Message = "Đăng ký thành công" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            var user = await _userManager.FindByNameAsync(authenticationRequest.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, authenticationRequest.Password))
            {
                _logger.LogWarning("Đăng nhập thất bại cho người dùng: {Username}", authenticationRequest.Username);
                return Unauthorized("Tên đăng nhập hoặc mật khẩu không đúng");
            }

            return Ok(new AuthenticationResponse
            {
                AccessToken = _jwtService.CreateAccessToken(user),
                RefreshToken = _jwtService.CreateRefreshToken(user)
            });
        }
        
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var userName = _jwtService.ValidateRefreshToken(refreshToken);
            if (userName == null)
            {
                return BadRequest("Token không hợp lệ");
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Unauthorized("Không tìm thấy người dùng");
            }

            return Ok(new { AccessToken = _jwtService.CreateAccessToken(user) });
        }
        
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback))
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // [HttpGet("google-callback")]
        // public async Task<IActionResult> GoogleCallback()
        // {
        //     var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //
        //     if (!authenticateResult.Succeeded)
        //     {
        //         return Unauthorized("Xác thực Google thất bại.");
        //     }
        //
        //     var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
        //     var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        //     var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        //
        //     if (string.IsNullOrEmpty(email))
        //     {
        //         return BadRequest("Không thể lấy thông tin email từ Google.");
        //     }
        //
        //     var user = await _userManager.FindByEmailAsync(email);
        //     if (user == null)
        //     {
        //         user = new User
        //         {
        //             Email = email,
        //             UserName = email,
        //         };
        //
        //         var createResult = await _userManager.CreateAsync(user);
        //         if (!createResult.Succeeded)
        //         {
        //             return BadRequest("Không thể tạo tài khoản từ Google.");
        //         }
        //
        //         await _userManager.AddToRoleAsync(user, "User"); // Gán role mặc định
        //     }
        //
        //     // Tạo JWT token cho người dùng
        //     var accessToken = _jwtService.CreateAccessToken(user);
        //     var refreshToken = _jwtService.CreateRefreshToken(user);
        //
        //     return Ok(new AuthenticationResponse
        //     {
        //         AccessToken = accessToken,
        //         RefreshToken = refreshToken
        //     });
        // }
        
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return Unauthorized("Xác thực Google thất bại.");
            }

            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            
            foreach (var claim in claims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }

            
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var givenName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var familyName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
            var profilePicture = claims?.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;
            var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Không thể lấy thông tin email từ Google.");
            }

            return Ok(new
            {
                GoogleId = googleId,
                Email = email,
                FullName = name,
                FirstName = givenName,
                LastName = familyName,
                ProfilePicture = profilePicture
            });
        }
        
        [HttpGet("login-facebook")]
        public IActionResult LoginWithFacebook()
        {
            var redirectUrl = Url.Action(nameof(FacebookResponse), "Auth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        // Xử lý phản hồi từ Facebook
        [HttpGet("facebook-callback")]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded || result.Principal == null)
            {
                return BadRequest("Facebook authentication failed");
            }

            var claimsIdentity = result.Principal.Identities.FirstOrDefault();
            if (claimsIdentity == null)
            {
                return BadRequest("No identity found");
            }

            var claims = claimsIdentity.Claims.ToDictionary(c => c.Type, c => c.Value);

            var name = claims.GetValueOrDefault(ClaimTypes.Name);
            var email = claims.GetValueOrDefault(ClaimTypes.Email);
            var firstName = claims.GetValueOrDefault(ClaimTypes.GivenName);
            var lastName = claims.GetValueOrDefault(ClaimTypes.Surname);
            var birthDate = claims.GetValueOrDefault(ClaimTypes.DateOfBirth);
            var gender = claims.GetValueOrDefault(ClaimTypes.Gender);
            var location = claims.GetValueOrDefault("urn:facebook:location");
            var hometown = claims.GetValueOrDefault("urn:facebook:hometown");

            // Giải mã JSON để lấy URL ảnh đại diện đúng
            string profilePictureUrl = null;
            var profilePictureJson = claims.GetValueOrDefault("urn:facebook:picture");
            if (!string.IsNullOrEmpty(profilePictureJson))
            {
                try
                {
                    var jsonDoc = System.Text.Json.JsonDocument.Parse(profilePictureJson);
                    profilePictureUrl = jsonDoc.RootElement.GetProperty("url").GetString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing Facebook profile picture: {ex.Message}");
                }
            }

            return Ok(new
            {
                Name = name,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                BirthDate = birthDate,
                Gender = gender,
                Location = location,
                Hometown = hometown,
                ProfilePicture = profilePictureUrl
            });
        }


    }
}