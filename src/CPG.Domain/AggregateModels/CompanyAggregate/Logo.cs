using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using System.Text.RegularExpressions;

namespace CPG.Domain.AggregateModels.CompanyAggregate;

public class Logo
{

    public Logo()
    {
        
    }
    public string Value { get; init; }

    public Logo(string logo)
    {
        if (string.IsNullOrWhiteSpace(logo))
            throw new EmptyLogoException($"Parameter {nameof(logo)} cannot be empty.");

        if (!Regex.IsMatch(logo, @"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$"))
            throw new InvalidPathLogoException($"Parameter {nameof(logo)} has a  invalid path.");

        //valid lenght of logo
        string maxlenght = "2*1024*1024";


        //valid extention:"jpg"، "jpeg"، "png"،"tiff" و "svg" 
        if (!Regex.IsMatch(logo, "^.*\\.(jpg|JPG|gif|jpeg|png|tiff|svg)$"))
            throw new InvalidLogoExtentionException($"Parameter {nameof(logo)} has a  invalid extention.");

        Value = logo;
    }
}
