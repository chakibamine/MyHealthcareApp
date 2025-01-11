using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHealthcareApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyHealthcareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationStocksController : ControllerBase
    {
        private readonly MyHealthcareAppContext _context;

        public MedicationStocksController(MyHealthcareAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<MedicationStock>> GetMedicationStocks()
        {
            return _context.MedicationStocks.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<MedicationStock> GetMedicationStock(int id)
        {
            var stock = _context.MedicationStocks.Find(id);
            if (stock == null)
            {
                return NotFound();
            }
            return stock;
        }

        [HttpPost]
        public ActionResult<MedicationStock> CreateMedicationStock(MedicationStock stock)
        {
            _context.MedicationStocks.Add(stock);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetMedicationStock), new { id = stock.Id }, stock);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMedicationStock(int id, MedicationStock stock)
        {
            if (id != stock.Id)
            {
                return BadRequest();
            }

            _context.Entry(stock).State = EntityState.Modified;
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetMedicationStock), new { id = stock.Id }, stock);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMedicationStock(int id)
        {
            var stock = _context.MedicationStocks.Find(id);
            if (stock == null)
            {
                return NotFound();
            }

            _context.MedicationStocks.Remove(stock);
            _context.SaveChanges();
            return NoContent();
        }
    }
}