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
    public class JoinSound : BasePlugin, IPluginConfig<JoinSoundConfig>
    {
        public override string ModuleName => "CS_SoundPlugin";
        public override string ModuleVersion => "0.0.2";
        public override string ModuleAuthor => "SmirkinGerman";
        public JoinSoundConfig Config { get; set; } = new JoinSoundConfig();
        public void OnConfigParsed(JoinSoundConfig config)
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

        private void OnClientConnected(int playerSlot)
        {
            var player = Utilities.GetPlayerFromSlot(playerSlot);
            if (player?.IsValid == true && !player.IsBot)
            {
                AddTimer(1.0f, () => PlaySoundUtill.PlaySound(player, Config));
            }
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
                PlaySoundUtill.PlayKnifeKillSound(Config);
            }

            return HookResult.Continue;
        }

        [ConsoleCommand("test", "Test join sound")]
        public void PlayJoinCommand(CCSPlayerController? controller, CommandInfo info)
        {
            if (controller == null || !controller.IsValid || controller.IsBot)
            {
                info.ReplyToCommand("Command must be run by a valid player.");
                return;
            }
            PlaySoundUtill.PlaySound(controller, Config);
        }
    }

    public class SoundItem
    {
        public string Path { get; set; } = string.Empty;
        public string Knife { get; set; } = string.Empty;
    }
}