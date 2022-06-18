using AutoMapper;
using Chatto.Infrastructure;
using Chatto.Models;
using Shared.Models;

namespace Chatto;

public class AutoMapperSettings : Profile
{
    public AutoMapperSettings()
    {
        CreateMap<TextChannelModel, TextChannel>();
        CreateMap<TextChannel, TextChannelModel>();
    }
}