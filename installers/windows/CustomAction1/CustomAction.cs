using System;
using System.Collections.Generic;
using System.Linq;
using WixToolset.Dtf.WindowsInstaller;

namespace CustomAction1
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult copySettingsFileToUserhome(Session session)
        {
            session.Log("Begin copySettingsFileToUserhome");

            try
            {
                string usersRoot = @"C:\\Users";
                string sourceFile = @"C:\\ProgramData\\WSO2-Integrator\\settings.json";
                if (!System.IO.File.Exists(sourceFile))
                {
                    session.Log($"Source settings.json not found: {sourceFile}");
                    return ActionResult.Failure;
                }

                var userDirs = System.IO.Directory.GetDirectories(usersRoot);
                foreach (var userDir in userDirs)
                {
                    string appDataRoaming = System.IO.Path.Combine(userDir, "AppData", "Roaming");
                    if (System.IO.Directory.Exists(appDataRoaming))
                    {
                        string targetDir = System.IO.Path.Combine(appDataRoaming, "WSO2-Integrator", "User");
                        if (!System.IO.Directory.Exists(targetDir))
                        {
                            System.IO.Directory.CreateDirectory(targetDir);
                        }
                        string targetFile = System.IO.Path.Combine(targetDir, "settings.json");
                        System.IO.File.Copy(sourceFile, targetFile, true);
                        session.Log($"Copied settings.json to: {targetFile}");
                    }
                }
            }
            catch (Exception ex)
            {
                session.Log($"Error copying settings.json: {ex}");
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }
    }
}
