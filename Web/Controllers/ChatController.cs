using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;
using Web.DTOs;

namespace Web.Controllers
{
	[Route("api/chat")]
	[ApiController]
	public class ChatController : Controller
	{

		private readonly WebContext _context;
		private readonly IHubContext<ChatHub> _chatHub;
		public ChatController(WebContext context, IHubContext<ChatHub> chatHub)
		{
			_context = context;
			_chatHub = chatHub;
		}


		[HttpGet("get-or-create-conversation")]
		public async Task<IActionResult> GetOrCreateConversationForCustomer()
		{
			var currentUserName = User.Identity.Name;
			var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == currentUserName);

			if (currentUser == null)
			{
				return NotFound("Vui lòng đăng nhập để thực hiện thao tác!");
			}

			var existingConversation = await _context.Conversations
				.FirstOrDefaultAsync(c => c.UserId == currentUser.UserId && !c.ConversationMembers.Any());

			if (existingConversation != null)
			{
				return Ok(existingConversation.ConversationId);
			}

			// Nếu không tìm thấy cuộc trò chuyện hiện có, tạo mới
			var newConversation = new Conversation
			{
				UserId = currentUser.UserId,
				CreatedTime = DateTime.UtcNow,
				UpdatedTime = DateTime.UtcNow,
				IsActive = true,
				IsArchived = false,
				IsDeleted = false
			};

			await _context.Conversations.AddAsync(newConversation);
			await _context.SaveChangesAsync();

			return Ok(newConversation.ConversationId);
		}

		[HttpPost("send-customer-message")]
		public async Task<IActionResult> SendCustomerMessage([FromBody] ChatMessageDto messageDto)
		{
			var currentUserName = User.Identity.Name;
			var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == currentUserName);

			if (currentUser == null)
			{
				return NotFound("Không tìm thấy người dùng!");
			}

			var message = new Message
			{
				ConversationId = messageDto.ConversationId,
				SenderId = currentUser.UserId,
				MessageText = messageDto.MessageText,
				SentTime = DateTime.UtcNow,
				MessageType = messageDto.MessageType
			};

			await _context.Messages.AddAsync(message);
			await _context.SaveChangesAsync();

			// Gửi thông báo đến tất cả admin
			var adminUsers = _context.Users.Where(u => u.UserType == "Admin").ToList();
			foreach (var admin in adminUsers)
			{
				await _chatHub.Clients.User(admin.UserId.ToString()).SendAsync("ReceiveMessage", message);
			}

			return Ok();
		}

		[HttpGet("system-conversations")]
		public async Task<IActionResult> GetSystemConversations()
		{
			var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			var systemConversations = await _context.Conversations
				.Include(c => c.ConversationMembers)
				.Where(c => c.IsActive.GetValueOrDefault() && !c.IsArchived.GetValueOrDefault() && !c.IsDeleted.GetValueOrDefault())
				.Where(c => !c.ConversationMembers.Any())
				.Select(c => new
				{
					ConversationId = c.ConversationId,
					UpdatedTime = c.UpdatedTime,
					UnreadMessagesCount = _context.Messages.Count(m => m.ConversationId == c.ConversationId &&
										  !_context.MessageReadStatuses.Any(s => s.MessageId == m.MessageId && s.UserId.ToString() == userId))
				})
				.OrderByDescending(c => c.UpdatedTime)
				.ToListAsync();

			return Ok(systemConversations);
		}


		[HttpGet("conversation-messages/{conversationId}")]
		public async Task<IActionResult> GetMessagesForConversation(int conversationId)
		{
			var messages = await _context.Messages
				.Include(m => m.Sender)
				.Where(m => m.ConversationId == conversationId)
				.OrderBy(m => m.SentTime)
				.Select(m => new MessageDto
				{
					MessageId = m.MessageId,
					SenderName = m.Sender.Username,
					SenderRole = m.Sender.UserType,
					MessageText = m.MessageText,
					SentTime = m.SentTime,
					MessageType = m.MessageType
				})
				.ToListAsync();

			return Ok(messages);
		}





	}
}
