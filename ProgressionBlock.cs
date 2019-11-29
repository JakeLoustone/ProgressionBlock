using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Progression Block", "Jake Loustone", "0.0.1")]
    [Description("Blocks researching higher tier blueprints.")]
    public class ProgressionBlock : RustPlugin
    {
        private List<string> workbench1 = new List<string>();
        private List<string> workbench2 = new List<string>();
        private List<string> workbench3 = new List<string>();

        const string PBAdmin = "progressionblock.admin";

        private void Init()
        {
            CheckBlueprints();
        }

        private void CheckBlueprints()
        {
            foreach (var bp in ItemManager.bpList)
            {
                if (bp.userCraftable && bp.defaultBlueprint == false)
                {
                    var shortname = bp.targetItem.shortname;

                    switch (bp.workbenchLevelRequired)
                    {
                        case 1:
                            workbench1.Add(shortname);
                            break;

                        case 2:
                            workbench2.Add(shortname);
                            break;

                        case 3:
                            workbench3.Add(shortname);
                            break;
                    }
                }
            }
        }

        //config.serverTierLevel
        #region Commands

        [ChatCommand("setservertier")]
        void SetServerTier(BasePlayer player, string command, string[] arguments)
        {
            if (permission.UserHasPermission(player.UserIDString, RLAdmin))
            {
                int tempInt = 1;

                int.TryParse(arguments[0], out tempInt);

                if (tempInt < 1 || tempInt > 3)
                {
                    tempInt = 1;
                }

                config.serverTierLevel = tempInt;

                SaveConfig();

                SendReply(player, "Server Tier Level set!");
            }
        }

        [ConsoleCommand("setservertier")]
        private void SetServerTier(ConsoleSystem.Arg arg)
        {
            if (arg.IsAdmin != true) { return; }

            int tempInt = 1;

            int.TryParse(arguments[0], out tempInt);

            if (tempInt < 1 || tempInt > 3)
            {
                tempInt = 1;
            }

            config.serverTierLevel = tempInt;

            SaveConfig();

            arg.Reply = "Server Tier Level set!";
        }

        #endregion

        #region Oxide Hooks

        private object CanResearchItem(BasePlayer player, Item item)
        {
            switch (config.serverTierLevel)
            {
                case 1:
                    if (workbench1.Contains(item.info.shortname))
                    {
                        return (object)null;
                    }
                    break;

                case 2:
                    if (workbench1.Contains(item.info.shortname) || workbench2.Contains(item.info.shortname))
                    {
                        return (object)null;
                    }
                    break;

                case 3:
                    if (workbench1.Contains(item.info.shortname) || workbench2.Contains(item.info.shortname) || workbench3.Contains(item.info.shortname))
                    {
                        return (object)null;
                    }
                    break;
            }

            return false;
        }

        #endregion

        #region Config

        private static ConfigData config;

        private class ConfigData
        {
            [JsonProperty(PropertyName = "Current Server Tier Level")]
            public int serverTierLevel;
        }

        private ConfigData GetDefaultConfig()
        {
            return new ConfigData
            {
                serverTierLevel = 1
            };
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();

            try
            {
                config = Config.ReadObject<ConfigData>();
            }
            catch
            {
                LoadDefaultConfig();
            }

            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            PrintError("Configuration file is corrupt(or not exists), creating new one!");
            config = GetDefaultConfig();
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(config);
        }
    }
}

#endregion