using Backend.Models.DTOs;
using Backend.Models.Entities;

namespace Backend.Mappings
{
    public static class MappingProfile
    {
        #region User Mappings

        public static UserDto ToDto(this Chatuser user)
        {
            return new UserDto
            {
                Id = user.Iduser,
                Username = user.Username,
                IsOnline = user.Isconnect ?? false,
                CreatedAt = user.Createdat
            };
        }

        #endregion

        #region Message Mappings

        public static MessageDto ToDto(this Message message, long? chatroomId = null)
        {
            return new MessageDto
            {
                Id = message.Idmsg,
                SenderId = message.Iduser,
                ChatroomId = chatroomId,           // On passe manuellement le ChatroomId car il vient de Msg_Appartient
                Content = message.Content,
                CreatedAt = message.Createdat
            };
        }

        public static Message ToEntity(this CreateMessageDto dto)
        {
            return new Message
            {
                Iduser = dto.SenderId,
                Content = dto.Content,
                Createdat = DateTime.UtcNow
            };
        }

        #endregion

        #region Chatroom Mappings

        public static ChatroomDto ToDto(this Chatroom chatroom)
        {
            return new ChatroomDto
            {
                Id = chatroom.Idchatroom,
                CreatedByUserId = chatroom.Iduser,
                CreatedAt = chatroom.Roomcreatedat
            };
        }

        #endregion

        #region ChatroomMember Mappings

        public static ChatroomMemberDto ToDto(this Chatroommember member)
        {
            return new ChatroomMemberDto
            {
                ChatroomId = member.Idchatroom,
                UserId = member.Iduser,
                JoinedAt = member.Useraddat
            };
        }

        #endregion

        #region Attachment Mappings

        public static AttachmentDto ToDto(this Attachment attachment)
        {
            return new AttachmentDto
            {
                Id = attachment.Idattachment,
                Size = attachment.Attachmentsize,
                Path = attachment.Attachmentpath
            };
        }

        #endregion

        #region MsgAppartient Mappings (Lien Message - Chatroom)

        public static MsgAppartientDto ToDto(this MsgAppartient msgAppartient)
        {
            return new MsgAppartientDto
            {
                MessageId = msgAppartient.Idmsg,
                ChatroomId = msgAppartient.Idchatroom,
                SentAt = msgAppartient.Msgsendchatroomat
            };
        }

        #endregion

        #region UserRecive Mappings (Statut de lecture)

        public static UserReciveDto ToDto(this UserRecive userRecive)
        {
            return new UserReciveDto
            {
                UserId = userRecive.Iduser,
                MessageId = userRecive.Idmsg,
                ReceivedAt = userRecive.Recivedat,
                IsViewed = userRecive.Msgvue,
                ViewedAt = userRecive.Vueat
            };
        }

        #endregion

        // ====================== Méthodes pour List ======================

        public static List<UserDto> ToDtoList(this IEnumerable<Chatuser> users)
            => users.Select(u => u.ToDto()).ToList();

        public static List<MessageDto> ToDtoList(this IEnumerable<Message> messages, long? chatroomId = null)
            => messages.Select(m => m.ToDto(chatroomId)).ToList();

        public static List<ChatroomDto> ToDtoList(this IEnumerable<Chatroom> chatrooms)
            => chatrooms.Select(c => c.ToDto()).ToList();
    }
}