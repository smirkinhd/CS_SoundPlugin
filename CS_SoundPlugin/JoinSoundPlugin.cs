using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CS_SoundPlugin
{
    public class JoinSound : BasePlugin, IPluginConfig<PluginConfig>
    {
        public override string ModuleName => "CS_SoundPlugin";
        public override string ModuleVersion => "0.0.1";
        public override string ModuleAuthor => "SmirkinGerman";

        public PluginConfig Config { get; set; } = new PluginConfig();
        private Dictionary<ulong, int> playerSoundIndex = new Dictionary<ulong, int>();

        public void OnConfigParsed(PluginConfig config)
        {
            Config = config;
            Server.PrintToConsole("JoinSound configuration loaded.");
        }

        public override void Load(bool hotReload)
        {
            base.Load(hotReload);
            RegisterListener<Listeners.OnClientConnected>(OnClientConnected);
        }

        private void OnClientConnected(int playerSlot)
        {
            var player = Utilities.GetPlayerFromSlot(playerSlot);
            if (player?.IsValid == true && !player.IsBot)
            {
                AddTimer(1.0f, () => PlaySound(player));
            }
        }

        private void PlaySound(CCSPlayerController player)
        {
            try
            {
                if (!player.IsValid || player.IsBot)
                    return;

                string soundPath = GetSoundPath(player);
                if (!string.IsNullOrEmpty(soundPath))
                {
                    player.ExecuteClientCommand($"play {soundPath}");
                    Server.PrintToConsole($"Played sound {soundPath} for {player.PlayerName}");
                }
                else
                {
                    Server.PrintToConsole("No sound available for playback.");
                }
            }
            catch (Exception ex)
            {
                Server.PrintToConsole($"Error playing sound for {player.PlayerName}: {ex.Message}");
            }
        }

        private string GetSoundPath(CCSPlayerController player)
        {
            if (Config.MusicList.Count == 0)
                return string.Empty;

            switch (Config.SoundMode.ToLower())
            {
                case "random":
                    return Config.MusicList[new Random().Next(Config.MusicList.Count)].Path;

                case "order":
                default:
                    var steamId = player.SteamID;
                    if (!playerSoundIndex.ContainsKey(steamId))
                    {
                        playerSoundIndex[steamId] = 0;
                    }

                    int index = playerSoundIndex[steamId];
                    string path = Config.MusicList[index].Path;
                    playerSoundIndex[steamId] = (index + 1) % Config.MusicList.Count;
                    return path;
            }
        }

        [ConsoleCommand("test", "Test join sound")]
        public void PlayJoinCommand(CCSPlayerController? controller, CommandInfo info)
        {
            if (controller == null || !controller.IsValid || controller.IsBot)
            {
                info.ReplyToCommand("Command must be run by a valid player.");
                return;
            }
            PlaySound(controller);
        }
    }

    public class PluginConfig : BasePluginConfig
    {
        public string SoundMode { get; set; } = "order";
        public List<SoundItem> MusicList { get; set; } = new List<SoundItem>
        {
            new SoundItem { Path = "zvyki/join_sound.vsnd" },
        };
    }

    public class SoundItem
    {
        public string Path { get; set; } = string.Empty;
    }
}