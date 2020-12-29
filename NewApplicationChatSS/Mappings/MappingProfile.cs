using AutoMapper;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.DAL.Entities;
using NewApplicationChatSS.Models;
using NewApplicationChatSS.Models.Auth;

namespace NewApplicationChatSS.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RoomDTO, Room>().ReverseMap();
            CreateMap<MessageDTO, Message>().ReverseMap();
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>()
                .ForMember(x => x.PasswordHash, opt => opt.MapFrom(src => src.Password));

            CreateMap<RegisterModel, UserDTO>().ReverseMap();
            CreateMap<LoginModel, UserDTO>().ReverseMap();
            CreateMap<RoomModel, RoomDTO>().ReverseMap();
            CreateMap<MessageModel, MessageDTO>().ReverseMap();
        }
    }
}
