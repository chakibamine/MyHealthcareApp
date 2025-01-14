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
                .Include(m => m.Appointment)
                .Select(m => new
                {
                    m.Id,
                    m.PatientId,
                    m.DoctorId,
                    m.AppointmentId,
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
                    },
                    Appointment = m.Appointment == null ? null : new
                    {
                        m.Appointment.Id,
                        m.Appointment.Date,
                        Status = m.Appointment.Statut
                    }
                })
                .ToListAsync();

            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetMedicalRecord(int id)
        {
            var medicalRecord = await _context.MedicalRecords
                .Include(m => m.Doctor)
                .Include(m => m.Patient)
                .Include(m => m.Prescription)
                .Include(m => m.Appointment)
                .Select(m => new
                {
                    m.Id,
                    m.PatientId,
                    m.DoctorId,
                    m.AppointmentId,
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
                    },
                    Appointment = m.Appointment == null ? null : new
                    {
                        m.Appointment.Id,
                        m.Appointment.Date,
                        Status = m.Appointment.Statut
                    }
                })
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicalRecord == null)
            {
                return NotFound();
            }

            return Ok(medicalRecord);
        }

        [HttpPost]
        public async Task<ActionResult<object>> PostMedicalRecord(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();

            var result = await _context.MedicalRecords
                .Include(m => m.Doctor)
                .Include(m => m.Patient)
                .Include(m => m.Prescription)
                .Include(m => m.Appointment)
                .Select(m => new
                {
                    m.Id,
                    m.PatientId,
                    m.DoctorId,
                    m.AppointmentId,
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
                    },
                    Appointment = m.Appointment == null ? null : new
                    {
                        m.Appointment.Id,
                        m.Appointment.Date,
                        Status = m.Appointment.Statut
                    }
                })
                .FirstOrDefaultAsync(m => m.Id == medicalRecord.Id);

            return CreatedAtAction(nameof(GetMedicalRecord), new { id = medicalRecord.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> PutMedicalRecord(int id, MedicalRecord medicalRecord)
        {
            if (id != medicalRecord.Id)
            {
                return BadRequest();
            }

            _context.Entry(medicalRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                var result = await _context.MedicalRecords
                    .Include(m => m.Doctor)
                    .Include(m => m.Patient)
                    .Include(m => m.Prescription)
                    .Include(m => m.Appointment)
                    .Select(m => new
                    {
                        m.Id,
                        m.PatientId,
                        m.DoctorId,
                        m.AppointmentId,
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
                        },
                        Appointment = m.Appointment == null ? null : new
                        {
                            m.Appointment.Id,
                            m.Appointment.Date,
                            Status = m.Appointment.Statut
                        }
                    })
                    .FirstOrDefaultAsync(m => m.Id == id);

                return Ok(result);
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