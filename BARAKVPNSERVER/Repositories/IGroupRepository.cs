using BARAKVPNSERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Repositories
{
    public interface IGroupRepository
    {
        Task<int> CreateGroup(Group group);
        Task<Group> GetGroupGroup(int id);
        Task<List<string>> UpdateGroup(GroupDTO group, User user);
    }
}
