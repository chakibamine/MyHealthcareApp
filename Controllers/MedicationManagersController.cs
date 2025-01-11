using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHealthcareApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyHealthcareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationManagersController : ControllerBase
    {
        private readonly MyHealthcareAppContext _context;

        public MedicationManagersController(MyHealthcareAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<MedicationManager>> GetMedicationManagers()
        {
            return _context.MedicationManagers.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<MedicationManager> GetMedicationManager(int id)
        {
            var manager = _context.MedicationManagers.Find(id);
            if (manager == null)
            {
                return NotFound();
            }
            return manager;
        }

        [HttpPost]
        public ActionResult<MedicationManager> CreateMedicationManager(MedicationManager manager)
        {
            _context.MedicationManagers.Add(manager);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetMedicationManager), new { id = manager.Id }, manager);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMedicationManager(int id, MedicationManager manager)
        {
            if (id != manager.Id)
            {
                return BadRequest();
            }

            _context.Entry(manager).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMedicationManager(int id)
        {
            var manager = _context.MedicationManagers.Find(id);
            if (manager == null)
            {
                return NotFound();
            }

            _context.MedicationManagers.Remove(manager);
            _context.SaveChanges();
            return NoContent();
        }
    }
}