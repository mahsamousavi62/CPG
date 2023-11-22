using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
