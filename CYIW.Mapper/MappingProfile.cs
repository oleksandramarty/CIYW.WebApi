using AutoMapper;
using CIYW.Domain.Models.User;
using CIYW.Mediatr.Auth.Queries;
using CIYW.Mediatr.Users.Requests;
using CIYW.Models.Responses.Users;
using Microsoft.AspNetCore.Identity;

namespace CIYW.Models.Mapping;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        this.CreateMap<User, CurrentUserResponse>();
        this.CreateMap<Role, CurrentUserResponse>()
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Name));


        this.CreateMap<CreateUserCommand, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsBlocked, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.IsTemporaryPassword, opt => opt.MapFrom(src => true));
    }
}