﻿using IllusionPlugin;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Media;
using System.Linq;
using AsyncTwitch;
using IllusionInjector;
using TMPro;
using CustomUI.GameplaySettings;
namespace GamePlayModifiersPlus
{
    class TwitchCommands
    {
        public static void CheckChargeMessage(TwitchMessage message)
        {
            if (message.BitAmount >= Plugin.Config.bitsPerCharge)
            {
                Plugin.charges += (message.BitAmount / Plugin.Config.bitsPerCharge);
                TwitchConnection.Instance.SendChatMessage("Current Charges: " + Plugin.charges);
            }
            if (message.Author.DisplayName.ToLower().Contains("kyle1413k") && message.Content.ToLower().Contains("!charge"))
            {
                Plugin.charges += (Plugin.Config.chargesForSuperCharge / 2 + 2);
                TwitchConnection.Instance.SendChatMessage("Current Charges: " + Plugin.charges);
            }
            if (message.Content.ToLower().Contains("!gm") && message.Content.ToLower().Contains("super"))
            {
                if (Plugin.charges >= Plugin.Config.chargesForSuperCharge)
                {
                    Plugin.charges -= Plugin.Config.chargesForSuperCharge;
                    Plugin.nextIsSuper = true;
                }
            }
        }

        public static void CheckConfigMessage(TwitchMessage message)
        {
            string messageString = message.Content.ToLower();
            if (!messageString.Contains("!configchange"))  return;
            if (!message.Author.IsMod && !message.Author.IsBroadcaster) return;
                string command = "";
            string property = "";
            bool isPropertyOnly = false;
            string value = value = messageString.Split('=')[1]; 
            string arg1 = messageString.Split(' ', ' ')[1];
            string arg2 = messageString.Split(' ', ' ', '=')[2];
            Plugin.Log(arg1 + " " + arg2 + " " + value);
            switch (arg1)
            {
                case "da":
                    command = "da";
                    break;
                case "smaller":
                    command = "smaller";
                    break;
                case "larger":
                    command = "larger";
                    break;
                case "random":
                    command = "random";
                    break;
                case "instafail":
                    command = "instafail";
                    break;
                case "invincible":
                    command = "invincible";
                    break;
                case "njsrandom":
                    command = "njsrandom";
                    break;
                case "noarrows":
                    command = "noarrows";
                    break;
                case "funky":
                    command = "funky";
                    break;
                case "rainbow":
                    command = "rainbow";
                    break;
                default:
                    isPropertyOnly = true;
                    break;

            }
            if (isPropertyOnly)
                {
                    switch (arg1.Split('=')[0])
                    {
                        case "bitspercharge":
                            property = "bitspercharge";
                            break;
                        case "chargesforsupercharge":
                            property = "chargesforsupercharge";
                            break;
                        case "maxcharges":
                            property = "maxcharges";
                            break;
                        case "chargesperlevel":
                            property = "chargesperlevel";
                            break;
                        case "allowsubs":
                            property = "allowsubs";
                            break;
                        case "alloweveryone":
                            property = "alloweveryone";
                            break;
                        default:
                            return;
                    }
                    Plugin.Config.ChangeConfigValue(property, value);
                }
                else
                {
                    switch(arg2)
                    {
                        case "chargecost":
                            property = "chargecost";
                            break;
                        case "cooldown":
                            property = "cooldown";
                            break;
                        case "duration":
                            property = "duration";
                            break;
                        case "bitspercharge":
                            property = "bitspercharge";
                            break;
                        case "chargesforsupercharge":
                            property = "chargesforsupercharge";
                            break;
                        case "maxcharges":
                            property = "maxcharges";
                            break;
                        case "chargesperlevel":
                            property = "chargesperlevel";
                            break;
                        case "min":
                            property = "min";
                            break;
                        case "max":
                            property = "max";
                            break;
                        case "allowsubs":
                            property = "allowsubs";
                            break;
                        case "alloweveryone":
                            property = "alloweveryone";
                            break;
                        default:
                            return;
                    }

                    Plugin.Config.ChangeConfigValue(command, property, value);
            }


            

        }

        public static void CheckInfoCommands(TwitchMessage message)
        {
            if (message.Content.ToLower().Contains("!gm help"))
            {
                TwitchConnection.Instance.SendChatMessage("Include !gm followed by a command in your message while the streamer has twitch mode on to mess with their game:" +
                    " !gm commands to view available commands");
            }
            if (message.Content.ToLower().Contains("!gm chargehelp"))
            {
                TwitchConnection.Instance.SendChatMessage("Every " + Plugin.Config.bitsPerCharge + " bits sent with a message adds a charge, which are used to activate commands! If you add super at the end of a command, it will cost " + Plugin.Config.chargesForSuperCharge + " Charges but will make the effect last much longer! " + Plugin.Config.chargesPerLevel + " Charges are generated every song with chat mode on");
            }
            if (message.Content.ToLower().Contains("!gm commands"))
            {
                TwitchConnection.Instance.SendChatMessage("Currently supported commands | status: whether chat integration is on, who is allowed to use it | game: Details about game commands | charges: view current charges and costs | chargehelp: Explain charge system | mod: view moderator commands");
            }

            if (message.Content.ToLower().Contains("!gm mod"))
            {
                TwitchConnection.Instance.SendChatMessage("Current mod commands: ConfigChange: change config values. Sample config : https://imgur.com/76OtpSx Syntax: !configchange command property=value. Example: '!configchange da chargecost=5' or '!configchange bitsPerCharge=50' ");
            }
            if (message.Content.ToLower().Contains("!gm game"))
            {
                TwitchConnection.Instance.SendChatMessage("Commands | DA: Disappearing Arrows mode | Instafail: instant fail mode | Invincible: invincibility | Smaller/Larger/Random: Change note sizes | njsRandom: Randomly change NoteJumpSpeed | noArrows: No Arrows mode | Funky: Death | Rainbow: RAINBOWW");
            }


            if (message.Content.ToLower().Contains("!gm charges"))
            {
                TwitchConnection.Instance.SendChatMessage("Charges: " + Plugin.charges + " | " + Plugin.Config.GetChargeCostString());
            }
        }

        public static void CheckStatusCommands(TwitchMessage message)
        {
            if (message.Content.ToLower().Contains("!gm pp"))
            {
                if (Plugin.currentpp != 0)
                    TwitchConnection.Instance.SendChatMessage("Streamer Rank: #" + Plugin.currentRank + ". Streamer pp: " + Plugin.currentpp + "pp");
                else
                    TwitchConnection.Instance.SendChatMessage("Currently do not have streamer info");
            }
            if (message.Content.ToLower().Contains("!gm status"))
            {
                string scopeMessage = "";
                int scope = CheckCommandScope();
                switch(scope)
                {
                    case 0:
                        scopeMessage = "Everyone has access to commands";
                        break;
                    case 1:
                        scopeMessage = "Subscribers have access to commands";
                        break;
                    case 2:
                        scopeMessage = "Moderators have access to commands";
                        break;

                }

                Plugin.beepSound.Play();
                if (Plugin.twitchStuff)
                    TwitchConnection.Instance.SendChatMessage("Chat Integration Enabled. " + scopeMessage);
                else
                    TwitchConnection.Instance.SendChatMessage("Chat Integration Not Enabled. " + scopeMessage);
            }
        }

        public static void CheckHealthCommands(TwitchMessage message)
        {
            if (!Plugin.cooldowns.GetCooldown("Health"))
            {
                if (message.Content.ToLower().Contains("!gm instafail"))
                {

                    if (Plugin.nextIsSuper && Plugin.charges >= Plugin.Config.instaFailChargeCost)
                    {
                        Plugin.beepSound.Play();
                        SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.TempInstaFail(Plugin.songAudio.clip.length));
                        SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.songAudio.clip.length, "Health", "Super Insta Fail Active."));
                        Plugin.nextIsSuper = false;
                        Plugin.healthActivated = true;
                        Plugin.charges -= Plugin.Config.instaFailChargeCost;
                    }
                    else if (Plugin.charges >= Plugin.Config.instaFailChargeCost)
                    {
                        Plugin.beepSound.Play();
                        SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.TempInstaFail(Plugin.Config.instaFailDuration));
                        SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.Config.instaFailCooldown, "Health", "Insta Fail Active."));
                        Plugin.healthActivated = true;
                        Plugin.charges -= Plugin.Config.instaFailChargeCost;
                    }

                }

                if (message.Content.ToLower().Contains("!gm invincible") && !Plugin.healthActivated && !Plugin.cooldowns.GetCooldown("Health"))
                {
                    if (Plugin.nextIsSuper && Plugin.charges >= Plugin.Config.invincibleChargeCost)
                    {
                        Plugin.beepSound.Play();
                        SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.TempInvincibility(Plugin.songAudio.clip.length));
                        SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.songAudio.clip.length, "Health", "Super Invincibility Active."));
                        Plugin.nextIsSuper = false;
                        Plugin.charges -= Plugin.Config.invincibleChargeCost;
                    }
                    else if (Plugin.charges >= Plugin.Config.invincibleChargeCost)
                    {
                        Plugin.beepSound.Play();
                        SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.TempInvincibility(Plugin.Config.invincibleDuration));
                        SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.Config.invincibleCooldown, "Health", "Invincibility Active."));
                        Plugin.charges -= Plugin.Config.invincibleChargeCost;
                    }

                }
            }
        }

        public static void CheckSizeCommands(TwitchMessage message)
        {
            if (message.Content.ToLower().Contains("!gm smaller")&& !Plugin.cooldowns.GetCooldown("NormalSize"))
            {
                if (Plugin.nextIsSuper && Plugin.charges >= Plugin.Config.smallerNoteChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.ScaleNotes(0.7f, Plugin.songAudio.clip.length));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.songAudio.clip.length, "NormalSize", "Super Note Scale Change Active."));
                    Plugin.nextIsSuper = false;
                    Plugin.sizeActivated = true;
                    Plugin.charges -= Plugin.Config.smallerNoteChargeCost;
                }
                else if (Plugin.charges >= Plugin.Config.smallerNoteChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.ScaleNotes(0.7f, Plugin.Config.smallerNoteDuration));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.Config.smallerNotesCooldown, "NormalSize", "Temporarily Scaling Notes"));
                    Plugin.sizeActivated = true;
                    Plugin.charges -= Plugin.Config.smallerNoteChargeCost;
                }

            }

            if (message.Content.ToLower().Contains("!gm larger") && !Plugin.cooldowns.GetCooldown("NormalSize") && !Plugin.sizeActivated)
            {
                if (Plugin.nextIsSuper && Plugin.charges >= Plugin.Config.largerNotesChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.ScaleNotes(1.3f, Plugin.songAudio.clip.length));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.songAudio.clip.length, "NormalSize", "Super Note Scale Change Active."));
                    Plugin.nextIsSuper = false;
                    Plugin.charges -= Plugin.Config.largerNotesChargeCost;
                }
                else if (Plugin.charges >= Plugin.Config.largerNotesChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.ScaleNotes(1.3f, Plugin.Config.largerNotesDuration));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.Config.largerNotesCooldown, "NormalSize", "Temporarily Scaling Notes"));
                    Plugin.charges -= Plugin.Config.largerNotesChargeCost;
                }

            }

            if (message.Content.ToLower().Contains("!gm random") && !Plugin.cooldowns.GetCooldown("Random"))
            {
                if (Plugin.nextIsSuper && Plugin.charges >= Plugin.Config.randomNotesChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.RandomNotes(Plugin.songAudio.clip.length));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.songAudio.clip.length, "Random", "Super Random Note Scale Change Active."));
                    Plugin.nextIsSuper = false;
                    Plugin.superRandom = true;
                    Plugin.charges -= Plugin.Config.randomNotesChargeCost;
                }
                else if (Plugin.charges >= Plugin.Config.randomNotesChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.RandomNotes(Plugin.Config.randomNotesDuration));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.Config.randomNotesCooldown, "Random", "Randomly Scaling Notes"));
                    Plugin.charges -= Plugin.Config.randomNotesChargeCost;
                }

            }

        }

        public static void CheckGameplayCommands(TwitchMessage message)
        {

            if (message.Content.ToLower().Contains("!gm da") && !Plugin.cooldowns.GetCooldown("Note") && !Plugin.levelData.gameplayCoreSetupData.gameplayModifiers.disappearingArrows)
            {
                if (Plugin.nextIsSuper && Plugin.charges >= Plugin.Config.daChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.TempDA(Plugin.songAudio.clip.length));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.songAudio.clip.length, "DA", "Super DA Active."));
                    Plugin.nextIsSuper = false;
                    Plugin.charges -= Plugin.Config.daChargeCost;
                }
                else if (Plugin.charges >= Plugin.Config.daChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.TempDA(Plugin.Config.daDuration));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.Config.daCooldown, "DA", "DA Active."));
                    Plugin.charges -= Plugin.Config.daChargeCost;
                }
            }

            if (message.Content.ToLower().Contains("!gm njsrandom") && !Plugin.cooldowns.GetCooldown("RandomNJS"))
            {

                if (Plugin.nextIsSuper && Plugin.charges >= Plugin.Config.njsRandomChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.njsRandom(Plugin.songAudio.clip.length));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.songAudio.clip.length, "RandomNJS", "Super Random Note Jump Speed Active."));
                    Plugin.nextIsSuper = false;
                    Plugin.charges -= Plugin.Config.njsRandomChargeCost;
                }
                else if (Plugin.charges >= Plugin.Config.njsRandomChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.njsRandom(Plugin.Config.njsRandomDuration));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.Config.njsRandomCooldown, "RandomNJS", "Random Note Jump Speed Active."));
                    Plugin.charges -= Plugin.Config.njsRandomChargeCost;
                }
            }
            if (message.Content.ToLower().Contains("!gm noarrows") && !Plugin.cooldowns.GetCooldown("NoArrows"))
            {

                if (Plugin.nextIsSuper && Plugin.charges >= Plugin.Config.noArrowsChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.TempNoArrows(Plugin.songAudio.clip.length));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.songAudio.clip.length, "RandomNJS", "Super Random Note Jump Speed Active."));
                    Plugin.nextIsSuper = false;
                    Plugin.charges -= Plugin.Config.noArrowsChargeCost;
                }
                else if (Plugin.charges >= Plugin.Config.noArrowsChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.TempNoArrows(Plugin.Config.noArrowsDuration));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.Config.noArrowsCooldown, "NoArrows", "Temporary No Arrows Activated"));
                    Plugin.charges -= Plugin.Config.noArrowsChargeCost;
                }
            }
            if (message.Content.ToLower().Contains("!gm funky") && !Plugin.cooldowns.GetCooldown("Funky"))
            {

                if (Plugin.nextIsSuper && Plugin.charges >= Plugin.Config.instaFailChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.Funky(Plugin.songAudio.clip.length));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.songAudio.clip.length, "Funky", "Time to get Funky."));
                    Plugin.nextIsSuper = false;
                    Plugin.charges -= Plugin.Config.funkyChargeCost;
                }
                else if (Plugin.charges >= Plugin.Config.funkyChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.Funky(Plugin.Config.funkyDuration));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.Config.funkyCooldown, "Funky", "Funky Mode Activated"));
                    Plugin.charges -= Plugin.Config.funkyChargeCost;
                }
            }
            if (message.Content.ToLower().Contains("!gm rainbow") && !Plugin.cooldowns.GetCooldown("Rainbow"))
            {

                if (Plugin.nextIsSuper && Plugin.charges >= Plugin.Config.rainbowChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.Rainbow(Plugin.songAudio.clip.length));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.songAudio.clip.length, "Rainbow", "RAIIINBOWWS."));
                    Plugin.nextIsSuper = false;
                    Plugin.charges -= Plugin.Config.rainbowChargeCost;
                }
                else if (Plugin.charges >= Plugin.Config.rainbowChargeCost)
                {
                    Plugin.beepSound.Play();
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.Rainbow(Plugin.Config.rainbowDuration));
                    SharedCoroutineStarter.instance.StartCoroutine(TwitchPowers.CoolDown(Plugin.Config.rainbowCooldown, "Rainbow", "Rainbow Activated"));
                    Plugin.charges -= Plugin.Config.rainbowChargeCost;
                }
            }

            }



        public static int CheckCommandScope()
        {

            if (Plugin.Config.allowEveryone) return 0;
            else if (Plugin.Config.allowSubs) return 1;
            else return 2;

        }
    }
}