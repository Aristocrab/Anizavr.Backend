using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ValueOf;

namespace Application.Entities;

[Keyless]
public partial class Email : ValueOf<string, Email>
{
    protected override void Validate()
    {
        if (!EmailRegex().IsMatch(Value))
            throw new ArgumentException($"Email '{Value}' is not valid");
    }
    
    [GeneratedRegex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")]
    private static partial Regex EmailRegex();
}