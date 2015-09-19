using System;
using LeagueSharp;
using System.Drawing;
using LeagueSharp.Common;
namespace FuckingRengarReborn
{
public class Program
{
#region
private static Menu _config;
private static int _lastTick;
private static Orbwalking.Orbwalker _orbwalker;
private static Spell _q = new Spell(SpellSlot.Q, 240);
private static Spell _w = new Spell(SpellSlot.W, 350);
private static Spell _e = new Spell(SpellSlot.E, 950);
private static SpellSlot _smite = ObjectManager.Player.GetSpellSlot("summonersmite");
private static SpellSlot _smitee = ObjectManager.Player.GetSpellSlot("s5_summonersmiteduel");
private static SpellSlot _smiteee = ObjectManager.Player.GetSpellSlot("s5_summonersmiteplayerganker");
#endregion
#region
private static void Main(string[] args)
{
    CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
}
#endregion
#region
private static void Game_OnGameLoad(EventArgs args)
{
    if (ObjectManager.Player.ChampionName != "Rengar")
    {
        return;
    }
    _e.SetSkillshot(0.25f, 90, 1500, true, SkillshotType.SkillshotLine);
    _config = new Menu("FuckingRengarReborn", "FuckingRengarReborn", true);
    _orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));
    var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
    TargetSelector.AddToMenu(targetSelectorMenu);
    _config.AddSubMenu(targetSelectorMenu);
    _config.SubMenu("FuckingRengarReborn").SubMenu("Combo Mode").SubMenu("Switch Key").AddItem(new MenuItem("cs", "Combo switch Key").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
    _config.SubMenu("FuckingRengarReborn").SubMenu("Combo Mode").AddItem(new MenuItem("em", "E Mode").SetValue(false));
    _config.SubMenu("FuckingRengarReborn").SubMenu("Orb Mode").SubMenu("Switch Key").AddItem(new MenuItem("os", "Orb switch Key").SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Press)));
    _config.SubMenu("FuckingRengarReborn").SubMenu("Orb Mode").AddItem(new MenuItem("om", "Oneshot Mode").SetValue(false));
    _config.SubMenu("FuckingRengarReborn").SubMenu("Combo").AddItem(new MenuItem("eq", "E in Q Mode").SetValue(true));
    _config.SubMenu("FuckingRengarReborn").SubMenu("Combo").AddItem(new MenuItem("aq", "Use WE after Q").SetValue(true));
    _config.SubMenu("FuckingRengarReborn").SubMenu("Combo").AddItem(new MenuItem("wkb", "W 5 ferocity if killable").SetValue(true));
    _config.SubMenu("FuckingRengarReborn").SubMenu("AutoHeal").AddItem(new MenuItem("ah", "Auto Heal").SetValue(new Slider(33, 100, 0)));
    _config.SubMenu("FuckingRengarReborn").SubMenu("LaneClear").AddItem(new MenuItem("ok", "Spells on killable minions").SetValue(true));
    _config.SubMenu("FuckingRengarReborn").SubMenu("LaneClear").AddItem(new MenuItem("ql", "Q").SetValue(true));
    _config.SubMenu("FuckingRengarReborn").SubMenu("LaneClear").AddItem(new MenuItem("wl", "W").SetValue(true));
    _config.SubMenu("FuckingRengarReborn").SubMenu("LaneClear").AddItem(new MenuItem("el", "E").SetValue(true));
    _config.SubMenu("FuckingRengarReborn").SubMenu("Drawings").AddItem(new MenuItem("cd", "Combo Mode Text").SetValue(true));
    _config.SubMenu("FuckingRengarReborn").SubMenu("Drawings").AddItem(new MenuItem("od", "Orb Mode Text").SetValue(true));
    _config.SubMenu("FuckingRengarReborn").SubMenu("Drawings").AddItem(new MenuItem("dt", "Active Enemy").SetValue(new Circle(true, Color.GreenYellow)));
    _config.AddToMainMenu();
    Game.OnUpdate += Game_OnUpdate;
    Obj_AI_Base.OnProcessSpellCast += oncast;
    Drawing.OnDraw += Drawing_OnDraw;
    
    Game.PrintChat("<b><font color=\"#04EECA\">Fucking</font> <font color=\"#DC0DA1\">Rengar</font> <font color=\"#FF0000\">Reborn</font> <font color=\"#FFFFFF\">by</font> <font color=\"#FFEB00\">folxu</font> <font color=\"#00FF2F\">Loaded!</font></b>");
    Game.PrintChat("<b><font color=\"#FFA600\">Working on 5.18</font></b>");
    Game.PrintChat("<b><font color=\"#FF00F3\">GL HF !</font></b>");
}
#endregion
#region
private static void Game_OnUpdate(EventArgs args)
{
    Comboswitch();
    Orbswitch();
    Auto();
    switch (_orbwalker.ActiveMode)
    {
        case Orbwalking.OrbwalkingMode.Combo:
            Combo();
        break;
        case Orbwalking.OrbwalkingMode.LaneClear:
            LaneClear();
        break;
    }
}
#endregion
#region
private static void oncast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
{
    var spell = args.SData;
    if (!sender.IsMe)
    {
        return;
    }
    if (spell.Name.ToLower().Contains("rengarq") || spell.Name.ToLower().Contains("rengare"))
    {
        Orbwalking.ResetAutoAttackTimer();
    }
}
#endregion
#region
private static void Drawing_OnDraw(EventArgs args)
{
    var t = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
    var tt = TargetSelector.GetTarget(350, TargetSelector.DamageType.Physical);
    if (_config.Item("dt").GetValue<Circle>().Active)
    {
        if (tt.IsValidTarget(350))
        {
            Render.Circle.DrawCircle(tt.Position, 115f, _config.Item("dt").GetValue<Circle>().Color, 1);
        }
        else
        {
            if (t.IsValidTarget(1500))
            {
                Render.Circle.DrawCircle(t.Position, 115f, _config.Item("dt").GetValue<Circle>().Color, 1);
            }
        }
    }
    if (_config.Item("cd").GetValue<bool>())
    {
        if (_config.Item("em").GetValue<bool>())
        {
            Drawing.DrawText(Drawing.WorldToScreen(ObjectManager.Player.Position)[0] - 40, Drawing.WorldToScreen(ObjectManager.Player.Position)[1] - 15, Color.White, "Combo Mode: E");
        }
        else
        {
            Drawing.DrawText(Drawing.WorldToScreen(ObjectManager.Player.Position)[0] - 40, Drawing.WorldToScreen(ObjectManager.Player.Position)[1] - 15, Color.White, "Combo Mode: Q");
        }
    }
    if (_config.Item("od").GetValue<bool>())
    {
        if (_config.Item("om").GetValue<bool>())
        {
            Drawing.DrawText(Drawing.WorldToScreen(ObjectManager.Player.Position)[0] - 40, Drawing.WorldToScreen(ObjectManager.Player.Position)[1], Color.White, "Orb Mode: Oneshot");
        }
        else
        {
            Drawing.DrawText(Drawing.WorldToScreen(ObjectManager.Player.Position)[0] - 40, Drawing.WorldToScreen(ObjectManager.Player.Position)[1], Color.White, "Orb Mode: Mixed");
        }
    }
}
#endregion
#region
private static void Auto()
{
    if (ObjectManager.Player.Mana == 5)
    {
        if ((ObjectManager.Player.Health/ObjectManager.Player.MaxHealth)*100 <= _config.Item("ah").GetValue<Slider>().Value)
        {
            if (_w.IsReady())
            {
                _w.Cast();
            }
        }
    }
    if (ObjectManager.Player.HasBuff("rengarpassivebuff"))
    {
        var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
        if (TargetSelector.GetPriority(target) == 2.5f)
        {
            TargetSelector.SetTarget(target);
            if (!_config.Item("ForceFocusSelected").GetValue<bool>())
            {
                _config.Item("ForceFocusSelected").SetValue(true);
            }
        }
    }
    else
    {
        if (_config.Item("ForceFocusSelected").GetValue<bool>())
        {
            _config.Item("ForceFocusSelected").SetValue(false);
        }
    }
}
#endregion
#region
private static void Combo()
{
    var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
    var targett = TargetSelector.GetTarget(350, TargetSelector.DamageType.Physical);
    if (ObjectManager.Player.HasBuff("rengarpassivebuff"))
    {
    }
    else
    {
        if (targett.IsValidTarget(350))
        {
            if (Items.HasItem(3053) && Items.CanUseItem(3053))
            {
                Items.UseItem(3053, targett);
            }
            if (Items.HasItem(3074) && Items.CanUseItem(3074))
            {
                Items.UseItem(3074, targett);
            }
            if (Items.HasItem(3077) && Items.CanUseItem(3077))
            {
                Items.UseItem(3077, targett);
            }
            if (Items.HasItem(3142) && Items.CanUseItem(3142))
            {
                Items.UseItem(3142, targett);
            }
            if (Items.HasItem(3143) && Items.CanUseItem(3143))
            {
                Items.UseItem(3143, targett);
            }
            if (Items.HasItem(3144) && Items.CanUseItem(3144))
            {
                Items.UseItem(3144, targett);
            }
            if (Items.HasItem(3153) && Items.CanUseItem(3153))
            {
                Items.UseItem(3153, targett);
            }
            if (_smite != SpellSlot.Unknown)
            {
                if (ObjectManager.Player.Spellbook.CanUseSpell(_smite) == SpellState.Ready)
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smite, targett);
                }
            }
            if (_smitee != SpellSlot.Unknown)
            {
                if (ObjectManager.Player.Spellbook.CanUseSpell(_smitee) == SpellState.Ready)
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smitee, targett);
                }
            }
            if (_smiteee != SpellSlot.Unknown)
            {
                if (ObjectManager.Player.Spellbook.CanUseSpell(_smiteee) == SpellState.Ready)
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteee, targett);
                }
            }
        }
    }
    if (ObjectManager.Player.Mana < 5)
    {
        if (targett.IsValidTarget(350))
        {
            if (_q.IsReady())
            {
                _q.Cast();
            }
            if (ObjectManager.Player.HasBuff("rengarqbase"))
            {
                Orbwalking.ResetAutoAttackTimer();
            }
            if (_config.Item("om").GetValue<bool>())
            {
                if (targett.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                {
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targett);
                }
            }
            else
            {
                if ((_w.IsReady() || _e.IsReady()) && targett.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                {
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targett);
                }
            }
            if (_config.Item("aq").GetValue<bool>())
            {
                if (!ObjectManager.Player.HasBuff("rengarqbase") && !ObjectManager.Player.HasBuff("rengarqemp"))
                {
                    if (_w.IsReady())
                    {
                        _w.Cast();
                    }
                    if (_e.IsReady())
                    {
                        var EPred = _e.GetPrediction(targett);
                        if (EPred.Hitchance >= HitChance.High)
                        {
                            _e.Cast(EPred.CastPosition);
                        }
                    }
                    if (_config.Item("om").GetValue<bool>())
                    {
                        if (targett.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targett);
                        }
                    }
                    else
                    {
                        if ((_w.IsReady() || _e.IsReady()) && targett.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targett);
                        }
                    }

                }
            }
            else
            {
                if (_w.IsReady())
                {
                    _w.Cast();
                }
                if (_e.IsReady())
                {
                    var EPred = _e.GetPrediction(targett);
                    if (EPred.Hitchance >= HitChance.High)
                    {
                        _e.Cast(EPred.CastPosition);
                    }
                }
                if (_config.Item("om").GetValue<bool>())
                {
                    if (targett.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                    {
                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targett);
                    }
                }
                else
                {
                    if ((_w.IsReady() || _e.IsReady()) && targett.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                    {
                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targett);
                    }
                }
            }
        }
        else
        {
            if (target.IsValidTarget(950))
            {
                if (ObjectManager.Player.HasBuff("rengarpassivebuff"))
                {
                    if (_q.IsReady())
                    {
                        _q.Cast();
                    }
                }
                else
                {
                    if (_e.IsReady())
                    {
                        var EPred = _e.GetPrediction(target);
                        if (EPred.Hitchance >= HitChance.High)
                        {
                            _e.Cast(EPred.CastPosition);
                        }
                    }
                }
            }
        }
    }
    else
    {
        if ((ObjectManager.Player.Health/ObjectManager.Player.MaxHealth)*100 > _config.Item("ah").GetValue<Slider>().Value)
        {
            if (_config.Item("em").GetValue<bool>())
            {
                if (targett.IsValidTarget(350))
                {
                    if (_e.IsReady())
                    {
                        var EPred = _e.GetPrediction(targett);
                        if (EPred.Hitchance >= HitChance.High)
                        {
                            _e.Cast(EPred.CastPosition);
                        }
                    }
                    if (_config.Item("om").GetValue<bool>())
                    {
                        if (targett.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targett);
                        }
                    }
                    else
                    {
                        if ((_w.IsReady() || _e.IsReady()) && targett.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targett);
                        }
                    }

                }
                else
                {
                    if (!ObjectManager.Player.HasBuff("rengarpassivebuff"))
                    {
                        if (target.IsValidTarget(950))
                        {
                            if (_e.IsReady())
                            {
                                var EPred = _e.GetPrediction(target);
                                if (EPred.Hitchance >= HitChance.High)
                                {
                                    _e.Cast(EPred.CastPosition);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (targett.IsValidTarget(350))
                {
                    if (_config.Item("wkb").GetValue<bool>())
                    {
                        if (ObjectManager.Player.GetSpellDamage(targett, SpellSlot.W) >= targett.Health)
                        {
                            _w.Cast();
                        }
                        else
                        {
                            if (_q.IsReady())
                            {
                                _q.Cast();
                            }
                        }
                        if (ObjectManager.Player.HasBuff("rengarqemp"))
                        {
                            Orbwalking.ResetAutoAttackTimer();
                        }
                        if (_config.Item("om").GetValue<bool>())
                        {
                            if (targett.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                            {
                                ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targett);
                            }
                        }
                        else
                        {
                            if ((_w.IsReady() || _e.IsReady()) && targett.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                            {
                                ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targett);
                            }
                        }
                    }
                    else
                    {
                        if (_q.IsReady())
                        {
                            _q.Cast();
                        }
                        if (ObjectManager.Player.HasBuff("rengarqemp"))
                        {
                            Orbwalking.ResetAutoAttackTimer();
                        }
                        if (_config.Item("om").GetValue<bool>())
                        {
                            if (targett.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                            {
                                ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targett);
                            }
                        }
                        else
                        {
                            if ((_w.IsReady() || _e.IsReady()) && targett.Distance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100)
                            {
                                ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targett);
                            }
                        }
                    }
                }
                else
                {
                    if (_config.Item("eq").GetValue<bool>())
                    {
                        if (!ObjectManager.Player.HasBuff("rengarpassivebuff"))
                        {
                            if (target.IsValidTarget(950))
                            {
                                if (target.Distance(ObjectManager.Player.Position) > 250)
                                {
                                    if (_e.IsReady())
                                    {
                                        var EPred = _e.GetPrediction(target);
                                        if (EPred.Hitchance >= HitChance.High)
                                        {
                                            _e.Cast(EPred.CastPosition);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
#endregion
#region
private static void LaneClear()
{
    if (ObjectManager.Player.Mana < 5)
    {
        if (_e.IsReady())
        {
            var eminions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 950, MinionTypes.All, MinionTeam.NotAlly);
            if (eminions != null)
            {
                if (_config.Item("ok").GetValue<bool>())
                {
                    foreach (var minion in eminions)
                    {
                        if (_config.Item("el").GetValue<bool>())
                        {
                            if (ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E) >= minion.Health)
                            {
                                var EPred = _e.GetPrediction(minion);
                                if (EPred.Hitchance >= HitChance.High)
                                {
                                    _e.Cast(EPred.CastPosition);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var minion in eminions)
                    {
                        if (_config.Item("el").GetValue<bool>())
                        {
                            var EPred = _e.GetPrediction(minion);
                            if (EPred.Hitchance >= HitChance.High)
                            {
                                _e.Cast(EPred.CastPosition);
                            }
                        }
                    }
                }
            }
        }
        var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 350, MinionTypes.All, MinionTeam.NotAlly);
        if (minions != null)
        {
            foreach (var minion in minions)
            {
                if (_config.Item("ql").GetValue<bool>())
                {
                    if (_q.IsReady())
                    {
                        _q.Cast();
                    }
                }
                if (_config.Item("ok").GetValue<bool>())
                {
                    if (_config.Item("wl").GetValue<bool>())
                    {
                        if (_w.IsReady())
                        {
                            if (ObjectManager.Player.GetSpellDamage(minion, SpellSlot.W) >= minion.Health)
                            {
                                _w.Cast();
                            }
                        }
                    }
                }
                else
                {
                    if (_config.Item("wl").GetValue<bool>())
                    {
                        if (_w.IsReady())
                        {
                            _w.Cast();
                        }
                    }
                }
            }
        }
    }
}
#endregion
#region
private static void Comboswitch()
{
    var lasttime = Environment.TickCount - _lastTick;
    if (!_config.Item("cs").GetValue<KeyBind>().Active || lasttime <= Game.Ping)
    {
        return;
    }
    if (_config.Item("em").GetValue<bool>())
    {
        _config.Item("em").SetValue(false);
        _lastTick = Environment.TickCount + 300;
    }
    else
    {
        _config.Item("em").SetValue(true);
        _lastTick = Environment.TickCount + 300;
    }
}
#endregion
#region
private static void Orbswitch()
{
    var lasttime = Environment.TickCount - _lastTick;
    if (!_config.Item("os").GetValue<KeyBind>().Active || lasttime <= Game.Ping)
    {
        return;
    }
    if (_config.Item("om").GetValue<bool>())
    {
        _config.Item("om").SetValue(false);
        _lastTick = Environment.TickCount + 300;
    }
    else
    {
        _config.Item("om").SetValue(true);
        _lastTick = Environment.TickCount + 300;
    }
}
#endregion
}
}
