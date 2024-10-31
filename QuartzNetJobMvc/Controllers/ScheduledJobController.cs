using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;
using QuartzNetJobMvc.Databases.AppDbContextModels;

namespace QuartzNetJobMvc.Controllers
{
    public class ScheduledJobController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ISchedulerFactory _schedulerFactory;

        public ScheduledJobController(AppDbContext dbContext, ISchedulerFactory schedulerFactory)
        {
            _dbContext = dbContext;
            _schedulerFactory = schedulerFactory;
        }

        public async Task<IActionResult> Index()
        {
            var scheduleSettings = await _dbContext.ScheduleSettings.ToListAsync();
            return View(scheduleSettings);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ScheduleSetting model)
        {
            if (ModelState.IsValid)
            {
                // Check if a schedule with the same ServiceName already exists
                var existingSchedule = await _dbContext.ScheduleSettings
                    .AnyAsync(s => s.ServiceName == model.ServiceName);

                if (existingSchedule)
                {
                    TempData["ErrorMessage"] = "A job with the same Service Name already exists.";
                    return View(model);
                }

                // Add new schedule settings to the database
                _dbContext.ScheduleSettings.Add(model);
                await _dbContext.SaveChangesAsync();

                TempData["Message"] = "Job schedule created successfully.";

                // Optionally, you can add logic to schedule the job here if required

                return RedirectToAction("Index"); // Redirect to the index page
            }
            return View(model); // Return the same view with model if validation fails
        }

        public async Task<IActionResult> Edit(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                return NotFound();
            }

            var scheduleSetting = await _dbContext.ScheduleSettings
                .FirstOrDefaultAsync(s => s.ServiceName == serviceName);

            if (scheduleSetting == null)
            {
                return NotFound();
            }

            return View(scheduleSetting);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ScheduleSetting model)
        {
            if (ModelState.IsValid)
            {
                // Find the schedule settings by ServiceName
                var scheduleSettings = await _dbContext.ScheduleSettings
                    .FirstOrDefaultAsync(s => s.ServiceName == model.ServiceName);

                if (scheduleSettings != null)
                {
                    // Update the CronExpression
                    scheduleSettings.CronExpression = model.CronExpression;

                    // Scheduler logic
                    var scheduler = await _schedulerFactory.GetScheduler();

                    // Check if the trigger exists and delete if it does
                    var triggerKey = new TriggerKey("emailJobTrigger");
                    if (await scheduler.CheckExists(triggerKey))
                    {
                        await scheduler.UnscheduleJob(triggerKey);
                    }

                    // Determine the job type to schedule
                    IJobDetail jobDetail = JobBuilder.Create<EmailJob>()
                        .WithIdentity("emailJob")
                        .Build();

                    // Create a new trigger with the updated schedule
                    var newTrigger = TriggerBuilder.Create()
                        .WithIdentity(triggerKey)
                        .WithCronSchedule(scheduleSettings.CronExpression) // Use the new cron expression
                        .Build();

                    // Schedule the job with the new trigger
                    await scheduler.ScheduleJob(jobDetail, newTrigger);

                    // Save the updated schedule to the database
                    _dbContext.ScheduleSettings.Update(scheduleSettings);
                    await _dbContext.SaveChangesAsync();

                    TempData["Message"] = "Job schedule updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Schedule not found.";
                }
                return RedirectToAction("Index"); // Redirect to an appropriate view
            }
            return View(model); // Return the same view with model if validation fails
        }
    }
}
