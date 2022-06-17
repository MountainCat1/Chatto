using AutoMapper;
using CommunicationAPI.Infrastructure;
using Shared.Models;

namespace ChattoAuth;

public class AutoMapperSettings : Profile
{
    public AutoMapperSettings()
    {
        CreateMap<CreateUserModel, User>();
    }
}