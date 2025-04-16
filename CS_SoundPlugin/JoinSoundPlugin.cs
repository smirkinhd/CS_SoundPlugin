using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CS_SoundPlugin
{
    public class JoinSound : BasePlugin, IPluginConfig<PluginConfig>
    {
        public override string ModuleName => "CS_SoundPlugin";
        public override string ModuleVersion => "0.0.2";
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
            RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        }

        private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
        {
            var attacker = @event.Attacker;
            var victim = @event.Userid;
            var weapon = @event.Weapon;

            if (attacker == null || victim == null || attacker.IsBot || victim.IsBot)
                return HookResult.Continue;

            if (weapon == "knife")
            {
                PlayKnifeKillSound();
            }

            return HookResult.Continue;
        }

        private void PlayKnifeKillSound()
        {
            try
            {
                string knifeSoundPath = Config.MusicList.FirstOrDefault()?.Knife ?? string.Empty;
                if (string.IsNullOrEmpty(knifeSoundPath))
                {
                    Server.PrintToConsole("No knife kill sound configured.");
                    return;
                }

                var players = Utilities.GetPlayers()
                    .Where(p => p.IsValid && !p.IsBot)
                    .ToList();

                foreach (var player in players)
                {
                    player.ExecuteClientCommand($"play {knifeSoundPath}");
                }

                Server.PrintToConsole($"Played knife kill sound {knifeSoundPath} for all players.");
            }
            catch (Exception ex)
            {
                Server.PrintToConsole($"Error playing knife kill sound: {ex.Message}");
            }
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
            new SoundItem
            {
                Path = "zvuki/join_sound.vsnd_c",
                Knife = "zvuki/kill_knife.vsnd_c"
            }
        };
    }

    public class SoundItem
    {
        public string Path { get; set; } = string.Empty;
        public string Knife { get; set; } = string.Empty;
    }
}