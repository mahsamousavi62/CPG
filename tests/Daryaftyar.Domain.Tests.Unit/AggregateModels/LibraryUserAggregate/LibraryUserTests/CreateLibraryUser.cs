using FluentAssertions;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate;
using Daryaftyar.Domain.Tests.Unit.Helpers;
using Xunit;

namespace Daryaftyar.Domain.Tests.Unit.AggregateModels.DaryaftyarUserAggregate.DaryaftyarUserTests
{
    public class CreateDaryaftyarUser : AggregateTestHelper
    {
        private static DaryaftyarUser Act(UserCredential credentials, Name name, string email)
            => DaryaftyarUser.Create(credentials, name, email);

        [Fact]
        public void given_valid_data_Daryaftyar_user_should_be_created()
        {
            const string login = "Login";
            const string password = "Password";
            const string firstName = "FirstName";
            const string lastName = "LastName";
            const string emailAddress = "email@com.pl";
            var credentials = new UserCredential(login, password);
            var name = new Name(firstName, lastName);
            var email = new Email(emailAddress);

            var DaryaftyarUser = Act(credentials, name, email);

            DaryaftyarUser.Should().NotBeNull();
            DaryaftyarUser.Credentials.Login.Should().Be(login);
            DaryaftyarUser.Credentials.Password.Should().Be(password);
            DaryaftyarUser.FirstName.Should().Be(firstName);
            DaryaftyarUser.LastName.Should().Be(lastName);
            DaryaftyarUser.Email.Value.Should().Be(email);
            DaryaftyarUser.IsActive.Should().BeTrue();
        }
    }
}
