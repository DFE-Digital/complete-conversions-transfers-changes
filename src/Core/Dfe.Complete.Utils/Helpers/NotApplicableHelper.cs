namespace Dfe.Complete.Utils.Helpers;

public static class NotApplicableHelper
{
    public static T? NullWhenNotApplicable<T>(bool? notApplicable, T? value) where T : struct
    {
        return notApplicable != true ? value : null;
    }
}