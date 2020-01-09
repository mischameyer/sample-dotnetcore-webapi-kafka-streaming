using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StreamingService.Service
{
    public interface IProducerService
    {
        void CreateProducer();

        Task<string> SendMessage(string topic, string message, bool display, int key);
    }
}
