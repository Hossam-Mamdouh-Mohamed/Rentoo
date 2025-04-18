using Microsoft.Extensions.Localization;
using Rentoo.Web.Resources;

namespace Rentoo.Web.Localization;
public interface ISharedLocalizationService
{
    string this[string key] { get; }
}

public class SharedLocalizationService : ISharedLocalizationService
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public SharedLocalizationService(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
    }

    public string this[string key] => _localizer[key];
}