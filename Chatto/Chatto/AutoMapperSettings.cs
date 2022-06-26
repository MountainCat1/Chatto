using AutoMapper;
using Chatto.Infrastructure;
using Chatto.Models;

namespace Chatto;

public class AutoMapperSettings : Profile
{
    public AutoMapperSettings()
    {
        CreateMap<TextChannelModel, TextChannel>();
        CreateMap<TextChannel, TextChannelModel>();

        CreateMap<SendMessageModel, Message>();
        CreateMap<Message, SendMessageModel>();
        
        CreateMap<Message, MessageModel>();
        CreateMap<MessageModel, Message>();
    }
}