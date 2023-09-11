namespace Daryaftyar.Application.UseCases.Exceptions
{
    public class DaryaftyarUserAlreadyExistsException : ApplicationException
    {
        public override string Code => "Daryaftyar_user_already_exists";
        public string Email { get; }

        public DaryaftyarUserAlreadyExistsException(string email) : base($"Daryaftyar user already exists with email {email}.") 
            => Email = email;
    }
}
