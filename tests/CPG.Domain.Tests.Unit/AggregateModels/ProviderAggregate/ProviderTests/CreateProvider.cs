using System;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.File;
using CPG.Domain.SharedKernel.Minio;
using CPG.Domain.Tests.Unit.Helpers;
using CPG.Infrastructure.File;
using FluentAssertions;
using Moq;
using NSubstitute.ExceptionExtensions;
using Xunit;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.Tests.Unit.AggregateModels.ProviderAggregate.ProviderTests;

public class CreateProvider : AggregateTestHelper
{

    private static Provider Act(PersianName persianName, EnglishName englishName,
        ProviderType providerType, Logo logo, string providerData, PaymentMethodType[] details)
           => Provider.Create(persianName, englishName, providerType, logo, providerData, details);

    //[Fact]
    public void given_valid_data_CPG_Provider_should_be_created()
    {
        IFile file = new FormFileProxy(ReadFile());
        Logo logo = new(file);
        PersianName persianName = new(GetProviderPersianName);
        EnglishName englishName = new(GetProviderEnglishName);
        var minioMock = new Mock<IMinioProvider>();
        minioMock.Setup(m => m.PutObject(Enums.UploadFromEntityType.Provider.ToString(), file));

        var provider = Act(persianName, englishName, ProviderType.Vandar, logo, GetProviderData, GetPaymentMethods);

        provider.Should().NotBeNull();
        provider.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    public void given_empty_persianName_must_throw_exception(string persianName)
    {
       var exeption= Assert.Throws<EmptyPersianNameException>(() =>new PersianName(persianName));
        Assert.IsType<EmptyPersianNameException>(exeption);
        Assert.Equal("empty_persianName", exeption.Code);
    }

    [Theory]
    [InlineData("",typeof(EmptyPersianNameException))]
    [InlineData("شش",typeof(InvalidPersianNameCharachterException))]
    [InlineData("dsdssd",typeof(InvalidPersianNameException))]
    public void given_invaliad_persianName_must_throw_Exception(string persianName,Type exceptionType) 
    {
        Exception ex = Assert.Throws(exceptionType, () => new PersianName(persianName));
    }


    [Theory]
    [InlineData("", typeof(EmptyEnglishNameException))]
    [InlineData("dd", typeof(InvalidEnglishNameCharachterException))]
    [InlineData("بسیسیبیس", typeof(InvalidEnglishNameException))]
    public void given_invaliad_englishName_must_throw_Exception(string englishName, Type exceptionType)
    {
        Exception ex = Assert.Throws(exceptionType, () => new EnglishName(englishName));
    }


    [Theory]
    [InlineData()]
    public void given_invaliad_file_must_throw_exception()
    {
        IFile file = new FormFileProxy(ReadFile());
        Logo logo = new(file);
    }

}