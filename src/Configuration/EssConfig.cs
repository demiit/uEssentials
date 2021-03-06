/*
 *  This file is part of uEssentials project.
 *      https://uessentials.github.io/
 *
 *  Copyright (C) 2015-2016  Leonardosc
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along
 *  with this program; if not, write to the Free Software Foundation, Inc.,
 *  51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Essentials.Api;
using Essentials.Api.Configuration;
using Essentials.Common;
using Essentials.Misc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Essentials.Configuration {

    public class EssConfig : JsonConfig {

        public string Locale;
        public string PMFormatFrom;
        public string PMFormatTo;

        public bool UnfreezeOnDeath;
        public bool UnfreezeOnQuit;

        public bool EnableTextCommands;
        public bool EnableDeathMessages;
        public bool EnableJoinLeaveMessage;
        public bool ShowPermissionOnErrorMessage;

        public bool EnablePollRunningMessage;
        public int PollRunningMessageCooldown;
        public int ServerFrameRate;

        public ushort ItemSpawnLimit;

        public AntiSpamSettings AntiSpam;
        public UpdaterSettings Updater;
        public HomeCommandSettings HomeCommand;
        public WarpCommandSettings WarpCommand;
        public VehicleFeaturesSettings VehicleFeatures;
        public WebKitsSettings WebKits;
        public WebConfigSettings WebConfig;
        public KitSettings Kit;
        public TpaSettings Tpa;
        public EconomySettings Economy;

        public AutoAnnouncer AutoAnnouncer;
        public AutoCommands AutoCommands;

        public List<ushort> GiveItemBlacklist;
        public List<ushort> VehicleBlacklist;
        public List<string> EnabledSystems;
        public List<string> DisabledCommands;

        internal EssConfig() {}

        public override void LoadDefaults() {
            Locale = "en";

            PMFormatFrom = "(From {0}): {1}";
            PMFormatTo = "(To {0}): {1}";

            UnfreezeOnDeath = true;
            UnfreezeOnQuit = true;

            EnableJoinLeaveMessage = true;
            EnableTextCommands = true;
            EnableDeathMessages = true;
            ShowPermissionOnErrorMessage = true;

            EnablePollRunningMessage = true;
            PollRunningMessageCooldown = 20;
            ServerFrameRate = -1; // http://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html

            AutoAnnouncer = new AutoAnnouncer();
            AutoAnnouncer.LoadDefaults();

            AutoCommands = new AutoCommands();
            AutoCommands.LoadDefaults();

            AntiSpam = new AntiSpamSettings {
                Enabled = true,
                Interval = 3
            };

            Updater = new UpdaterSettings {
                CheckUpdates = true,
                DownloadLatest = true,
                AlertOnJoin = true
            };

            HomeCommand = new HomeCommandSettings {
                Cooldown = 30,
                Delay = 5,
                CancelTeleportWhenMove = true
            };

            WarpCommand = new WarpCommandSettings {
                Cooldown = 5,
                PerWarpPermission = true,
                CancelTeleportWhenMove = false
            };

            WebKits = new WebKitsSettings {
                Enabled = false,
                Url = ""
            };

            WebConfig = new WebConfigSettings {
                Enabled = false,
                Url = ""
            };

            Kit = new KitSettings {
                ShowCost = true,
                ShowCostIfZero = false,
                CostFormat = "{0}({1}{2})",
                GlobalCooldown = 0,
                ResetGlobalCooldownWhenDie = false
            };

            Tpa = new TpaSettings {
                ExpireDelay = 10,
                TeleportDelay = 5
            };

            Economy = new EconomySettings {
                UseXp = false,
                UconomyCurrency = "$",
                XpCurrency = "Xp"
            };

            VehicleFeatures = new VehicleFeaturesSettings {
                RefuelPercentage = 20,
                RepairPercentage = 70
            };

            GiveItemBlacklist = new List<ushort>();
            VehicleBlacklist = new List<ushort>();
            DisabledCommands = new List<string>();
            EnabledSystems = new List<string> { "kits", "warps" };

            ItemSpawnLimit = 10;
        }

        public override void Load(string filePath) {
            if (!File.Exists(filePath)) {
                base.Load(filePath);
            }

            try {
                var json = JObject.Parse(File.ReadAllText(filePath));
                base.Load(filePath);

                /*
                    Add missing fields...
                */
                var configFiels = GetType().GetFields();
                var needUpdate = configFiels.Length != json.Count;
                var nonNullFields = new Dictionary<string, object>();

                if (needUpdate) {
                    configFiels.Where(f => json[f.Name] != null).ForEach(f => {
                        nonNullFields.Add(f.Name, f.GetValue(this));
                    });

                    LoadDefaults();

                    nonNullFields.ForEach(pair => {
                        GetType().GetField(pair.Key).SetValue(this, pair.Value);
                    });

                    Save(filePath);
                }

                /*
                   Validation
                */
                if (VehicleFeatures.RefuelPercentage <= 0) {
                    UEssentials.Logger.LogError("Invalid config: VehicleFeatures.RefuelPercentage " +
                                                $"must be positive. (Got {VehicleFeatures.RefuelPercentage})");
                }

                if (VehicleFeatures.RepairPercentage <= 0) {
                    UEssentials.Logger.LogError("Invalid config: VehicleFeatures.RepairPercentage " +
                                                $"must be positive. (Got {VehicleFeatures.RepairPercentage})");
                }

                /*
                    Update old configs
                */
                if (json["HomeCommand"]["CancelTeleportWhenMove"] == null &&
                    json["HomeCommand"]["CancelWhenMove"] != null) {
                    HomeCommand.CancelTeleportWhenMove = (bool) json["HomeCommand"]["CancelWhenMove"];
                    Save(filePath);
                }
            } catch (Exception ex) {
                UEssentials.Logger.LogError($"Invalid configuration ({filePath})");
                UEssentials.Logger.LogError(ex.Message);
                UEssentials.Logger.LogError("Using default configuration...");
                LoadDefaults();
            }
        }

        [JsonObject]
        public struct AntiSpamSettings {

            public bool Enabled;
            public int Interval;

        }

        [JsonObject]
        public struct UpdaterSettings {

            public bool CheckUpdates;
            public bool DownloadLatest;
            public bool AlertOnJoin;

        }

        [JsonObject]
        public struct HomeCommandSettings {

            public int Cooldown;
            public int Delay;
            public bool CancelTeleportWhenMove;

        }

        [JsonObject]
        public struct WarpCommandSettings {

            public int Cooldown;
            public bool CancelTeleportWhenMove;
            public bool PerWarpPermission;

        }

        [JsonObject]
        public struct WebKitsSettings {

            public string Url;
            public bool Enabled;

        }

        [JsonObject]
        public struct WebConfigSettings {

            public string Url;
            public bool Enabled;

        }

        [JsonObject]
        public struct KitSettings {

            public bool ShowCost;
            public bool ShowCostIfZero;
            public string CostFormat;
            public uint GlobalCooldown;
            public bool ResetGlobalCooldownWhenDie;

        }

        [JsonObject]
        public struct TpaSettings {

            public int ExpireDelay;
            public int TeleportDelay;

        }

        [JsonObject]
        public struct EconomySettings {

            public bool UseXp;
            public string XpCurrency;
            public string UconomyCurrency;

        }

        [JsonObject]
        public struct VehicleFeaturesSettings {

            public int RefuelPercentage;
            public int RepairPercentage;

        }

    }

}