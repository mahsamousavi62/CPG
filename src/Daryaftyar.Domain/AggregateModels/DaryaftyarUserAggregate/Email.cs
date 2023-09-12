using System;
using System.Collections.Generic;
using System.Net.Mail;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Exceptions;
using Daryaftyar.Domain.Exceptions;
using Daryaftyar.Domain.SeedWork;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate
{
    public record Email
    {
        public string Value { get; init; }

        private Email() { }

        public Email(string email)
        {
            // TODO: Implement Guard clause
            try
            {
                var emailAddress = new MailAddress(email);

                Value = emailAddress.Address;
            }
            catch (Exception ex)
            {
                throw new InvalidEmailException(email, ex.Message);
            }
        }

        public static implicit operator string(Email email) => email.Value;
        public static implicit operator Email(string email) => new(email);

        public override string ToString() => Value;
    }
}