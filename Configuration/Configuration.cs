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

        private int retryJoinGameIntervalField;

        public PlayerSettings()
        {
            this.retryJoinGameIntervalField = (5000);
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(int), "5000")]
        public int RetryJoinGameInterval
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

        private int keepAliveIntervalField;

        public Configuration()
        {
            this.keepAliveIntervalField = 30000;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(int), "30000")]
        public int KeepAliveInterval
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

        private int retryRegisterGameIntervalField;

        public GameMasterSettings() { }


        public static GameMasterSettings GetDefaultGameMasterSettings()
        {
            return new GameMasterSettings()
            {
                retryRegisterGameIntervalField = 5000,
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
        public int RetryRegisterGameInterval
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

        private int placingNewPiecesFrequencyField;

        private int initialNumberOfPiecesField;

        private int boardWidthField;

        private int taskAreaLengthField;

        private int goalAreaLengthField;

        private int numberOfPlayersPerTeamField;

        private string gameNameField;

        private int numberOfGoalsPerGame;

        public GameMasterSettingsGameDefinition() { }

        public static GameMasterSettingsGameDefinition GetDefaultGameDefinition()
        {
            return new GameMasterSettingsGameDefinition()
            {
                shamProbabilityField = 0.1D,
                placingNewPiecesFrequencyField = 1000,
                initialNumberOfPiecesField = 4,
                boardWidthField = 5,
                taskAreaLengthField = 7,
                goalAreaLengthField = 3,
                numberOfPlayersPerTeamField = 4,
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

        public void SetInitialPiecesNumber(int number)
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
        public int PlacingNewPiecesFrequency
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
        public int InitialNumberOfPieces
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
        //[System.Xml.Serialization.XmlElementAttribute(DataType="nonNegativeInteger")]
        public int BoardWidth
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
        public int TaskAreaLength
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
        public int GoalAreaLength
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
        public int NumberOfPlayersPerTeam
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

        private int moveDelayField;

        private int discoverDelayField;

        private int testDelayField;

        private int destroyDelayField;

        private int pickUpDelayField;

        private int placingDelayField;

        private int knowledgeExchangeDelayField;

        public GameMasterSettingsActionCosts()
        {

        }

        public static GameMasterSettingsActionCosts GetDefaultCosts()
        {
            return new GameMasterSettingsActionCosts()
            {
                moveDelayField = 100,
                discoverDelayField = 450,
                testDelayField = 500,
                pickUpDelayField = 100,
                placingDelayField = 100,
                knowledgeExchangeDelayField = 1200
            };
        }

        /// <remarks/>
        public int MoveDelay
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
        public int DiscoverDelay
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
        public int TestDelay
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
        public int DestroyDelay
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
        public int PickUpDelay
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
        public int PlacingDelay
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
        public int KnowledgeExchangeDelay
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
