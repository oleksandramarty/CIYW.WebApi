using Microsoft.Extensions.DependencyInjection;

namespace CIYW.Auth.Tokens;

public static class TokenGeneratorCollectionExtensions
{
    public static IServiceCollection AddTokenGenerator(this IServiceCollection collection,
        Action<TokenGeneratorOptions> setupAction)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

        collection.Configure(setupAction);
        return collection.AddTransient<TokenGenerator>();
    }
}