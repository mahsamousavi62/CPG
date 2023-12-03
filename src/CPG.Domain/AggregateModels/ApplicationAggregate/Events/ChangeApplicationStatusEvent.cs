using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Events;

internal class ChangeApplicationStatusEvent(long applicationId, bool isActive, DateTime dateTime) : INotification
{
    public long ApplicationId { get; } = applicationId;

    public DateTime ModificationDateTime { get; set; } = dateTime;

    public bool IsActive { get; set; } = isActive;
}