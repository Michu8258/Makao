﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MakaoGraphicsRepresentation.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.2.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("UserName")]
        public string CurrentUserName {
            get {
                return ((string)(this["CurrentUserName"]));
            }
            set {
                this["CurrentUserName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("/Avatars/01.png")]
        public string CurrentUserAvatar {
            get {
                return ((string)(this["CurrentUserAvatar"]));
            }
            set {
                this["CurrentUserAvatar"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int AmountOfPlayers {
            get {
                return ((int)(this["AmountOfPlayers"]));
            }
            set {
                this["AmountOfPlayers"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int AmountOfDecks {
            get {
                return ((int)(this["AmountOfDecks"]));
            }
            set {
                this["AmountOfDecks"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int AmountOfJokers {
            get {
                return ((int)(this["AmountOfJokers"]));
            }
            set {
                this["AmountOfJokers"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int AmountOfStartCards {
            get {
                return ((int)(this["AmountOfStartCards"]));
            }
            set {
                this["AmountOfStartCards"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int PlayedGames {
            get {
                return ((int)(this["PlayedGames"]));
            }
            set {
                this["PlayedGames"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int WaitingForReadinessTimeout {
            get {
                return ((int)(this["WaitingForReadinessTimeout"]));
            }
            set {
                this["WaitingForReadinessTimeout"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Blue")]
        public global::CardGraphicsLibraryHandler.BackColor CardBacks {
            get {
                return ((global::CardGraphicsLibraryHandler.BackColor)(this["CardBacks"]));
            }
            set {
                this["CardBacks"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ReadinessTimeoutEnabled {
            get {
                return ((bool)(this["ReadinessTimeoutEnabled"]));
            }
            set {
                this["ReadinessTimeoutEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool JoiningTimeoutEnabled {
            get {
                return ((bool)(this["JoiningTimeoutEnabled"]));
            }
            set {
                this["JoiningTimeoutEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int WaitingForJoiningTimeout {
            get {
                return ((int)(this["WaitingForJoiningTimeout"]));
            }
            set {
                this["WaitingForJoiningTimeout"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int PlayedAndWonGames {
            get {
                return ((int)(this["PlayedAndWonGames"]));
            }
            set {
                this["PlayedAndWonGames"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Left")]
        public global::CardsRepresentation.ThirdPlayerLocation LocationOfThirdPlayer {
            get {
                return ((global::CardsRepresentation.ThirdPlayerLocation)(this["LocationOfThirdPlayer"]));
            }
            set {
                this["LocationOfThirdPlayer"] = value;
            }
        }
    }
}
