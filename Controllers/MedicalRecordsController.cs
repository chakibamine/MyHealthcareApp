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
    public class MedicalRecordsController : ControllerBase
    {
        private readonly MyHealthcareAppContext _context;

        public MedicalRecordsController(MyHealthcareAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetMedicalRecords()
        {
            var records = await _context.MedicalRecords
                .Include(m => m.Doctor)
                .Include(m => m.Patient)
                .Include(m => m.Prescription)
                .Select(m => new
                {
                    m.Id,
                    m.PatientId,
                    m.DoctorId,
                    m.Date,
                    m.Diagnosis,
                    Doctor = new { 
                        m.Doctor.Id, 
                        m.Doctor.Nom,
                        m.Doctor.Specialite 
                    },
                    Patient = new { 
                        m.Patient.Id, 
                        m.Patient.Nom,
                        m.Patient.Email 
                    },
                    Prescription = m.Prescription == null ? null : new
                    {
                        m.Prescription.Id,
                        m.Prescription.Description,
                        m.Prescription.Medicaments,
                        m.Prescription.Posologie
                    }
                })
                .ToListAsync();

            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecord>> GetMedicalRecord(int id)
        {
            var medicalRecord = await _context.MedicalRecords
                .Include(m => m.Doctor)
                .Include(m => m.Patient)
                .Include(m => m.Prescription)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicalRecord == null)
            {
                return NotFound();
            }

            return medicalRecord;
        }

        [HttpPost]
        public async Task<ActionResult<MedicalRecord>> PostMedicalRecord(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedicalRecord), new { id = medicalRecord.Id }, medicalRecord);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicalRecord(int id, MedicalRecord medicalRecord)
        {
            if (id != medicalRecord.Id)
            {
                return BadRequest();
            }

            _context.Entry(medicalRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicalRecordExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord == null)
            {
                return NotFound();
            }

            _context.MedicalRecords.Remove(medicalRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicalRecordExists(int id)
        {
            return _context.MedicalRecords.Any(e => e.Id == id);
        }
    }
}