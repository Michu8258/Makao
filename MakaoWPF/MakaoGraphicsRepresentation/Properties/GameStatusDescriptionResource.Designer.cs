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
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class GameStatusDescriptionResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal GameStatusDescriptionResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MakaoGraphicsRepresentation.Properties.GameStatusDescriptionResource", typeof(GameStatusDescriptionResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tryb bitwy.
        /// </summary>
        internal static string BattleStatus01 {
            get {
                return ResourceManager.GetString("BattleStatus01", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kiedy jeden z graczy postanowi wyłożyć na stół kartę waleczną, bitewną, rozpocznie się tryb bitwy. Wówczas na stół można wykładać tylko karty bitewne, i to takie, które będą pasowały kolorem lub figurą do poprzedniej karty. Przypominając, mowa tu o wszystkich dwójkach i trójkach, a także o królach pik i serce (pozostałe dwa króle nie są kartami bitewnymi)..
        /// </summary>
        internal static string BattleStatus02 {
            get {
                return ResourceManager.GetString("BattleStatus02", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Przy wykładaniu kart na stół, zliczana jest liczba kart do wzięcia przez gracza, który poniesie porażkę w bitwie - każda dwójka to 2, trójka to 3, a krót bitewny to 5 dodatkowych kart. Porażkę odnosi ten gracz, który nie może wyłożyć na stół żadnej pasującej karty bitewnej (może także skorzystać z jokera). Gdy gracz nie ma ruchu, wtedy musi wziąć kartę ze stosu - odpowiednia ilość od razu zostanie dodana do jego puli kart, a tryb gry wróci do standardowego.
        ///W tym trybie obowiązuje dodatkowa funkcjonalność  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string BattleStatus03 {
            get {
                return ResourceManager.GetString("BattleStatus03", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tryb żądania figur.
        /// </summary>
        internal static string RankDemandingStatus01 {
            get {
                return ResourceManager.GetString("RankDemandingStatus01", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tryb żądania figur może być rozpoczęty wyłącznie wtedy, gdy gracz, któremu w danym momencie przysługuje ruch, wyłoży na stół jednego lub więcej wleta. Wówczas może on zażądać od pozostałych graczy wyłożenia na stół konkretnej figury - wyłącznie z puli kart niewalecznych i niefunkcyjnych..
        /// </summary>
        internal static string RankDemandingStatus02 {
            get {
                return ResourceManager.GetString("RankDemandingStatus02", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Jeśli gracz, który wykonuje ruch po graczu, który żądanie figur rozpoczął, a nie ma w ręku karty z żądaną figurą- bierze jedną kartę ze stosu. Jeśli dobrana figura pasuje do żądanej figury, wówczas może tę kartę położyć na stole (wyłącznie tę konkretną kartę). Natomiast jeśli gracz ma w ręku innego waleta, wówczas może przebić żądanie - zażądać innej figury. Jeśli na stół zostanie wyłożona choć jedna zażądana figura, wóczas żądania nie można już przebić. Dodatkowo istnieje możliwość wyłożenia na stół waleta [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RankDemandingStatus03 {
            get {
                return ResourceManager.GetString("RankDemandingStatus03", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tryb standardowy.
        /// </summary>
        internal static string StandardStatus01 {
            get {
                return ResourceManager.GetString("StandardStatus01", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tryb standardowy jest podstawowym trybem gry. Podczas gry w tym trybie, żadenemu graczowi nie grożą żadne negatywne konsekwencje. Z tego trybu można prześć bezpośrednio do każdego z innych trybów - zależy to od karty położonej na stół przez jednego z graczy. Wykładanie na stół niewalecznych i niefunkcyjnych kart podtrzymuje obowiązywanie tego trybu gry..
        /// </summary>
        internal static string StandardStatus02 {
            get {
                return ResourceManager.GetString("StandardStatus02", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Przejścia do pozostałych trybów gry:
        ///1. Tryb żądania figur - jeden z graczy musi wyłożyć na stół co najmniej jednego waleta, oraz zażądać konkretnej figury,
        ///2. Tryb żadania koloru - jeden z graczy musi wyłożyć na stół co najmniej jednego asa, oraz zażądać konkretnego koloru,
        ///3. Tryb postoju - jeden z graczy musi wyłożyć na stół co najmniej jedną czwórkę,
        ///4. Tryb bitwy - jeden z graczy musi wyłożyć na stół co najmniej jedną kartę waleczną, bitewną (dwójka, czwórka, król (serce lub pik).
        ///Do wszystkich wy [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string StandardStatus03 {
            get {
                return ResourceManager.GetString("StandardStatus03", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tryb postojów.
        /// </summary>
        internal static string StopsStatus01 {
            get {
                return ResourceManager.GetString("StopsStatus01", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do trybu postojów przechodzi się wówczas, gdy jeden z graczy wyłoży na stół choć jedną czwórkę. Wóczas kolejny gracz również może położyć na stół czwórkę. Różnicą w porównaniu do trybów żądania koloru czy figury jest konieczność posiadania przez kolenych graczy czwórek w rękach..
        /// </summary>
        internal static string StopsStatus02 {
            get {
                return ResourceManager.GetString("StopsStatus02", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to W przypadku, gdy poprzedni gracz wyłożył na stół czwórkę, nie można dobrać karty ze stołu - ta możliwość jest zablokowana. W związku z tym, kolejny gracz musi mieć czwórkę w ręku (lub jokera, któego zamieni w czwórkę). Jeśli gracz nie ma żadnej z powyższych możliwości ruchu - czeka tyle kolejek, ile czwórek pojawiło się w czasie trwania tego trybu na stole. Tzn. jeśli zostały wyłożone trzy czwórki, to stoi trzy kolejki. Oznacza to, że jeśli przychodzi kolej ruchu pauzującego gracza, nie wykonuje on ruchu, a [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string StopsStatus03 {
            get {
                return ResourceManager.GetString("StopsStatus03", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tryb żądania koloru.
        /// </summary>
        internal static string SuitDemandingStatus01 {
            get {
                return ResourceManager.GetString("SuitDemandingStatus01", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tryb żądania koloru jest bardzo zbliżony do trybu żądania figury. W tym przypadku, kartą którą można rozpocząć żądanie koloru, jest as. Przy wyłożeniu na stół jednego lub więcen asów, można zażądać od kolejnego gracza wyłożenia na stół konkretnego koloru (ważny jest kolor, w związku z tym, figura nie ma tutaj znaczenia): pik, kier, karo, trefl..
        /// </summary>
        internal static string SuitDemandingStatus02 {
            get {
                return ResourceManager.GetString("SuitDemandingStatus02", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Podobnie jak w przypadku żądania koloru, gracz, którego kolej nastęuje po graczu, który zażądał koloru, może to żądanie przebić. Natomiast jeśli gracz kolejny nie ma żadnej karty o żądanym kolorze w ręku, dobiera jedną ze stosu kart (jeśli ta karta pasuje kolorem do żądania, wówczas może tę jedną kartę położyć na stole). W przypadku żądania koloru, tryb przestaje obowiązywać z pierwszą kartą wyłożoną na stół, której kolor pasuje do żądania. W związku z tym nie jest konieczne dotarcie ruchy do gracza rozpocz [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SuitDemandingStatus03 {
            get {
                return ResourceManager.GetString("SuitDemandingStatus03", resourceCulture);
            }
        }
    }
}
