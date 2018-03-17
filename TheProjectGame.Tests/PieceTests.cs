using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TheProjectGame.GameArea;

namespace TheProjectGame.Tests
{
    [TestClass]
    public class PieceTests
    {
        [TestMethod]
        public void NewPieceWithoutParams()
        {
            var piece = new Piece();
            Assert.AreEqual(GameArea.PieceType.Unknown, piece.Type);
        }

        [TestMethod]
        public void PieceUnknown()
        {
            var piece = new Piece(PieceType.Unknown);
            Assert.AreEqual(PieceType.Unknown, piece.Type);
        }
        [TestMethod]
        public void PieceValid()
        {
            var piece = new Piece(PieceType.Valid);
            Assert.AreEqual(PieceType.Valid, piece.Type);
        }
        [TestMethod]
        public void PieceSham()
        {
            var piece = new Piece(PieceType.Sham);
            Assert.AreEqual(PieceType.Sham, piece.Type);
        }

        [TestMethod]
        public void SetShamToValid()
        {
            var piece = new Piece(PieceType.Sham);
            Assert.AreEqual(PieceType.Sham, piece.Type, "Invalid object initialization - piece is not Sham");
            piece.Type = PieceType.Valid;
            Assert.AreEqual(PieceType.Valid, piece.Type,"Piece.Type not changed to Valid");
        }

        [TestMethod]
        public void SetNewPieceToSham()
        {
            var piece = new Piece();
            piece.Type = PieceType.Sham;
            Assert.AreEqual(PieceType.Sham, piece.Type, "Piece created with default constructor did not changed type to Sham");
        }
    }
}
