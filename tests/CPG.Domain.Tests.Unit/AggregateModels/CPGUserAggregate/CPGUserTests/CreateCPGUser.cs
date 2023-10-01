using FluentAssertions;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.Tests.Unit.Helpers;
using Xunit;

namespace CPG.Domain.Tests.Unit.AggregateModels.CPGUserAggregate.CPGUserTests
{
    public class CreateCPGUser : AggregateTestHelper
    {
        private static CPGUser Act(UserCredential credentials, Name name, string email)
            => CPGUser.Create(credentials, name, email);

        [Fact]
        public void given_valid_data_CPG_user_should_be_created()
        {
            const string login = "Login";
            const string password = "Password";
            const string firstName = "FirstName";
            const string lastName = "LastName";
            const string emailAddress = "email@com.pl";
            var credentials = new UserCredential(login, password);
            var name = new Name(firstName, lastName);
            var email = new Email(emailAddress);

            var CPGUser = Act(credentials, name, email);

            CPGUser.Should().NotBeNull();
            CPGUser.Credentials.Login.Should().Be(login);
            CPGUser.Credentials.Password.Should().Be(password);
            CPGUser.FirstName.Should().Be(firstName);
            CPGUser.LastName.Should().Be(lastName);
            CPGUser.Email.Value.Should().Be(email);
            CPGUser.IsActive.Should().BeTrue();
        }
    }
}
