using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StreamingService.Service
{
    public interface IProducerService2
    {

        void CreateProducer();

        Task<string> SendMessage(string topic, string order, bool display, int key);

    }
}
