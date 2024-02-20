using CPG.Tests.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.Tests.Unit.Helpers;

public class AggregateTestHelper : TestBase
{
    //protected CPGUser GetValidCPGUserAggregate() => PrepareCPGUserAggregate();
    //protected Book GetValidBookAggregate() => PrepareBookAggregate();

    protected string GetProviderPersianName => CreatePersianString(6);
    protected string GetProviderEnglishName => CreateString();
    protected string GetProviderData => CreateString();
    protected static PaymentMethodType[] GetPaymentMethods => Enum.GetValues(typeof(PaymentMethodType)).Cast<PaymentMethodType>().ToArray();

    protected string CurrentDirectory = AppContext.BaseDirectory;

    protected FormFile ReadFile()
    {
        //var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //var path = System.IO.Path.Combine(directory, "myFile.txt");

        DirectoryInfo directory = new (CurrentDirectory);

        var filePath = Path.Combine(directory.FullName,"Resource","Test.jpg");

        using (var stream = File.OpenRead(filePath))
        {
            return new FormFile(stream, 0, stream.Length, null, "test.jpg");
        };
    }
    protected FormFile ReadInvalidFile()
    {
        DirectoryInfo directory = new (CurrentDirectory);

        var filePath = Path.Combine(directory.FullName,"Resource","invalidFile.docx");
        using (var stream = File.OpenRead(filePath))
        {
            return new FormFile(stream, 0, stream.Length, null, "invalidFile.docx");
        };
    }
}
