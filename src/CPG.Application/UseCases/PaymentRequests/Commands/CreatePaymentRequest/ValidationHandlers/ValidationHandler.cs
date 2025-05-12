namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;

internal class ValidatorHandler<T> : IValidationHandler<T>
{
    private IValidationHandler<T> _nextHandler;

    public IValidationHandler<T> SetNext(IValidationHandler<T> handler)
    {
        _nextHandler = handler;

        return handler;
    }

    public virtual void Handle(T request)
    {
        if (_nextHandler != null)
        {
            _nextHandler.Handle(request);
        }
        else
        {
            return;
        }
    }
}