using System;
using System.Collections.Generic;


namespace Messages
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class Data : PlayerMessage
    {

        private TaskField[] taskFieldsField;

        private GoalField[] goalFieldsField;

        private Piece[] piecesField;

        private Location playerLocationField;

        private bool gameFinishedField;

        private string playerGuidField;
        public bool ShouldSerializePieces()
        {
            return true;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public TaskField[] TaskFields
        {
            get
            {
                return this.taskFieldsField;
            }
            set
            {
                this.taskFieldsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public GoalField[] GoalFields
        {
            get
            {
                return this.goalFieldsField;
            }
            set
            {
                this.goalFieldsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public Piece[] Pieces
        {
            get
            {
                return this.piecesField;
            }
            set
            {
                this.piecesField = value;
            }
        }

        /// <remarks/>
        public Location PlayerLocation
        {
            get
            {
                return this.playerLocationField;
            }
            set
            {
                this.playerLocationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool gameFinished
        {
            get
            {
                return this.gameFinishedField;
            }
            set
            {
                this.gameFinishedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string playerGuid
        {
            get
            {
                return this.playerGuidField;
            }
            set
            {
                this.playerGuidField = value;
            }
        }

        
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = true)]
    public partial class TaskField : Field
    {

        private int distanceToPieceField;

        private ulong pieceIdField;

        private bool pieceIdFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int distanceToPiece
        {
            get
            {
                return this.distanceToPieceField;
            }
            set
            {
                this.distanceToPieceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong pieceId
        {
            get
            {
                return this.pieceIdField;
            }
            set
            {
                this.pieceIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool pieceIdSpecified
        {
            get
            {
                return this.pieceIdFieldSpecified;
            }
            set
            {
                this.pieceIdFieldSpecified = value;
            }
        }

        public TaskField(uint x, uint y) : base(x, y) { }
        public TaskField() : base() { }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Field))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(GoalField))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TaskField))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = true)]
    public partial class Location
    {

        private uint xField;

        private uint yField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint x
        {
            get
            {
                return this.xField;
            }
            set
            {
                this.xField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint y
        {
            get
            {
                return this.yField;
            }
            set
            {
                this.yField = value;
            }
        }

        public Location(uint x, uint y)
        {
            xField = x;
            yField = y;
        }

        public Location() { }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(GoalField))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TaskField))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = true)]
    public abstract partial class Field : Location
    {

        private System.DateTime timestampField;

        private ulong playerIdField;

        private bool playerIdFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime timestamp
        {
            get
            {
                return this.timestampField;
            }
            set
            {
                this.timestampField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong playerId
        {
            get
            {
                return this.playerIdField;
            }
            set
            {
                this.playerIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool playerIdSpecified
        {
            get
            {
                return this.playerIdFieldSpecified;
            }
            set
            {
                this.playerIdFieldSpecified = value;
            }
        }

        public Field(uint x, uint y) : base(x, y) { }

        public override bool Equals(object obj)
        {
            var field = obj as Field;
            return field != null &&
                   base.Equals(obj) &&
                   timestampField == field.timestampField &&
                   playerIdField == field.playerIdField &&
                   playerIdFieldSpecified == field.playerIdFieldSpecified &&
                   timestamp == field.timestamp &&
                   playerId == field.playerId &&
                   playerIdSpecified == field.playerIdSpecified;
        }

        public Field() : base() { }
    }



    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = true)]
    public partial class GoalField : Field
    {

        private GoalFieldType typeField;

        private TeamColour teamField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public GoalFieldType type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public TeamColour team
        {
            get
            {
                return this.teamField;
            }
            set
            {
                this.teamField = value;
            }
        }

        public GoalField(uint x, uint y, TeamColour team, GoalFieldType type = GoalFieldType.goal) : base(x, y)
        {
            teamField = team;
            typeField = type;
        }

        public GoalField() : base(0, 0)
        {
        }
        public override bool Equals(object obj)
        {
            GoalField goalToCompare;
            if (obj.GetType() == typeof(GoalField))
                goalToCompare = (GoalField)obj;
            else
                return false;
            return goalToCompare.typeField == this.typeField
                && goalToCompare.teamField == this.teamField
                && goalToCompare.x == this.x
                && goalToCompare.y == this.y
                && base.Equals(goalToCompare);
        }

        public bool Equals(GoalField other)
        {
            return this.Equals(other);
        }

        public bool Equals(GoalField x, GoalField y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(GoalField obj)
        {
            return 0;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public enum GoalFieldType
    {

        /// <remarks/>
        unknown,

        /// <remarks/>
        goal,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("non-goal")]
        nongoal,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public enum TeamColour
    {

        /// <remarks/>
        red,

        /// <remarks/>
        blue,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class Piece
    {

        private ulong idField;

        private PieceType typeField;

        private System.DateTime timestampField;

        private ulong playerIdField;

        private bool playerIdFieldSpecified;

        public Piece(PieceType type, ulong id)
        {
            typeField = type;
            idField = id;
        }

        public Piece()
        {
            typeField = PieceType.unknown;
        }

        public Piece(Piece original)
        {
            idField = original.idField;
            typeField = original.typeField;
            timestampField = original.timestampField;
            playerIdSpecified = original.playerIdFieldSpecified;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public PieceType type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime timestamp
        {
            get
            {
                return this.timestampField;
            }
            set
            {
                this.timestampField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong playerId
        {
            get
            {
                return this.playerIdField;
            }
            set
            {
                this.playerIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool playerIdSpecified
        {
            get
            {
                return this.playerIdFieldSpecified;
            }
            set
            {
                this.playerIdFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public enum PieceType
    {

        /// <remarks/>
        unknown,

        /// <remarks/>
        sham,

        /// <remarks/>
        normal,

        /// <remarks/>
        destroyed,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = true)]
    public partial class Player
    {

        private TeamColour teamField;

        private PlayerRole roleField;

        private ulong idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public TeamColour team
        {
            get
            {
                return this.teamField;
            }
            set
            {
                this.teamField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public PlayerRole role
        {
            get
            {
                return this.roleField;
            }
            set
            {
                this.roleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public enum PlayerRole
    {

        /// <remarks/>
        leader,

        /// <remarks/>
        member,
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(BetweenPlayersMessage))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = true)]
    public partial class PlayerMessage
    {

        private ulong playerIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong playerId
        {
            get
            {
                return this.playerIdField;
            }
            set
            {
                this.playerIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class TestPiece : GameMessage
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = true)]
    public abstract partial class GameMessage
    {

        private string playerGuidField;

        private ulong gameIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string playerGuid
        {
            get
            {
                return this.playerGuidField;
            }
            set
            {
                this.playerGuidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong gameId
        {
            get
            {
                return this.gameIdField;
            }
            set
            {
                this.gameIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class DestroyPiece : GameMessage
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class PlacePiece : GameMessage
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class PickUpPiece : GameMessage
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class Move : GameMessage
    {

        private MoveType directionField;

        private bool directionFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public MoveType direction
        {
            get
            {
                return this.directionField;
            }
            set
            {
                this.directionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool directionSpecified
        {
            get
            {
                return this.directionFieldSpecified;
            }
            set
            {
                this.directionFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public enum MoveType
    {

        /// <remarks/>
        up,

        /// <remarks/>
        down,

        /// <remarks/>
        left,

        /// <remarks/>
        right,
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class Discover : GameMessage
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class AuthorizeKnowledgeExchange : GameMessage
    {

        private ulong withPlayerIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong withPlayerId
        {
            get
            {
                return this.withPlayerIdField;
            }
            set
            {
                this.withPlayerIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class KnowledgeExchangeRequest : BetweenPlayersMessage
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = true)]
    public abstract partial class BetweenPlayersMessage : PlayerMessage
    {

        private ulong senderPlayerIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong senderPlayerId
        {
            get
            {
                return this.senderPlayerIdField;
            }
            set
            {
                this.senderPlayerIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class AcceptExchangeRequest : BetweenPlayersMessage
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class RejectKnowledgeExchange : BetweenPlayersMessage
    {

        private bool permanentField;

        private string playerGuidField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool permanent
        {
            get
            {
                return this.permanentField;
            }
            set
            {
                this.permanentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string playerGuid
        {
            get
            {
                return this.playerGuidField;
            }
            set
            {
                this.playerGuidField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class SuggestAction : BetweenPlayersMessage
    {

        private TaskField[] taskFieldsField;

        private GoalField[] goalFieldsField;

        private string playerGuidField;

        private ulong gameIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public TaskField[] TaskFields
        {
            get
            {
                return this.taskFieldsField;
            }
            set
            {
                this.taskFieldsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public GoalField[] GoalFields
        {
            get
            {
                return this.goalFieldsField;
            }
            set
            {
                this.goalFieldsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string playerGuid
        {
            get
            {
                return this.playerGuidField;
            }
            set
            {
                this.playerGuidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong gameId
        {
            get
            {
                return this.gameIdField;
            }
            set
            {
                this.gameIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class SuggestActionResponse : BetweenPlayersMessage
    {

        private TaskField[] taskFieldsField;

        private GoalField[] goalFieldsField;

        private string playerGuidField;

        private ulong gameIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public TaskField[] TaskFields
        {
            get
            {
                return this.taskFieldsField;
            }
            set
            {
                this.taskFieldsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public GoalField[] GoalFields
        {
            get
            {
                return this.goalFieldsField;
            }
            set
            {
                this.goalFieldsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string playerGuid
        {
            get
            {
                return this.playerGuidField;
            }
            set
            {
                this.playerGuidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong gameId
        {
            get
            {
                return this.gameIdField;
            }
            set
            {
                this.gameIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class Game : PlayerMessage
    {

        private Player[] playersField;

        private GameBoard boardField;

        private Location playerLocationField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public Player[] Players
        {
            get
            {
                return this.playersField;
            }
            set
            {
                this.playersField = value;
            }
        }

        /// <remarks/>
        public GameBoard Board
        {
            get
            {
                return this.boardField;
            }
            set
            {
                this.boardField = value;
            }
        }

        /// <remarks/>
        public Location PlayerLocation
        {
            get
            {
                return this.playerLocationField;
            }
            set
            {
                this.playerLocationField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    public partial class GameBoard
    {

        private uint widthField;

        private uint tasksHeightField;

        private uint goalsHeightField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint tasksHeight
        {
            get
            {
                return this.tasksHeightField;
            }
            set
            {
                this.tasksHeightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint goalsHeight
        {
            get
            {
                return this.goalsHeightField;
            }
            set
            {
                this.goalsHeightField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class RegisterGame
    {

        private GameInfo newGameInfoField;

        /// <remarks/>
        public GameInfo NewGameInfo
        {
            get
            {
                return this.newGameInfoField;
            }
            set
            {
                this.newGameInfoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = true)]
    public partial class GameInfo
    {

        private string gameNameField;

        private ulong redTeamPlayersField;

        private ulong blueTeamPlayersField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string gameName
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong redTeamPlayers
        {
            get
            {
                return this.redTeamPlayersField;
            }
            set
            {
                this.redTeamPlayersField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong blueTeamPlayers
        {
            get
            {
                return this.blueTeamPlayersField;
            }
            set
            {
                this.blueTeamPlayersField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class ConfirmGameRegistration
    {

        private ulong gameIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong gameId
        {
            get
            {
                return this.gameIdField;
            }
            set
            {
                this.gameIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class RejectGameRegistration
    {

        private string gameNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string gameName
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

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class GameStarted
    {

        private ulong gameIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong gameId
        {
            get
            {
                return this.gameIdField;
            }
            set
            {
                this.gameIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class GetGames
    {
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class RegisteredGames
    {

        private GameInfo[] gameInfoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("GameInfo")]
        public GameInfo[] GameInfo
        {
            get
            {
                return this.gameInfoField;
            }
            set
            {
                this.gameInfoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class JoinGame
    {

        private string gameNameField;

        private TeamColour preferredTeamField;

        private PlayerRole preferredRoleField;

        private ulong playerIdField;

        private bool playerIdFieldSpecified;

        public JoinGame()
        {
            playerIdFieldSpecified = true; //haksior na dodawanie playerId do serialziacji ZAWSZE !!!
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string gameName
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public TeamColour preferredTeam
        {
            get
            {
                return this.preferredTeamField;
            }
            set
            {
                this.preferredTeamField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public PlayerRole preferredRole
        {
            get
            {
                return this.preferredRoleField;
            }
            set
            {
                this.preferredRoleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong playerId
        {
            get
            {
                return this.playerIdField;
            }
            set
            {
                this.playerIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool playerIdSpecified
        {
            get
            {
                return this.playerIdFieldSpecified;
            }
            set
            {
                this.playerIdFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class ConfirmJoiningGame : PlayerMessage
    {

        private Player playerDefinitionField;

        private ulong gameIdField;

        private string privateGuidField;

        /// <remarks/>
        public Player PlayerDefinition
        {
            get
            {
                return this.playerDefinitionField;
            }
            set
            {
                this.playerDefinitionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong gameId
        {
            get
            {
                return this.gameIdField;
            }
            set
            {
                this.gameIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string privateGuid
        {
            get
            {
                return this.privateGuidField;
            }
            set
            {
                this.privateGuidField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class RejectJoiningGame : PlayerMessage
    {

        private string gameNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string gameName
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

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class GameMasterDisconnectedMessage
    {

        private ulong gameIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong gameId
        {
            get
            {
                return this.gameIdField;
            }
            set
            {
                this.gameIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = false)]
    public partial class PlayerDisconnected
    {

        private ulong playerIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ulong playerId
        {
            get
            {
                return this.playerIdField;
            }
            set
            {
                this.playerIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://se2.mini.pw.edu.pl/17-results/", IsNullable = true)]
    public partial class ErrorMessage
    {

        private string exceptionTypeField;

        private string exceptionMessageField;

        private string exceptionCauseParameterNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
        public string ExceptionType
        {
            get
            {
                return this.exceptionTypeField;
            }
            set
            {
                this.exceptionTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ExceptionMessage
        {
            get
            {
                return this.exceptionMessageField;
            }
            set
            {
                this.exceptionMessageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
        public string ExceptionCauseParameterName
        {
            get
            {
                return this.exceptionCauseParameterNameField;
            }
            set
            {
                this.exceptionCauseParameterNameField = value;
            }
        }
    }
}
