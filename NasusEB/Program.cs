using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace Nasus
{
    internal class Program
    {
        private const string ChampionName = "Nasus";

        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Menu Config;

        private static Spell.Active Q;
        private static Spell.Targeted W;
        private static Spell.Skillshot E;
        private static Spell.Active R;

        public static Menu Menu,
            comboMenu,
            harassMenu,
            drawMenu,
            laneClearMenu,
            lastHitMenu;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Chat.Print("NentoR's Nasus Loaded! Enjoy :)", Color.Gold);

            if (Player.ChampionName != ChampionName)
                return;

            Q = new Spell.Active(SpellSlot.Q, (uint)((float)ObjectManager.Player.GetAutoAttackRange()));
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Skillshot(SpellSlot.E, 650, EloBuddy.SDK.Enumerations.SkillShotType.Circular);
            R = new Spell.Active(SpellSlot.R);

            // Main Menu
            Config = MainMenu.AddMenu("NentoR's Nasus", "Nasus");
            Menu.AddGroupLabel("Made by NentoR");

            // Combo Menu
            /* var comboMenu = new Menu("Combo", "Combo");
             comboMenu.AddItem(new MenuItem("ComboQ", "Use Q").SetValue(true));
             comboMenu.AddItem(new MenuItem("ComboW", "Use W").SetValue(true));
             comboMenu.AddItem(new MenuItem("ComboE", "Use E").SetValue(true));
             comboMenu.AddItem(new MenuItem("ComboR", "Use R (Misc | Check)").SetValue(true).SetTooltip("Use R as Defensive Spell when HP Percent < Slider below (Smart)"));
             comboMenu.AddItem(new MenuItem("RHP", "Use R if % HP").SetValue(new Slider(25)));
             Config.AddSubMenu(comboMenu); */

            // Combo Menu
            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.AddGroupLabel("Combo");
            comboMenu.Add("ComboQ", new CheckBox("Use Q"));
            comboMenu.Add("ComboW", new CheckBox("Use W"));
            comboMenu.Add("ComboE", new CheckBox("Use E"));
            comboMenu.AddLabel("Ult Settings");
            comboMenu.Add("ComboR", new CheckBox("Use R (Check)"));
            comboMenu.Add("RHP", new Slider("Use R if % HP", 30, 0, 100));

            // Lane Clear Menu
            /*  var laneClearMenu = new Menu("Lane Clear", "laneclear");
              laneClearMenu.AddItem(new MenuItem("LCQ", "Use Q to stack").SetValue(true));
              laneClearMenu.AddItem(new MenuItem("LCE", "Use E").SetValue(false));
              Config.AddSubMenu(laneClearMenu); */

            // Lane Clear Menu
            laneClearMenu = Menu.AddSubMenu("LaneClear", "LaneClear");
            laneClearMenu.AddGroupLabel("LaneClear");
            laneClearMenu.Add("LCQ", new CheckBox("Use Q to stack"));
            laneClearMenu.Add("LCE", new CheckBox("Use E"));

            // LastHit Menu
            /* var lastHitMenu = new Menu("LastHit", "lasthit");
             lastHitMenu.AddItem(new MenuItem("LHQ", "Use Q to stack").SetValue(true));
             Config.AddSubMenu(lastHitMenu); */

            // LastHit Menu
            lastHitMenu = Menu.AddSubMenu("LasHit", "LastHit");
            lastHitMenu.AddGroupLabel("LastHit");
            lastHitMenu.Add("LHQ", new CheckBox("LastHitting(Stacking) with Q"));

            // Harass Menu
            /* var harassMenu = new Menu("Harass", "Harass");
             harassMenu.AddItem(new MenuItem("HQ", "Use Q to Harass").SetValue(true));
             harassMenu.AddItem(new MenuItem("HW", "Use W to Harass").SetValue(true));
             harassMenu.AddItem(new MenuItem("HE", "Use E to Harass").SetValue(true));
             Config.AddSubMenu(harassMenu); */

            // Harass Menu
            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("HQ", new CheckBox("Use Q to Harass"));
            harassMenu.Add("HW", new CheckBox("Use W to Harass"));
            harassMenu.Add("HE", new CheckBox("Use E to Harass"));

            // Drawings Menu
            /* var drawMenu = new Menu("Drawings", "Drawings");
             drawMenu.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(true));
             drawMenu.AddItem(new MenuItem("DW", "Draw W Range").SetValue(true));
             drawMenu.AddItem(new MenuItem("DE", "Draw E Range").SetValue(true));
             Config.AddSubMenu(drawMenu); */

            // Drawings Menu
            drawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DQ", new CheckBox("Draw Q Range"));
            drawMenu.Add("DW", new CheckBox("Draw W Range"));
            drawMenu.Add("DE", new CheckBox("Draw E Range"));

            // Credits to this boy ;) 
            /* Config.AddItem(new MenuItem("Credits", "                .:Credits:.").SetFontStyle(FontStyle.Bold, SharpDX.Color.Chartreuse));
              Config.AddItem(new MenuItem("CreditsBoy", "                SupportExTraGoZ").SetFontStyle(FontStyle.Bold, SharpDX.Color.DeepPink));
              Config.AddItem(new MenuItem("CreditsBoy2", "                Screeder").SetFontStyle(FontStyle.Bold, SharpDX.Color.Purple));

      Special thanks to you guys teached me a lot you will miss me from L# ;( */

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            // Accurate Draw AutoAttack
            if (drawMenu["DQ"].Cast<CheckBox>().CurrentValue)
            {
                new Circle { Color = Color.Chartreuse, Radius = Q.Range, BorderWidth = 2F }.Draw(Player.Position);
            }
            if (drawMenu["DW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle { Color = Color.DeepPink, Radius = W.Range, BorderWidth = 2F }.Draw(Player.Position);
            }
            // Render.Circle.DrawCircle(Player.Position, W.Range, Color.DeepPink);
            if (drawMenu["DE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle { Color = Color.DeepSkyBlue, Radius = E.Range, BorderWidth = 2F }.Draw(Player.Position);
            }
            // Render.Circle.DrawCircle(Player.Position, E.Range, Color.DeepSkyBlue);
        }


        private static void Combo()
        {
            // var ComboQ = Config.Item("ComboQ").GetValue<bool>();
            var ComboQ = comboMenu["ComboQ"].Cast<CheckBox>().CurrentValue;
            // var ComboW = Config.Item("ComboW").GetValue<bool>();
            var ComboW = comboMenu["ComboW"].Cast<CheckBox>().CurrentValue;
            // var ComboE = Config.Item("ComboE").GetValue<bool>();
            var ComboE = comboMenu["ComboE"].Cast<CheckBox>().CurrentValue;
            // var RHP = Config.Item("RHP").GetValue<Slider>().Value;
            var ComboR = comboMenu["ComboR"].Cast<CheckBox>().CurrentValue;
            var RHP = comboMenu["RHP"].Cast<Slider>().CurrentValue;


            var TargetEE = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var target = TargetSelector.GetTarget(Player.AttackRange, DamageType.Physical);

            if (!target.IsValidTarget() || target == null)
                return;

            if (target.IsValidTarget(Q.Range) && ComboQ && Q.IsReady())
                Q.Cast();
            // Orbwalker.ForceTarget(TargetEE);

            if (target.IsValidTarget(W.Range) && ComboW && W.IsReady())
                W.Cast(target);

            if (target.IsValidTarget(E.Range) && ComboE && E.IsReady())
            {
                var predE = (E.GetPrediction(target));
                if (predE.HitChancePercent >= 75)
                {
                    E.Cast(predE.CastPosition);
                }
            }

        }

        private static void RCheck()
        {
            // if (Config.Item("ComboR").GetValue<bool>() && R.IsReady() && Player.HealthPercent < Config.Item("RHP").GetValue<Slider>().Value && Player.CountEnemiesInRange(W.Range) > 0)
            if (comboMenu["ComboR"].Cast<CheckBox>().CurrentValue && R.IsReady() && Player.Instance.HealthPercent < comboMenu["RHP"].Cast<Slider>().CurrentValue && Player.Instance.CountEenemiesInRange(W.Range) > 0)
            {
                R.Cast();
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            /*  switch (Orbwalker.ActiveMode)
              {
                  case Orbwalking.OrbwalkingMode.Combo:
                      Combo();
                      break;

                  case Orbwalking.OrbwalkingMode.LaneClear:
                      LaneClear();
                      break;

                  case Orbwalking.OrbwalkingMode.Mixed:
                      Harass();
                      break;

                  case Orbwalking.OrbwalkingMode.LastHit:
                      LastHit();
                      break;
              }
              RCheck();
          }

          private static void LaneClear()
          {
              var StackQ = MinionManager.GetMinions(Player.Position, Q.Range + 100);
              var MQ = Config.Item("LCQ").GetValue<bool>();
              var ME = Config.Item("LCE").GetValue<bool>();
              var MECast = MinionManager.GetMinions(E.Range + E.Width);
              var ELoc = E.GetCircularFarmLocation(MECast, E.Range);

              if (MQ)
              {
                  foreach (var minion in StackQ)
                  {
                      if (minion.Health <= Q.GetDamage(minion) && Q.IsReady())
                      {
                          Q.Cast();
                          Orbwalker.ForceTarget(minion);
                      }
                  }
              }
              if (ME)
              {
                  foreach (var minion in MECast)
                  {
                      if (E.IsInRange(minion))
                      {
                          E.Cast(ELoc.Position);

                      }
                  }
              }
          }

          private static void Harass()
          {
              var HarassQ = Config.Item("HQ").GetValue<bool>();
              var HarassW = Config.Item("HW").GetValue<bool>();
              var HarassE = Config.Item("HE").GetValue<bool>();

              var Qtarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
              var Wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
              var Etarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

              if (HarassQ)
              {
                  if (Qtarget.IsValidTarget(Q.Range) && Q.IsReady())
                  {
                      Q.Cast();
                  }
              }

              if (HarassW)
              {
                  if (Wtarget.IsValidTarget(W.Range) && W.IsReady())
                  {
                      W.Cast();
                  }

              }

              if (HarassE)
              {
                  if (Etarget.IsValidTarget(E.Range) && E.IsReady())
                  {
                      E.CastIfHitchanceEquals(Etarget, HitChance.VeryHigh);
                  }
              }
          }

          private static void LastHit()
          {
              var LastHitQ = Config.Item("LHQ").GetValue<bool>();
              var minionQ = MinionManager.GetMinions(Player.Position, Q.Range + 100);

              if (LastHitQ)
              {
                  foreach (var minion in minionQ)
                  {
                      if (minion.Health <= Q.GetDamage(minion) && Q.IsReady())
                      {
                          Q.Cast();
                          Orbwalker.ForceTarget(minion);
                      }
                  }
              }
          } */
        }
    }
}