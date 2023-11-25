using CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate
{
    public class PersianName
    {
            public string Value { get; init; }

            public PersianName(string persianName)
            {
                if (string.IsNullOrWhiteSpace(persianName))
                    throw new UserCreationException($"Parameter {nameof(persianName)} cannot be empty.");

            //TODO:check persian letter

           // if (!Regex.IsMatch(persianName, "[A-Za-z\\s]+"))
             //   throw new InvalidPersianNameException ($"Parameter {nameof(persianName)} is invalid.");

            Value = persianName;
            }
    }
}
