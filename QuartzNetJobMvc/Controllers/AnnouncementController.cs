namespace QuartzNetJobMvc.Controllers;

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
        try
        {
            if (ModelState.IsValid)
            {
                _dbContext.Announcements.Add(announcement);
                var result = await _dbContext.SaveChangesAsync();

                TempData["Message"] = "Announcement created successfully.";

                // Trigger the email job manually or it will be picked up by the job’s schedule
                return RedirectToAction("Create", "Announcement");
            }
            announcement = new Announcement();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"]  = "An error occurred while creating the announcement.";
        }
        return View(announcement);
    }
}
