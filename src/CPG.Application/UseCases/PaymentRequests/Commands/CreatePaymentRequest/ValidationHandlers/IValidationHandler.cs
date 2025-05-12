namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers
{
    internal interface IValidationHandler<T>
    {
        IValidationHandler<T> SetNext(IValidationHandler<T> handler);

        void Handle(T request);
    }
}