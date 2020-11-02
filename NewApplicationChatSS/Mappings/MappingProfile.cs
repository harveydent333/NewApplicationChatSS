using AutoMapper;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.DAL.Entities;
using NewApplicationChatSS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
