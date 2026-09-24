using System;
using WixToolset.Dtf.WindowsInstaller;

namespace App.Actions;

/// <summary>
/// Managed custom actions executed by the MSI during install/uninstall. Each public static
/// method tagged [CustomAction] becomes callable from a &lt;CustomAction&gt; element in the
/// WiX authoring (see Product.wxs) once this project's output is referenced as a Binary.
/// </summary>
public static class Actions
{
    // Immediate custom action: runs in-process with the installer UI/engine, so it can read
    // and write session properties directly. Use these for validation and for staging data
    // (via CustomActionData) that a later deferred action will need.
    [CustomAction]
    public static ActionResult ValidateSampleProperty(Session session)
    {
        return Invoke(session, () =>
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

    // Deferred custom action: runs with elevated (system) privileges but cannot touch session
    // properties, so any input has to be handed in through CustomActionData - wire that up in
    // WiX with a <CustomAction ... Property="ThisActionsId"> that Sets the data before it runs.
    [CustomAction]
    public static ActionResult WriteSampleMarkerFile(Session session)
    {
        return Invoke(session, () =>
        {
            var installFolder = session.CustomActionData["INSTALLFOLDER"];
            var markerPath = System.IO.Path.Combine(installFolder, "installed-by-custom-action.txt");
            System.IO.File.WriteAllText(markerPath, $"Installed {DateTime.UtcNow:O}{Environment.NewLine}");
        });
    }

    // Shared error handling so every action logs and fails cleanly instead of crashing the MSI.
    private static ActionResult Invoke(Session session, Action action)
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
