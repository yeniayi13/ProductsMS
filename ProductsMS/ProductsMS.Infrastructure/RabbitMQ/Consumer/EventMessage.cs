﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Infrastructure.RabbitMQ.Consumer
{
    [ExcludeFromCodeCoverage]
    public class EventMessage<T>
    {
        public string EventType { get; set; } // "USER_CREATED" o "USER_UPDATED o "USER_DELETED"
        public T Data { get; set; } // Contiene los datos del usuario
    }
}
