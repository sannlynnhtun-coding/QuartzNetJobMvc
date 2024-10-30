using Microsoft.AspNetCore.Mvc;

namespace QuartzNetJobMvc.Controllers
{
    // Controllers/AnnouncementController.cs
    public class AnnouncementController : Controller
    {
        private readonly YourDbContext _dbContext;

        public AnnouncementController(YourDbContext dbContext)
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
