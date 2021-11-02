using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VieTrails_Web.Models;

namespace VieTrails_Web.Service.IService
{
    public interface IAccountService : IService<User>
    {
        Task<User> LoginAsync(string url, User objToCreate);
        Task<bool> RegisterAsync(string url, User objToCreate);
    }
}
