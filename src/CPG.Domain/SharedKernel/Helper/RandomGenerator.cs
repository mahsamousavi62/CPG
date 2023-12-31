using System;

namespace CPG.Domain.SharedKernel.Helper;

public class RandomGenerator
{
    private static readonly Random random = new Random();
    public static string GenerateRandomDigitNumber(int numDigits)
    {
        if (numDigits <= 0)
        {
            throw new ArgumentException("Number of digits should be greater than 0.");
        }

        string number = "";

        for (int i = 0; i < 16; i++)
        {
            number += random.Next(0, 10);
        }
        return number;
    }
}
