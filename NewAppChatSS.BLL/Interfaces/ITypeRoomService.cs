using NewAppChatSS.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.BLL.Interfaces
{
    public interface ITypeRoomService
    {
        void MakeTypeRoom(TypeRoomDTO typeRoomDTO);
        TypeRoomDTO GetTypeRoom(Int32 id);
        IEnumerable<TypeRoomDTO> GetTypesRoomsDTO();
    }
}
