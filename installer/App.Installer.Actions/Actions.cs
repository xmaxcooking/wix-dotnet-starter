using WixToolset.Dtf.WindowsInstaller;

namespace App.Installer.Actions;

public static class Actions
{
    [CustomAction]
    public static ActionResult ValidateSampleProperty(Session session)
    {
        return invoke(session, () =>
        {
            var value = session["SAMPLEPROPERTY"];
            if (string.IsNullOrWhiteSpace(value))
            {
                session["SAMPLEPROPERTYVALID"] = "0";
                session.Log("SAMPLEPROPERTY was not set.");
                return;
            }

            session["SAMPLEPROPERTYVALID"] = "1";
        });
    }

    [CustomAction]
    public static ActionResult WriteSampleMarkerFile(Session session)
    {
        return invoke(session, () =>
        {
            var installFolder = session.CustomActionData["INSTALLFOLDER"];
            var markerPath = System.IO.Path.Combine(installFolder, "installed-by-custom-action.txt");
            System.IO.File.WriteAllText(markerPath, $"Installed {DateTime.UtcNow:O}{Environment.NewLine}");
        });
    }

    private static ActionResult invoke(Session session, Action action)
    {
        try
        {
            action();
            return ActionResult.Success;
        }
        catch (Exception e)
        {
            session.Log("Custom action failed: " + e);
            return ActionResult.Failure;
        }
    }
}