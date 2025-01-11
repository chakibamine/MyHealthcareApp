using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHealthcareApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyHealthcareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingsController : ControllerBase
    {
        private readonly MyHealthcareAppContext _context;

        public BillingsController(MyHealthcareAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Billing>> GetBillings()
        {
            return _context.Billings.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Billing> GetBilling(int id)
        {
            var billing = _context.Billings.Find(id);
            if (billing == null)
            {
                return NotFound();
            }
            return billing;
        }

        [HttpPost]
        public ActionResult<Billing> CreateBilling(Billing billing)
        {
            _context.Billings.Add(billing);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetBilling), new { id = billing.Id }, billing);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBilling(int id, Billing billing)
        {
            if (id != billing.Id)
            {
                return BadRequest();
            }

            _context.Entry(billing).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBilling(int id)
        {
            var billing = _context.Billings.Find(id);
            if (billing == null)
            {
                return NotFound();
            }

            _context.Billings.Remove(billing);
            _context.SaveChanges();
            return NoContent();
        }
    }
}