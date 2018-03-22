namespace Configuration {
    using Messages;
    using System;
    using System.Collections.Generic;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable=false)]
    public partial class PlayerSettings : Configuration {
        
        private uint retryJoinGameIntervalField;
        
        public PlayerSettings() {
            this.retryJoinGameIntervalField = ((uint)(5000));
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(uint), "5000")]
        public uint RetryJoinGameInterval {
            get {
                return this.retryJoinGameIntervalField;
            }
            set {
                this.retryJoinGameIntervalField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable=true)]
    public partial class Configuration {
        
        private uint keepAliveIntervalField;
        
        public Configuration() {
            this.keepAliveIntervalField = ((uint)(30000));
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(uint), "30000")]
        public uint KeepAliveInterval {
            get {
                return this.keepAliveIntervalField;
            }
            set {
                this.keepAliveIntervalField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable=false)]
    public partial class GameMasterSettings : Configuration {
        
        private GameMasterSettingsGameDefinition gameDefinitionField;
        
        private GameMasterSettingsActionCosts actionCostsField;
        
        private uint retryRegisterGameIntervalField;
        
        public GameMasterSettings() {
            this.retryRegisterGameIntervalField = ((uint)(5000));
        }
        
        /// <remarks/>
        public GameMasterSettingsGameDefinition GameDefinition {
            get {
                return this.gameDefinitionField;
            }
            set {
                this.gameDefinitionField = value;
            }
        }
        
        /// <remarks/>
        public GameMasterSettingsActionCosts ActionCosts {
            get {
                return this.actionCostsField;
            }
            set {
                this.actionCostsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(uint), "5000")]
        public uint RetryRegisterGameInterval {
            get {
                return this.retryRegisterGameIntervalField;
            }
            set {
                this.retryRegisterGameIntervalField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    public partial class GameMasterSettingsGameDefinition {
        
        private GoalField[] goalsField;
        
        private double shamProbabilityField;
        
        private uint placingNewPiecesFrequencyField;
        
        private uint initialNumberOfPiecesField;
        
        private string boardWidthField;
        
        private string taskAreaLengthField;
        
        private string goalAreaLengthField;
        
        private string numberOfPlayersPerTeamField;
        
        private string gameNameField;

        public GameMasterSettingsGameDefinition() {
            this.shamProbabilityField = 0.1D;
            this.placingNewPiecesFrequencyField = ((uint)(1000));
            this.initialNumberOfPiecesField = ((uint)(4));
            this.boardWidthField = "5";
            this.taskAreaLengthField = "7";
            this.goalAreaLengthField = "3";
            this.numberOfPlayersPerTeamField = "4";

            this.goalsField = new GoalField[]
                {
                new GoalField() { x = 1, y = 1, type = GoalFieldType.goal, team = TeamColour.blue },
                new GoalField() { x = 2, y = 2, type = GoalFieldType.goal, team = TeamColour.blue },
                new GoalField() { x = 3, y = 0, type = GoalFieldType.goal, team = TeamColour.blue },
                new GoalField() { x = 2, y = 11, type = GoalFieldType.goal, team = TeamColour.red },
                new GoalField() { x = 3, y = 10, type = GoalFieldType.goal, team = TeamColour.red },
                new GoalField() { x = 4, y = 12, type = GoalFieldType.goal, team = TeamColour.red }

                };

            //// ORIGINALLY GENERATED PART:
            //this.goalsField = new GoalField[]
            //    {
            //        // set of GoalFields was invalid ...
            //        // corrected
            //    new GoalField() { x = 1, y = 1, type = GoalFieldType.goal, team = TeamColour.blue },
            //    new GoalField() { x = 1, y = 2, type = GoalFieldType.goal, team = TeamColour.blue },
            //    new GoalField() { x = 1, y = 0, type = GoalFieldType.goal, team = TeamColour.blue },
            //    new GoalField() { x = 1, y = 1, type = GoalFieldType.goal, team = TeamColour.red },
            //    new GoalField() { x = 11, y = 2, type = GoalFieldType.goal, team = TeamColour.red },
            //    new GoalField() { x = 11, y = 0, type = GoalFieldType.goal, team = TeamColour.red }

            //    };
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Goals")]
        public GoalField[] Goals {
            get {
                return this.goalsField;
            }
            set {
                this.goalsField = value;
            }
        }
        
        /// <remarks/>
        public double ShamProbability {
            get {
                return this.shamProbabilityField;
            }
            set {
                this.shamProbabilityField = value;
            }
        }
        
        /// <remarks/>
        public uint PlacingNewPiecesFrequency {
            get {
                return this.placingNewPiecesFrequencyField;
            }
            set {
                this.placingNewPiecesFrequencyField = value;
            }
        }
        
        /// <remarks/>
        public uint InitialNumberOfPieces {
            get {
                return this.initialNumberOfPiecesField;
            }
            set {
                this.initialNumberOfPiecesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="nonNegativeInteger")]
        public string BoardWidth {
            get {
                return this.boardWidthField;
            }
            set {
                this.boardWidthField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="nonNegativeInteger")]
        public string TaskAreaLength {
            get {
                return this.taskAreaLengthField;
            }
            set {
                this.taskAreaLengthField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="nonNegativeInteger")]
        public string GoalAreaLength {
            get {
                return this.goalAreaLengthField;
            }
            set {
                this.goalAreaLengthField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="nonNegativeInteger")]
        public string NumberOfPlayersPerTeam {
            get {
                return this.numberOfPlayersPerTeamField;
            }
            set {
                this.numberOfPlayersPerTeamField = value;
            }
        }
        
        /// <remarks/>
        public string GameName {
            get {
                return this.gameNameField;
            }
            set {
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable=true)]
    public abstract partial class Field : Location {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Field))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(GoalField))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable=true)]
    public partial class Location {
        
        private uint xField;
        
        private uint yField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint x {
            get {
                return this.xField;
            }
            set {
                this.xField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint y {
            get {
                return this.yField;
            }
            set {
                this.yField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    public partial class GameMasterSettingsActionCosts {
        
        private uint moveDelayField;
        
        private uint discoverDelayField;
        
        private uint testDelayField;
        
        private uint pickUpDelayField;
        
        private uint placingDelayField;
        
        private uint knowledgeExchangeDelayField;
        
        public GameMasterSettingsActionCosts() {
            this.moveDelayField = ((uint)(100));
            this.discoverDelayField = ((uint)(450));
            this.testDelayField = ((uint)(500));
            this.pickUpDelayField = ((uint)(100));
            this.placingDelayField = ((uint)(100));
            this.knowledgeExchangeDelayField = ((uint)(1200));
        }
        
        /// <remarks/>
        public uint MoveDelay {
            get {
                return this.moveDelayField;
            }
            set {
                this.moveDelayField = value;
            }
        }
        
        /// <remarks/>
        public uint DiscoverDelay {
            get {
                return this.discoverDelayField;
            }
            set {
                this.discoverDelayField = value;
            }
        }
        
        /// <remarks/>
        public uint TestDelay {
            get {
                return this.testDelayField;
            }
            set {
                this.testDelayField = value;
            }
        }
        
        /// <remarks/>
        public uint PickUpDelay {
            get {
                return this.pickUpDelayField;
            }
            set {
                this.pickUpDelayField = value;
            }
        }
        
        /// <remarks/>
        public uint PlacingDelay {
            get {
                return this.placingDelayField;
            }
            set {
                this.placingDelayField = value;
            }
        }
        
        /// <remarks/>
        public uint KnowledgeExchangeDelay {
            get {
                return this.knowledgeExchangeDelayField;
            }
            set {
                this.knowledgeExchangeDelayField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://se2.mini.pw.edu.pl/17-pl-19/17-pl-19/", IsNullable=false)]
    public partial class CommunicationServerSettings : Configuration {
    }
}
