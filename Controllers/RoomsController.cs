using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHealthcareApp.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors;

namespace MyHealthcareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("NextJsPolicy")]
    public class RoomsController : ControllerBase
    {
        private readonly MyHealthcareAppContext _context;

        public RoomsController(MyHealthcareAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return await _context.Rooms.Include(r => r.Doctor).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Doctor)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }

        [HttpPost]
        public async Task<ActionResult<Room>> CreateRoom(Room room)
        {
            room.Doctor = null;

            if (room.DoctorId.HasValue)
            {
                var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == room.DoctorId);
                if (!doctorExists)
                {
                    return BadRequest("Specified Doctor does not exist");
                }
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var createdRoom = await _context.Rooms
                .Include(r => r.Doctor)
                .FirstOrDefaultAsync(r => r.Id == room.Id);

            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, createdRoom);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, Room room)
        {
            if (id != room.Id)
            {
                return BadRequest();
            }

            room.Doctor = null;

            if (room.DoctorId.HasValue)
            {
                var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == room.DoctorId);
                if (!doctorExists)
                {
                    return BadRequest("Specified Doctor does not exist");
                }
            }

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            var updatedRoom = await _context.Rooms
                .Include(r => r.Doctor)
                .FirstOrDefaultAsync(r => r.Id == id);

            return Ok(updatedRoom);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}