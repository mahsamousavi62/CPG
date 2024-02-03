using System.Collections.Generic;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;

public class ShowResponse : ResponseBase
{
    public List<Data> Data { get; set; }
    public LinkData Links { get; set; }
    public MetaData Meta { get; set; }
}

public class Data
{
    public string Id { get; set; }
    public string CustomerUuid { get; set; }
    public string Token { get; set; }
    public string BankCode { get; set; }
    public string CallbackUrl { get; set; }
    public int Count { get; set; }
    public string Limit { get; set; }
    public string Mobile { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string NationalCode { get; set; }
    public string ExpirationDate { get; set; }
    public string Status { get; set; }
    public object PayerAccount { get; set; }
    public string CreatedAt { get; set; }
    public string RevokedAt { get; set; }
}

public class LinkData
{
    public string Url { get; set; }
    public string Label { get; set; }
    public bool Active { get; set; }
    public string First { get; set; }
    public string Last { get; set; }
    public object Prev { get; set; }
    public object Next { get; set; }
}

public class MetaData
{
    public int CurrentPage { get; set; }
    public int From { get; set; }
    public int LastPage { get; set; }
    public List<Link> Links { get; set; }
    public string Path { get; set; }
    public int PerPage { get; set; }
    public int To { get; set; }
    public int Total { get; set; }
}