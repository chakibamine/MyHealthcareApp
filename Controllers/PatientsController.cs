using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHealthcareApp.Models;
using MyHealthcareApp.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyHealthcareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly MyHealthcareAppContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public PatientsController(MyHealthcareAppContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: api/Patients
        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetPatients()
        {
            return _context.Patients.ToList();
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public ActionResult<Patient> GetPatient(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }
            return patient;
        }

        // POST: api/Patients
        [HttpPost]
        public async Task<ActionResult<Patient>> CreatePatient(Patient patient)
        {
            // Initialize related collections
            patient.MedicalRecords = new List<MedicalRecord>();
            patient.Appointments = new List<Appointment>();
            patient.Bills = new List<Billing>();

            // Hash the password before saving
            patient.Password = _passwordHasher.HashPassword(patient.Password);
            
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        // PUT: api/Patients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, Patient patient)
        {
            if (id != patient.Id)
            {
                return BadRequest();
            }

            // If password is being updated, hash it
            if (!string.IsNullOrEmpty(patient.Password))
            {
                patient.Password = _passwordHasher.HashPassword(patient.Password);
            }

            _context.Entry(patient).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}