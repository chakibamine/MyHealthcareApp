using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHealthcareApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyHealthcareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly MyHealthcareAppContext _context;

        public PrescriptionsController(MyHealthcareAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Prescription>> GetPrescriptions()
        {
            return _context.Prescriptions.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Prescription> GetPrescription(int id)
        {
            var prescription = _context.Prescriptions.Find(id);
            if (prescription == null)
            {
                return NotFound();
            }
            return prescription;
        }

        [HttpPost]
        public ActionResult<Prescription> CreatePrescription(Prescription prescription)
        {
            _context.Prescriptions.Add(prescription);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetPrescription), new { id = prescription.Id }, prescription);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePrescription(int id, Prescription prescription)
        {
            if (id != prescription.Id)
            {
                return BadRequest();
            }

            _context.Entry(prescription).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePrescription(int id)
        {
            var prescription = _context.Prescriptions.Find(id);
            if (prescription == null)
            {
                return NotFound();
            }

            _context.Prescriptions.Remove(prescription);
            _context.SaveChanges();
            return NoContent();
        }
    }
}