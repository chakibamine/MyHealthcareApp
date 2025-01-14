using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHealthcareApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHealthcareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly MyHealthcareAppContext _context;

        public AppointmentsController(MyHealthcareAppContext context)
        {
            _context = context;
        }

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Select(a => new
                {
                    a.Id,
                    a.Date,
                    a.Heure,
                    a.Statut,
                    a.Notes,
                    Doctor = new
                    {
                        a.Doctor.Id,
                        a.Doctor.Nom
                    },
                    Patient = new
                    {
                        a.Patient.Id,
                        a.Patient.Nom
                    }
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure Doctor and Patient exist
            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == appointment.DoctorId);
            var patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);

            if (!doctorExists || !patientExists)
            {
                return BadRequest("Invalid DoctorId or PatientId.");
            }

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        // PUT: api/Appointments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure Doctor and Patient exist
            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == appointment.DoctorId);
            var patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);

            if (!doctorExists || !patientExists)
            {
                return BadRequest("Invalid DoctorId or PatientId.");
            }

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);

        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}