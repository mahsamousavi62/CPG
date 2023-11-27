using System;

namespace CPG.Domain.AggregateModels.UserAggregate
{
    public class UserRole
    {
        public UserRole(short roleType)
        {
            RoleType = roleType;
            CreationDate = DateTime.Now;
        }

        public long Id { get; set; }

        public long UserId { get; set; }

        public short RoleType { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ModificationDate { get; set; }

    }
}
