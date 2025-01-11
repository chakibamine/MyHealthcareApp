using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyHealthcareApp.Hubs;
using MyHealthcareApp.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyHealthcareApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly MyHealthcareAppContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(MyHealthcareAppContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
public class CreateChatRequest
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
}

[HttpPost("create")]
public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
{
    Console.WriteLine($"DoctorId: {request.DoctorId}, PatientId: {request.PatientId}");

    // Validate DoctorId
    var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == request.DoctorId);
    if (!doctorExists)
    {
        return BadRequest(new { error = $"Doctor with ID {request.DoctorId} does not exist." });
    }

    // Validate PatientId
    var patientExists = await _context.Patients.AnyAsync(p => p.Id == request.PatientId);
    if (!patientExists)
    {
        return BadRequest(new { error = $"Patient with ID {request.PatientId} does not exist." });
    }

    // Create Chat
    var chat = new Chat { DoctorId = request.DoctorId, PatientId = request.PatientId };
    _context.Chats.Add(chat);
    await _context.SaveChangesAsync();

    return Ok(chat);
}

[HttpPost("send")]
public async Task<IActionResult> SendMessage([FromBody] JsonElement payload)
{
    if (!payload.TryGetProperty("chatId", out JsonElement chatIdElement) ||
        !payload.TryGetProperty("sender", out JsonElement senderElement) ||
        !payload.TryGetProperty("content", out JsonElement contentElement))
    {
        return BadRequest(new { error = "Missing required fields: chatId, sender, or content." });
    }

    try
    {
        var chatId = chatIdElement.GetInt32();
        var sender = senderElement.GetString();
        var content = contentElement.GetString();

        if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(content))
        {
            return BadRequest(new { error = "Sender and Content cannot be null or empty." });
        }

        var chatExists = await _context.Chats.AnyAsync(c => c.Id == chatId);
        
        if (!chatExists)
        {
            return NotFound(new { error = $"Chat with ID {chatId} not found." });
        }

        var message = new Message
        {
            ChatId = chatId,
            Sender = sender,
            Content = content,
            Timestamp = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Use SignalR to notify clients in real time
        await _hubContext.Clients.Group(chatId.ToString())
            .SendAsync("ReceiveMessage", chatId, sender, content);

        return Ok(message);
    }
    catch (Exception ex)
    {
        return BadRequest(new { error = "Invalid data format.", details = ex.Message });
    }
}

[HttpGet("messages/{chatId}")]
public async Task<IActionResult> GetMessages(int chatId)
{
    var messages = await _context.Messages
        .Where(m => m.ChatId == chatId)
        .OrderBy(m => m.Timestamp)
        .Select(m => new
        {
            m.Id,
            m.ChatId,
            m.Sender,
            m.Content,
            m.Timestamp,
            Chat = new
            {
                m.Chat.Id,
                m.Chat.DoctorId,
                m.Chat.PatientId
            }
        })
        .ToListAsync();

    if (!messages.Any())
    {
        return NotFound(new { message = $"No messages found for ChatId: {chatId}" });
    }

    return Ok(messages);
}
[HttpGet("contacts/{authId}")]
public async Task<IActionResult> GetContactsByAuthId(int authId, [FromQuery] string role)
{
    if (string.IsNullOrWhiteSpace(role) || (role != "doctor" && role != "patient"))
    {
        return BadRequest("Invalid role parameter. Must be 'doctor' or 'patient'.");
    }

    // Determine whether to filter by DoctorId or PatientId based on the role
    var isDoctor = role.ToLower() == "doctor";

    var chats = await _context.Chats
        .Where(c => isDoctor ? c.DoctorId == authId : c.PatientId == authId)
        .Include(c => c.Doctor) // Include Doctor details
        .Include(c => c.Patient) // Include Patient details
        .Select(c => new
        {
            ChatId = c.Id,
            IsDoctor = isDoctor, // Determine if the user is the doctor
            OtherPartyId = isDoctor ? c.Patient.Id : c.Doctor.Id,
            OtherPartyName = isDoctor
                ? $"{c.Patient.Nom}"
                : $"{c.Doctor.Nom}",
            LastMessage = _context.Messages
                .Where(m => m.ChatId == c.Id)
                .OrderByDescending(m => m.Timestamp)
                .Select(m => m.Content)
                .FirstOrDefault(),
            LastMessageTime = _context.Messages
                .Where(m => m.ChatId == c.Id)
                .OrderByDescending(m => m.Timestamp)
                .Select(m => m.Timestamp)
                .FirstOrDefault()
        })
        .ToListAsync();

    // Return the filtered chats
    return Ok(chats);
}

    }
}