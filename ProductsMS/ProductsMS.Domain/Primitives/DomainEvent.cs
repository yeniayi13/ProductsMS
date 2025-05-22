using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace ProductsMS.Common.Primitives
{
    //TODO: Aqui utiliza un record, revisar el porque

    [ExcludeFromCodeCoverage]
    public record DomainEvent(Guid id) : INotification;
}