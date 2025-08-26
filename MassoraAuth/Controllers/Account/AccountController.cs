using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityServer.Models;
using IdentityServer.Services; // ÖNEMLİ: Eklendi
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// Route attribute'u kaldırıldı, bu artık bir MVC controller
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction; // Eklendi
    private readonly UserManager<ApplicationUser> _userManager; // Register için lazım olmasa da logout vs. için kalabilir.
    private readonly IEventService _events;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interaction,IEventService events, ILogger<AccountController> logger) // Constructor güncellendi
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _interaction = interaction;
        _events = events;
        _logger = logger;
    }

    [HttpGet] // Route attribute'u olmadığı için default olarak "Login" action ismini alır
    public IActionResult Login(string returnUrl)
    {
        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // ReturnUrl'in geçerli bir OIDC akışına ait olup olmadığını kontrol et
        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
        if (context == null && !Url.IsLocalUrl(model.ReturnUrl))
        {
            // Eğer returnUrl geçersizse, güvenlik açığı oluşturmamak için hata ver
            ModelState.AddModelError("", "Geçersiz dönüş adresi.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // Başarılı girişten sonra güvenli bir şekilde yönlendir
            return Redirect(model.ReturnUrl);
        }

        ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre.");
        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        var vm = await BuildLogoutViewModelAsync(logoutId);

        if (vm.ShowLogoutPrompt == false)
        {
            // Eğer onay gerekmiyorsa, doğrudan çıkış yap ve yönlendir.
            return await Logout(vm);
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(LogoutViewModel model)
    {
        // Kullanıcının oturumunu sonlandır (cookie'yi sil).
        await _signInManager.SignOutAsync();

        // Çıkış sonrası yönlendirilecek adresi al.
        var context = await _interaction.GetLogoutContextAsync(model.LogoutId);

        
        // Eğer yönlendirilecek bir adres varsa, oraya yönlendir.
        if (!string.IsNullOrEmpty(context?.PostLogoutRedirectUri))
        {
            return Redirect(context.PostLogoutRedirectUri);
        }

        // Yönlendirilecek adres yoksa, genel bir "Çıkış Yapıldı" sayfası göster.
        return RedirectToAction("Login", "Account");
    }

    // LogoutViewModel'i oluşturan yardımcı metod.
    private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
    {
        var context = await _interaction.GetLogoutContextAsync(logoutId);
        return new LogoutViewModel
        {
            LogoutId = logoutId,
            ShowLogoutPrompt = false // Varsayılan olarak onayı göster
        };
    }
    [HttpPost("api/account/register")] // API endpoint'i için net bir route belirleyelim
    [AllowAnonymous]
    public async Task<IActionResult> RegisterApi([FromBody] RegisterInputModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Başarılı olursa, oluşturulan kullanıcının ID'sini ve Email'ini geri dön.
            // MassoraAPI bu cevabı alıp kullanacak.
            return Ok(new { Id = user.Id, Email = user.Email });
        }

        // Başarısız olursa, Identity'den gelen hataları BadRequest olarak dön.
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return BadRequest(ModelState);
    }

}