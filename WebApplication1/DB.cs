using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class AppointmentSystemDbContext : IdentityDbContext<PatientModel>
    {
        public AppointmentSystemDbContext(DbContextOptions<AppointmentSystemDbContext> options)
            : base(options)
        {
        }
        public DbSet<PatientModel> Registration {  get; set; }
        public DbSet<AppointmentModel> Appointment{ get; set; }

    }
}
