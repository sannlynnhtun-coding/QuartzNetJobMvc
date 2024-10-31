# 🚀 Quartz.Net Job MVC

A **.NET 9 MVC** application that uses **Quartz.NET** for scheduling email jobs, **Serilog** for logging, and **FluentEmail** for email sending. This app allows administrators to create announcements and automatically send email notifications on a schedule if the email service is enabled.


### 📋 Features

- 📝 **Create Announcements**: Allows users to create announcements that can be sent as email notifications.
- ✉️ **Toggle Email Service**: Enables or disables the email sending functionality from the `ServiceSettings` table.
- ⏰ **Automated Email Scheduling**: Sends unsent announcements as emails every 5 minutes using Quartz.NET.
- 📊 **Logging with Serilog**: Logs events, including email sends and errors, to a file and SQL Server.


### 📂 Project Structure

- **Controllers**
  - `AnnouncementController`: Manages announcements, allowing creation and displaying feedback on success or failure.
  - `ServiceController`: Provides an interface to enable/disable the email service.

- **Extensions**
  - **Email** (`EmailServiceExtensions.cs`): Configures **FluentEmail** with SMTP settings to send emails.
  - **Logging** (`LoggingServiceExtensions.cs`): Configures **Serilog** to log to SQL Server and a file.
  - **Quartz** (`QuartzServiceExtensions.cs`, `EmailJob.cs`): Configures Quartz to schedule and run the `EmailJob`.

- 🔄 **EmailJob**: A Quartz job that checks for unsent announcements and sends them via email if the email service is enabled.


### 🛠️ Setup

1. **Database Configuration**:
   - Ensure your database connection string is correctly set in `appsettings.json`.
   - Use the following command to scaffold the database context and models:
     ```bash
     dotnet ef dbcontext scaffold "Server=.;Database=QuartzNetJobMvc;User Id=sa;Password=sasa@123;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o AppDbContextModels -c AppDbContext -f
     ```

2. **Email Settings**:
   - Configure SMTP email settings in `appsettings.json` under the `EmailSettings` section.

3. **Run the Application**:
   - Execute `dotnet run` to start the app.
   - Access the app at `https://localhost:7140` (or configured URL).


### 🚀 Usage

1. **Create an Announcement**:
   - Go to the Announcements page and fill in the details.
   - The announcement will be queued to send when the next job runs.

2. **Enable/Disable Email Service**:
   - Use the Service page to toggle email notifications.
   - If enabled, unsent announcements will be emailed automatically.

3. **Quartz Job**:
   - The job is set to check for new announcements and send them every 5 minutes.


### 📜 Logging

All log entries are saved to:
   - **SQL Server**: Logs to the `Logs` table (created automatically).
   - **File Logs**: Stores logs daily in `logs/log-.txt` in the root folder.


### 🛠 Configuration

In `Program.cs`, configuration extensions are added for:
- **Quartz** (`ConfigureQuartz`): Registers and configures Quartz scheduler with `EmailJob`.
- **FluentEmail** (`ConfigureFluentEmail`): Configures FluentEmail with SMTP settings.
- **Serilog** (`ConfigureSerilog`): Configures logging to file and SQL Server.

