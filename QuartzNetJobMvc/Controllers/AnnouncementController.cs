using Microsoft.AspNetCore.Mvc;
using QuartzNetJobMvc.Extensions;
using QuartzNetJobMvc.Extensions.Quartz;

namespace QuartzNetJobMvc.Controllers
{
    // Controllers/AnnouncementController.cs
    public class AnnouncementController : Controller
    {
        private readonly AppDbContext _dbContext;

        public AnnouncementController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Announcements.Add(announcement);
                await _dbContext.SaveChangesAsync();
                // Trigger the email job manually or it will be picked up by the job’s schedule
                return RedirectToAction("Index", "Home");
            }
            return View(announcement);
        }
    }
}
