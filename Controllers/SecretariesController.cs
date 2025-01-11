using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHealthcareApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHealthcareApp.Services;

namespace MyHealthcareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretariesController : ControllerBase
    {
        private readonly MyHealthcareAppContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public SecretariesController(MyHealthcareAppContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Secretary>> GetSecretaries()
        {
            return _context.Secretaries.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Secretary> GetSecretary(int id)
        {
            var secretary = _context.Secretaries.Find(id);
            if (secretary == null)
            {
                return NotFound();
            }
            return secretary;
        }

        [HttpPost]
        public async Task<ActionResult<Secretary>> PostSecretary(Secretary secretary)
        {
            // Hash the password before saving
            secretary.Password = _passwordHasher.HashPassword(secretary.Password);
            
            _context.Secretaries.Add(secretary);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSecretary), new { id = secretary.Id }, secretary);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSecretary(int id, Secretary secretary)
        {
            if (id != secretary.Id)
            {
                return BadRequest();
            }

            // If password is being updated, hash it
            if (!string.IsNullOrEmpty(secretary.Password))
            {
                secretary.Password = _passwordHasher.HashPassword(secretary.Password);
            }

            _context.Entry(secretary).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSecretary(int id)
        {
            var secretary = _context.Secretaries.Find(id);
            if (secretary == null)
            {
                return NotFound();
            }

            _context.Secretaries.Remove(secretary);
            _context.SaveChanges();
            return NoContent();
        }
    }
}