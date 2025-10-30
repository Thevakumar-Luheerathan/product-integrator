using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Management;
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
                string sourceFile = @"C:\ProgramData\WSO2 Integrator\settings.json";
                if (!System.IO.File.Exists(sourceFile))
                {
                    session.Log($"Source settings.json not found: {sourceFile}");
                    //return ActionResult.Failure;
                }

                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_UserProfile WHERE Special = False");

                using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
                {
                    foreach (ManagementObject profile in searcher.Get())
                    {
                        string localPath = (string)profile["LocalPath"];
                        if (string.IsNullOrEmpty(localPath)) continue;

                        string username = localPath.Substring(localPath.LastIndexOf('\\') + 1);
                        var user = UserPrincipal.FindByIdentity(context, username);

                        if (user == null || user.Enabled == false)
                        {
                            continue;
                        }

                        session.Log($"User directory: {localPath}");
                        string appDataRoaming = System.IO.Path.Combine(localPath, "AppData", "Roaming");
                        if (System.IO.Directory.Exists(appDataRoaming))
                        {
                            session.Log($"AppData\\Roaming directory found: {appDataRoaming}");
                            string targetDir = System.IO.Path.Combine(appDataRoaming, "WSO2 Integrator", "User");
                            if (!System.IO.Directory.Exists(targetDir))
                            {
                                System.IO.Directory.CreateDirectory(targetDir);
                            }
                            string targetFile = System.IO.Path.Combine(targetDir, "settings.json");
                            session.Log($"Copying settings.json to: {targetFile}");
                            System.IO.File.Copy(sourceFile, targetFile, true);
                            session.Log($"Copied settings.json to: {targetFile}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                session.Log($"Error copying settings.json: {ex}");
                //return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult clearCurrentActiveBallerinaVersion(Session session)
        {
            session.Log("Begin clearCurrentActiveBallerinaVersion");

            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_UserProfile WHERE Special = False");

                using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
                {
                    foreach (ManagementObject profile in searcher.Get())
                    {
                        string localPath = (string)profile["LocalPath"];
                        if (string.IsNullOrEmpty(localPath)) continue;

                        string username = localPath.Substring(localPath.LastIndexOf('\\') + 1);
                        var user = UserPrincipal.FindByIdentity(context, username);

                        if (user == null || user.Enabled == false)
                        {
                            continue;
                        }

                        session.Log($"User directory: {localPath}");
                        string ballerinaUserHome = System.IO.Path.Combine(localPath, ".ballerina");
                        if (System.IO.Directory.Exists(ballerinaUserHome))
                        {
                            session.Log($"Ballerina user home directory found: {ballerinaUserHome}");
                            string ballerinaVersionFile = System.IO.Path.Combine(ballerinaUserHome, "ballerina-version");
                            if (System.IO.File.Exists(ballerinaVersionFile))
                            {
                                System.IO.File.Delete(ballerinaVersionFile);
                                session.Log($"Deleted ballerina-version file: {ballerinaVersionFile}");
                            }
                            else
                            {
                                session.Log($"ballerina-version file not found: {ballerinaVersionFile}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                session.Log($"Error copying settings.json: {ex}");
                //return ActionResult.Failure;
            }

            return ActionResult.Success;
        }



    }
}
