using Backend.Data;
using Backend.Models.DTOs;
using Backend.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatRoomController : Controller
    {
        private readonly ChatDbContext _context;
        private readonly IConfiguration _configuration;

        public ChatRoomController(ChatDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ====================== PUBLIC ROUTES ======================



        // ====================== PROTECTED ROUTES ======================
        [HttpPost("Create")]
        [Authorize]
        public async Task<ActionResult<ChatroomDto>> Create([FromBody] CreateChatroomDto dto)
        {
            // 1. Validation du modèle
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var Owner = await _context.Chatuser.FirstOrDefaultAsync(u => u.Iduser == dto.CreatedByUserId);
                if (Owner == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "User non trouvé!"
                    });
                }

                var NewChatRoom = new Chatroom
                {
                    Iduser = dto.CreatedByUserId,
                    Roomcreatedat = DateTime.UtcNow,
                    Chatroomlabel = dto.ChatRoomLabel,

                };
                await _context.Chatroom.AddAsync(NewChatRoom);
                await _context.SaveChangesAsync();
                //creation de new chatroommember  !! 
                var NewChatRoomMember = new Chatroommember
                {
                    Iduser = dto.CreatedByUserId,
                    Idchatroom = NewChatRoom.Idchatroom,
                    Useraddat = DateTime.UtcNow,

                };
                await _context.Chatroommember.AddAsync(NewChatRoomMember);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    success = true,
                    message = "Chat Romm Ajouter avec succès."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    details = ex.Message,
                    message = "Probleme c`est produit  lors de la creation!",


                });
            }
        }

        [HttpPut("Label/{id}")]                    
        [Authorize]
        public async Task<IActionResult> UpdateChatRoomLabel(
     [FromRoute] long id,                   
     [FromBody] UpdateChatroomDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var chatRoom = await _context.Chatroom
                    .FirstOrDefaultAsync(c => c.Idchatroom == id && c.Iduser == dto.CreatedByUserId);

                if (chatRoom == null)
                {
                    return BadRequest(new
                    {
                        message = "Le chatRoom n'existe pas ou vous n'avez pas le droit de le modifier !"
                    });
                }

                chatRoom.Chatroomlabel = dto.ChatRoomLabel;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "ChatRoom label modifié avec succès."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Une erreur est survenue lors de la modification.",
                    details = ex.Message
                });
            }
        }
    }
}

