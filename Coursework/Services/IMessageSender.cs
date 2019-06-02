using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsulService.Services
{
    public interface IMessageSender
    {
        void SendMessage(string message);
    }
}
