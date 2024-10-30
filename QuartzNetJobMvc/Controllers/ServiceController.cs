using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuartzNetJobMvc.Extensions;
using QuartzNetJobMvc.Extensions.Quartz;

namespace QuartzNetJobMvc.Controllers
{
    // Controllers/ServiceController.cs
    public class ServiceController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ServiceController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var emailServiceSetting = await _dbContext.ServiceSettings
                .FirstOrDefaultAsync(s => s.ServiceName == "EmailService");

            return View(emailServiceSetting);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleEmailService()
        {
            var emailServiceSetting = await _dbContext.ServiceSettings
                .FirstOrDefaultAsync(s => s.ServiceName == "EmailService");

            if (emailServiceSetting != null)
            {
                emailServiceSetting.IsEnabled = !emailServiceSetting.IsEnabled;
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
