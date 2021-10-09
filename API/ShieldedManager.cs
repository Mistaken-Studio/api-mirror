using Exiled.API.Features;
using MEC;
using Mistaken.API.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Mistaken.API.Shield
{
    public class ShieldedManager : Module
    {
        public override bool IsBasic => true;

        public static readonly Dictionary<Player, Shielded> Shieldeds = new Dictionary<Player, Shielded>();

        public override string Name => "ShieldManager";

        internal ShieldedManager(PluginHandler p) : base(p)
        {
            this.Server_RestartingRound();
        }

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += this.Handle(() => this.Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Server.RestartingRound += this.Handle(() => this.Server_RestartingRound(), "RoundRestart");
        }

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= this.Handle(() => this.Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Server.RestartingRound -= this.Handle(() => this.Server_RestartingRound(), "RoundRestart");

            foreach (var item in Shieldeds.Values.ToArray())
                item.Disable();
        }

        public static void Add(Shielded shielded)
        {
            Remove(shielded.player);
            Shieldeds[shielded.player] = shielded;
        }

        public static void Remove(Player player)
        {
            if(Shieldeds.ContainsKey(player))
            {
                Shieldeds[player].Disable();
                Shieldeds.Remove(player);
            }
        }

        public static bool Has(Player player) => Shieldeds.ContainsKey(player);

        public static Shielded Get(Player player) => Shieldeds.TryGetValue(player, out var value) ? value : null;

        public static bool TryGet(Player player, out Shielded result)
        {
            return Shieldeds.TryGetValue(player, out result);
        }

        private void Server_RoundStarted()
        {
            this.RunCoroutine(ExecuteCycle(), "ExecuteCycle");
        }

        private void Server_RestartingRound()
        {
            foreach (var item in Shieldeds.Values.ToArray())
                item.Disable();
            Shieldeds.Clear();
        }

        private static IEnumerator<float> ExecuteCycle()
        {
            yield return Timing.WaitForSeconds(1);
            int rid = RoundPlus.RoundId;
            while (Round.IsStarted && rid == RoundPlus.RoundId)
            {
                foreach (var shielded in Shieldeds.Values.ToArray())
                    shielded.DoRegenerationCicle();
                yield return Timing.WaitForSeconds(1);
            }
        }
    }

    public class Shielded
    {
        public readonly Player player;

        public ushort MaxShield
        {
            get
            {
                return this._maxShield;
            }

            set
            {
                if (this.player != null)
                    this.player.MaxArtificialHealth += (value - this._maxShield);
                this._maxShield = value;
            }
        }

        private ushort _maxShield = 0;

        public ushort Regeneration { get; set; }

        public float SafeTime { get; }

        public float ShieldEffectivnes { get; }

        private readonly float originalShieldEffectivnes;

        public float ShieldDecay { get; set; }

        private readonly float originalShieldDecay;

        private readonly Timer safeTimer;
        private bool _isSafe = true;

        public bool IsSafe
        {
            get
            {
                return this._isSafe;
            }

            private set
            {
                if (value)
                    this._isSafe = true;
                else
                {
                    this.safeTimer.Start();
                    this._isSafe = false;
                }
            }
        }

        public Shielded(Player p, ushort maxShield, ushort regeneration, float safeTime = -1, float shieldDecay = -1, float shieldEffectivnes = -1)
        {
            Log.Debug($"Enabling shield for {p.Nickname}");
            this.MaxShield = maxShield;
            this.Regeneration = regeneration;
            this.SafeTime = safeTime;
            this.ShieldDecay = shieldDecay;
            this.ShieldEffectivnes = shieldEffectivnes;
            this.player = p;

            this.safeTimer = new Timer(this.SafeTime * 1000);
            this.safeTimer.Elapsed += (_, __) =>
            {
                this.IsSafe = true;
                this.safeTimer.Stop();
            };

            Exiled.Events.Handlers.Player.Left += this.Player_Left;
            Exiled.Events.Handlers.Player.ChangingRole += this.Player_ChangingRole;
            Exiled.Events.Handlers.Player.Hurting += this.Player_Hurting;

            var ps = p.ReferenceHub.playerStats;
            this.originalShieldDecay = ps.ArtificialHpDecay;
            this.originalShieldEffectivnes = ps.ArtificialNormalRatio;
            if (this.ShieldDecay != -1)
                ps.ArtificialHpDecay = this.ShieldDecay;
            if (this.ShieldEffectivnes != -1)
                ps.ArtificialNormalRatio = this.ShieldEffectivnes;
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            if (ev.Player.Id == this.player.Id)
                this.Disable();
        }

        private void Player_Left(Exiled.Events.EventArgs.LeftEventArgs ev)
        {
            if (ev.Player.Id == this.player.Id)
                this.Disable();
        }

        private void Player_Hurting(Exiled.Events.EventArgs.HurtingEventArgs ev)
        {
            if (ev.Target.Id == this.player.Id)
                this.IsSafe = false;
        }

        internal void DoRegenerationCicle()
        {
            if (this.player.ArtificialHealth > this.MaxShield)
            {
                if (this.ShieldDecay != -1)
                    this.player.ReferenceHub.playerStats.ArtificialHpDecay = this.originalShieldDecay;
                return;
            }
            else
            {
                if (this.ShieldDecay != -1)
                    this.player.ReferenceHub.playerStats.ArtificialHpDecay = this.ShieldDecay;
            }

            if (this.player.ArtificialHealth == this.MaxShield)
                return;

            if (this.SafeTime == -1 || !this.IsSafe)
                return;
            this.player.ArtificialHealth += this.Regeneration;
            if (this.player.ArtificialHealth > this.MaxShield)
                this.player.ArtificialHealth = this.MaxShield;
        }

        internal void Disable()
        {
            Log.Debug($"Disabling shield for {this.player.Nickname}");
            Exiled.Events.Handlers.Player.Left -= this.Player_Left;
            Exiled.Events.Handlers.Player.ChangingRole -= this.Player_ChangingRole;
            Exiled.Events.Handlers.Player.Hurting -= this.Player_Hurting;

            this.player.MaxArtificialHealth = 75;
            this.player.ArtificialHealth = 0;
            var ps = this.player.ReferenceHub.playerStats;
            if (this.ShieldEffectivnes != -1)
                ps.ArtificialNormalRatio = this.originalShieldEffectivnes;
            if (this.ShieldDecay != -1)
                ps.ArtificialHpDecay = this.originalShieldDecay;

            ShieldedManager.Shieldeds.Remove(this.player);
        }
    }
}
