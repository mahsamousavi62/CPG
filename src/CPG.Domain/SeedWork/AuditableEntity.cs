using System;

namespace CPG.Domain.SeedWork;

public abstract class AuditableEntity<T> : AuditableEntity
{
    public T Id { get; internal set; }

    private int? _requestedHashCode;

    public bool IsTransient()
    {
        return Id.Equals(default(T));
    }

    public override bool Equals(object obj)
    {
        if (obj is not AuditableEntity<T>)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        var item = (AuditableEntity<T>)obj;

        if (item.IsTransient() || IsTransient())
            return false;

        return item.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        if (IsTransient())
        {
            return base.GetHashCode();
        }

        if (!_requestedHashCode.HasValue)
            _requestedHashCode = Id.GetHashCode() ^ 31;

        return _requestedHashCode.Value;
    }

    public static bool operator ==(AuditableEntity<T> left, AuditableEntity<T> right)
    {
        return left?.Equals(right) ?? Equals(right, null);
    }

    public static bool operator !=(AuditableEntity<T> left, AuditableEntity<T> right)
    {
        return !(left == right);
    }
}

public abstract class AuditableEntity : Entity
{
    public DateTime CreationDate { get; set; }

    public long? CreationUserId { get; set; }

    public DateTime? ModificationDate { get; set; }

    public long? ModificationUserId { get; set; }
}