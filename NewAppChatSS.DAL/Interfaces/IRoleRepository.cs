namespace NewAppChatSS.DAL.Interfaces
{
    public interface IRoleRepository
    {
        string FindRoleIdByName(string roleName);

        string FindRoleNameById(string roleId);
    }
}