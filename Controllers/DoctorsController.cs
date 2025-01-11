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
    public class DoctorsController : ControllerBase
    {
        private readonly MyHealthcareAppContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public DoctorsController(MyHealthcareAppContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Doctor>> GetDoctors()
        {
            return _context.Doctors.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Doctor> GetDoctor(int id)
        {
            var doctor = _context.Doctors.Find(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return doctor;
        }

        [HttpPost]
    public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
    {
        
        // Set default password as email if not provided
        if (doctor.Password == null || doctor.Password == string.Empty)
        {
            Console.WriteLine("Password is null or empty");
            doctor.Password = doctor.Email;
        }

        // Hash the password before saving
        doctor.Password = _passwordHasher.HashPassword(doctor.Password);
        
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
    }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return BadRequest();
            }

            // Only hash password if one was provided
            if (!string.IsNullOrEmpty(doctor.Password))
            {
                doctor.Password = _passwordHasher.HashPassword(doctor.Password);
            }
            else
            {
                // Don't modify password if none provided
                _context.Entry(doctor).Property(x => x.Password).IsModified = false;
            }

            _context.Entry(doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}