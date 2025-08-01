﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Core.RabbitMQ
{
    public interface IEventBus<T>
    {
        Task PublishMessageAsync(T data, string queueName, string eventType);
    }
}
