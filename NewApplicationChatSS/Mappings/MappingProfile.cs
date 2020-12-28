using AutoMapper;
using NewAppChatSS.BLL.Models;
using NewAppChatSS.DAL.Entities;
using NewApplicationChatSS.Models;

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

            CreateMap<RegisterViewModel, UserDTO>().ReverseMap();
            CreateMap<LoginViewModel, UserDTO>().ReverseMap();
            CreateMap<RoomViewModel, RoomDTO>().ReverseMap();
            CreateMap<MessageViewModel, MessageDTO>().ReverseMap();
        }
    }
}
