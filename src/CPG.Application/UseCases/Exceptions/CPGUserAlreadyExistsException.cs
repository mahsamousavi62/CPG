namespace CPG.Application.UseCases.Exceptions
{
    public class CPGUserAlreadyExistsException : ApplicationException
    {
        public override string Code => "CPG_user_already_exists";
        public string Email { get; }

        public CPGUserAlreadyExistsException(string email) : base($"CPG user already exists with email {email}.") 
            => Email = email;
    }
}
