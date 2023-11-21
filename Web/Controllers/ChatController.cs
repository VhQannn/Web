using System.ComponentModel.Design;
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
		private readonly  TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
		private readonly DateTime vietnamTime;
		public ChatController(WebContext context, IHubContext<ChatHub> chatHub)
		{
			_context = context;
			_chatHub = chatHub;
			vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
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
				CreatedTime = vietnamTime,
				UpdatedTime = vietnamTime,
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
				SentTime = vietnamTime,
				MessageType = messageDto.MessageType
			};

			var savedMessage = await _context.Messages.AddAsync(message);
			// Tìm và cập nhật cuộc trò chuyện
			var conversation = await _context.Conversations
				.FirstOrDefaultAsync(c => c.ConversationId == messageDto.ConversationId);
			if (conversation != null)
			{
				conversation.UpdatedTime = vietnamTime;
				_context.Conversations.Update(conversation);
			}

			await _context.SaveChangesAsync();

			// Gửi tin nhắn tới group tương ứng với cuộc trò chuyện
			await _chatHub.Clients.Group($"Conversation-{messageDto.ConversationId}")
				.SendAsync("ReceiveMessage", new MessageDto
				{
					MessageId = savedMessage.Entity.MessageId,
					SenderName = savedMessage.Entity.Sender.Username,
					SenderRole = savedMessage.Entity.Sender.UserType,
					IsRead = false,
					MessageText = savedMessage.Entity.MessageText,
					SentTime = savedMessage.Entity.SentTime,
					MessageType = savedMessage.Entity.MessageType
				}, messageDto.ConversationId);

			await _chatHub.Clients.Group("Admins").SendAsync("NewMessageNotification", messageDto.ConversationId);


			return Ok();
		}

		[HttpGet("system-conversations")]
		public async Task<IActionResult> GetSystemConversations()
		{
			var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			var systemConversations = await _context.Conversations
				.Include(c => c.ConversationMembers)
				.Where(c => (c.IsActive == true || c.IsActive == null)
							&& (c.IsArchived == false || c.IsArchived == null)
							&& (c.IsDeleted == false || c.IsDeleted == null))
				.Where(c => !c.ConversationMembers.Any())
				.Include(c => c.Messages)
				.Where(c => c.Messages.Any())
				.Select(c => new
				{
					ConversationId = c.ConversationId,
					ConversationName = _context.Users.Where(u => u.UserId == c.UserId).Select(u => u.Username).FirstOrDefault(),
					UpdatedTime = c.UpdatedTime,
					UnreadMessagesCount = _context.Messages.Count(m => m.ConversationId == c.ConversationId &&
											  !_context.MessageReadStatuses.Any(s => s.MessageId == m.MessageId) &&
											  m.Sender.UserType == "Customer"),
					LastMessage = _context.Messages.Where(m => m.ConversationId == c.ConversationId)
						   .OrderByDescending(m => m.SentTime)
						   .Select(m => m.MessageText)
						   .FirstOrDefault()
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
					IsRead = _context.MessageReadStatuses.Any(s => s.MessageId == m.MessageId),
					MessageText = m.MessageText,
					SentTime = m.SentTime,
					MessageType = m.MessageType
				})
				.ToListAsync();

			return Ok(messages);
		}


		[HttpPost("mark-messages-as-read")]
		public async Task<IActionResult> MarkMessagesAsRead([FromBody] MarkAsReadDto dto)
		{
			var currentUserName = User.Identity.Name;
			var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == currentUserName);

			if (currentUser != null)
			{
				foreach (var messageId in dto.MessageIds)
				{
					// Kiểm tra xem tin nhắn đã được đánh dấu là đã đọc bởi người dùng này chưa
					var existingStatus = await _context.MessageReadStatuses
						.FirstOrDefaultAsync(s => s.MessageId == messageId && s.UserId == currentUser.UserId);

					if (existingStatus == null)
					{
						
						var status = new MessageReadStatus
						{
							MessageId = messageId,
							UserId = currentUser.UserId,
							ReadTime = vietnamTime
						};
						_context.MessageReadStatuses.Add(status);

						var message = await _context.Messages.Where(m => m.MessageId == messageId).FirstOrDefaultAsync();
						if(message != null)
						{
							await _chatHub.Clients.Group($"Conversation-{message.ConversationId}")
								.SendAsync("MessageRead", messageId);
						}

						
					}
				}

				await _context.SaveChangesAsync();
			}
			return Ok();
		}


		[HttpGet("search-conversations")]
		public async Task<IActionResult> SearchConversations(string searchTerm)
		{
			var filteredConversations = await _context.Conversations
				.Include(c => c.ConversationMembers)
				.Where(c => (c.IsActive == true || c.IsActive == null)
							&& (c.IsArchived == false || c.IsArchived == null)
							&& (c.IsDeleted == false || c.IsDeleted == null))
				.Where(c => !c.ConversationMembers.Any())
				.Include(c => c.Messages)
				.Where(c => c.Messages.Any())
				.Include(c => c.User)
				.Where(c => c.User.Username.Contains(searchTerm))
				.Select(c => new
				{
					ConversationId = c.ConversationId,
					ConversationName = _context.Users.Where(u => u.UserId == c.UserId).Select(u => u.Username).FirstOrDefault(),
					UpdatedTime = c.UpdatedTime,
					UnreadMessagesCount = _context.Messages.Count(m => m.ConversationId == c.ConversationId &&
											  !_context.MessageReadStatuses.Any(s => s.MessageId == m.MessageId) &&
											  m.Sender.UserType == "Customer"),
					LastMessage = _context.Messages.Where(m => m.ConversationId == c.ConversationId)
						   .OrderByDescending(m => m.SentTime)
						   .Select(m => m.MessageText)
						   .FirstOrDefault()
				})
				.OrderByDescending(c => c.UpdatedTime)
				.ToListAsync();

			return Ok(filteredConversations);
		}





	}
}
