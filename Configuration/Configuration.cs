namespace Configuration
{
    using Messages;
    using System;
    using System.Collections.Generic;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable = false)]
    public partial class PlayerSettings : Configuration
    {

        private uint retryJoinGameIntervalField;

        public PlayerSettings()
        {
            this.retryJoinGameIntervalField = ((uint)(5000));
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(uint), "5000")]
        public uint RetryJoinGameInterval
        {
            get
            {
                return this.retryJoinGameIntervalField;
            }
            set
            {
                this.retryJoinGameIntervalField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable = true)]
    public partial class Configuration
    {

        private uint keepAliveIntervalField;

        public Configuration()
        {
            this.keepAliveIntervalField = ((uint)(30000));
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(int), "30000")]
        public uint KeepAliveInterval
        {
            get
            {
                return this.keepAliveIntervalField;
            }
            set
            {
                this.keepAliveIntervalField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable = false)]
    public partial class GameMasterSettings : Configuration
    {

        private GameMasterSettingsGameDefinition gameDefinitionField;

        private GameMasterSettingsActionCosts actionCostsField;

        private uint retryRegisterGameIntervalField;

        public GameMasterSettings() { }


        public static GameMasterSettings GetDefaultGameMasterSettings()
        {
            return new GameMasterSettings()
            {
                retryRegisterGameIntervalField = ((uint)(5000)),
                actionCostsField = new GameMasterSettingsActionCosts(),
                gameDefinitionField = GameMasterSettingsGameDefinition.GetDefaultGameDefinition()
            };
        }

        public static GameMasterSettings GetGameMasterSettings(GameMasterSettingsGameDefinition settings)
        {
            return new GameMasterSettings()
            {
                retryRegisterGameIntervalField = 5000,
                gameDefinitionField = settings
            };
        }

        /// <remarks/>
        public GameMasterSettingsGameDefinition GameDefinition
        {
            get
            {
                return this.gameDefinitionField;
            }
            set
            {
                this.gameDefinitionField = value;
            }
        }

        /// <remarks/>
        public GameMasterSettingsActionCosts ActionCosts
        {
            get
            {
                return this.actionCostsField;
            }
            set
            {
                this.actionCostsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(int), "5000")]
        public uint RetryRegisterGameInterval
        {
            get
            {
                return this.retryRegisterGameIntervalField;
            }
            set
            {
                this.retryRegisterGameIntervalField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    public partial class GameMasterSettingsGameDefinition
    {

        private GoalField[] goalsField;

        private double shamProbabilityField;

        private uint placingNewPiecesFrequencyField;

        private uint initialNumberOfPiecesField;

        private uint boardWidthField;

        private uint taskAreaLengthField;

        private uint goalAreaLengthField;

        private uint numberOfPlayersPerTeamField;

        private string gameNameField;

        private int numberOfGoalsPerGame;

        public GameMasterSettingsGameDefinition() { }

        public static GameMasterSettingsGameDefinition GetDefaultGameDefinition()
        {
            return new GameMasterSettingsGameDefinition()
            {
                gameNameField = "DefaultGame" + DateTime.Now.Ticks,
                shamProbabilityField = 0.1D,
                placingNewPiecesFrequencyField = ((uint)(1000)),
                initialNumberOfPiecesField = ((uint)(4)),
                boardWidthField = ((uint)(5)),
                taskAreaLengthField = ((uint)(7)),
                goalAreaLengthField = ((uint)(3)),
                numberOfPlayersPerTeamField = ((uint)(4)),
                numberOfGoalsPerGame = 10,
                goalsField = new GoalField[]
                {
                    new GoalField() { x = 1, y = 1, type = GoalFieldType.goal, team = TeamColour.blue },
                new GoalField() { x = 2, y = 2, type = GoalFieldType.goal, team = TeamColour.blue },
                new GoalField() { x = 3, y = 0, type = GoalFieldType.goal, team = TeamColour.blue },
                new GoalField() { x = 2, y = 11, type = GoalFieldType.goal, team = TeamColour.red },
                new GoalField() { x = 3, y = 10, type = GoalFieldType.goal, team = TeamColour.red },
                new GoalField() { x = 4, y = 12, type = GoalFieldType.goal, team = TeamColour.red }
                }
            };
        }

        public void SetInitialPiecesNumber(uint number)
        {
            initialNumberOfPiecesField = number;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Goals")]
        public GoalField[] Goals
        {
            get
            {
                return this.goalsField;
            }
            set
            {
                this.goalsField = value;
            }
        }

        /// <remarks/>
        public double ShamProbability
        {
            get
            {
                return this.shamProbabilityField;
            }
            set
            {
                this.shamProbabilityField = value;
            }
        }

        /// <remarks/>
        public uint PlacingNewPiecesFrequency
        {
            get
            {
                return this.placingNewPiecesFrequencyField;
            }
            set
            {
                this.placingNewPiecesFrequencyField = value;
            }
        }

        /// <remarks/>
        public uint InitialNumberOfPieces
        {
            get
            {
                return this.initialNumberOfPiecesField;
            }
            set
            {
                this.initialNumberOfPiecesField = value;
            }
        }

        /// <remarks/>
        public uint BoardWidth
        {
            get
            {
                return this.boardWidthField;
            }
            set
            {
                this.boardWidthField = value;
            }
        }

        /// <remarks/>
        public uint TaskAreaLength
        {
            get
            {
                return this.taskAreaLengthField;
            }
            set
            {
                this.taskAreaLengthField = value;
            }
        }

        /// <remarks/>
        public uint GoalAreaLength
        {
            get
            {
                return this.goalAreaLengthField;
            }
            set
            {
                this.goalAreaLengthField = value;
            }
        }

        /// <remarks/>
        public uint NumberOfPlayersPerTeam
        {
            get
            {
                return this.numberOfPlayersPerTeamField;
            }
            set
            {
                this.numberOfPlayersPerTeamField = value;
            }
        }

        /// <remarks/>
        public string GameName
        {
            get
            {
                return this.gameNameField;
            }
            set
            {
                this.gameNameField = value;
            }
        }
    }

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable=true)]
    //public partial class GoalField : Field {

    //    private GoalFieldType typeField;

    //    private TeamColour teamField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public GoalFieldType type {
    //        get {
    //            return this.typeField;
    //        }
    //        set {
    //            this.typeField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public TeamColour team {
    //        get {
    //            return this.teamField;
    //        }
    //        set {
    //            this.teamField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable=false)]
    //public enum GoalFieldType {

    //    /// <remarks/>
    //    goal,

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlEnumAttribute("non-goal")]
    //    nongoal,
    //}

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    //[System.SerializableAttribute()]
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable=false)]
    //public enum TeamColour {

    //    /// <remarks/>
    //    red,

    //    /// <remarks/>
    //    blue,
    //}

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(GoalField))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable = true)]
    public abstract partial class Field : Location
    {
    }

    


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    public partial class GameMasterSettingsActionCosts
    {

        private uint moveDelayField;

        private uint discoverDelayField;

        private uint testDelayField;

        private uint destroyDelayField;

        private uint pickUpDelayField;

        private uint placingDelayField;

        private uint knowledgeExchangeDelayField;

        private uint suggestActionDelayField;

        public GameMasterSettingsActionCosts()
        {
            this.moveDelayField = ((uint)(100));
            this.discoverDelayField = ((uint)(450));
            this.testDelayField = ((uint)(500));
            this.destroyDelayField = ((uint)(100));
            this.pickUpDelayField = ((uint)(100));
            this.placingDelayField = ((uint)(100));
            this.knowledgeExchangeDelayField = ((uint)(1200));
            this.suggestActionDelayField = ((uint)(200));
        }

        /// <remarks/>
        public uint MoveDelay
        {
            get
            {
                return this.moveDelayField;
            }
            set
            {
                this.moveDelayField = value;
            }
        }

        /// <remarks/>
        public uint DiscoverDelay
        {
            get
            {
                return this.discoverDelayField;
            }
            set
            {
                this.discoverDelayField = value;
            }
        }

        /// <remarks/>
        public uint TestDelay
        {
            get
            {
                return this.testDelayField;
            }
            set
            {
                this.testDelayField = value;
            }
        }

        /// <remarks/>
        public uint DestroyDelay
        {
            get
            {
                return this.destroyDelayField;
            }
            set
            {
                this.destroyDelayField = value;
            }
        }

        /// <remarks/>
        public uint PickUpDelay
        {
            get
            {
                return this.pickUpDelayField;
            }
            set
            {
                this.pickUpDelayField = value;
            }
        }

        /// <remarks/>
        public uint PlacingDelay
        {
            get
            {
                return this.placingDelayField;
            }
            set
            {
                this.placingDelayField = value;
            }
        }

        /// <remarks/>
        public uint KnowledgeExchangeDelay
        {
            get
            {
                return this.knowledgeExchangeDelayField;
            }
            set
            {
                this.knowledgeExchangeDelayField = value;
            }
        }

        /// <remarks/>
        public uint SuggestActionDelay
        {
            get
            {
                return this.suggestActionDelayField;
            }
            set
            {
                this.suggestActionDelayField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable = false)]
    public partial class CommunicationServerSettings : Configuration
    {
    }
}
