using System.Text;

namespace NOS.Engineering.Challenge.API.Extensions;
public static class CachingExtensions
{
    private const string BASE_CACHE_KEY = "Cache";

    public static string GetCacheKey(List<string> parameters)
    {
        var sb = new StringBuilder();

        sb.Append($"{BASE_CACHE_KEY}");

        foreach (var param in parameters)
        {
            if (!string.IsNullOrWhiteSpace(param))
            {
                sb.Append($":{param}");
            }
        }

        return sb.ToString();
    }
}