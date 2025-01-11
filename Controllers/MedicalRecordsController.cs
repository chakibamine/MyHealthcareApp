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
        public ActionResult<IEnumerable<MedicalRecord>> GetMedicalRecords()
        {
            return _context.MedicalRecords.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<MedicalRecord> GetMedicalRecord(int id)
        {
            var record = _context.MedicalRecords.Find(id);
            if (record == null)
            {
                return NotFound();
            }
            return record;
        }

        [HttpPost]
        public async Task<ActionResult<MedicalRecord>> PostMedicalRecord(MedicalRecord record)
        {
            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMedicalRecord), new { id = record.Id }, record);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMedicalRecord(int id, MedicalRecord record)
        {
            if (id != record.Id)
            {
                return BadRequest();
            }

            _context.Entry(record).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMedicalRecord(int id)
        {
            var record = _context.MedicalRecords.Find(id);
            if (record == null)
            {
                return NotFound();
            }

            _context.MedicalRecords.Remove(record);
            _context.SaveChanges();
            return NoContent();
        }
    }
}