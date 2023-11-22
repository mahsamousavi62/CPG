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
    public class EnglishName
    {
        public string Value { get; init; }

        public EnglishName(string englishName)
        {
            if (string.IsNullOrWhiteSpace(englishName))
                throw new EmptyEnglishNameException($"Parameter {nameof(englishName)} cannot be empty.");

            if (!Regex.IsMatch(englishName,"[A-Za-z\\s]+"))

                throw new InvalidEnglishNameException($"Parameter {nameof(englishName)} is invalid.");
           
            Value = englishName;
        }
    }
}
