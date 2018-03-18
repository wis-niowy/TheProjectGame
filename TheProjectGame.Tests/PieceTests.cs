using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using Messages;

namespace GameArea.Tests
{
    [TestClass]
    public class PieceTests
    {
        [TestMethod]
        public void NewPieceWithoutParams()
        {
            var piece = new Piece();
            Assert.AreEqual(PieceType.unknown, piece.type);
        }

        [TestMethod]
        public void PieceUnknown()
        {
            var piece = new Piece(PieceType.unknown);
            Assert.AreEqual(PieceType.unknown, piece.type);
        }
        [TestMethod]
        public void PieceValid()
        {
            var piece = new Piece(PieceType.normal);
            Assert.AreEqual(PieceType.normal, piece.type);
        }
        [TestMethod]
        public void PieceSham()
        {
            var piece = new Piece(PieceType.sham);
            Assert.AreEqual(PieceType.sham, piece.type);
        }

        [TestMethod]
        public void SetShamToValid()
        {
            var piece = new Piece(PieceType.sham);
            Assert.AreEqual(PieceType.sham, piece.type, "Invalid object initialization - piece is not Sham");
            piece.type = PieceType.normal;
            Assert.AreEqual(PieceType.normal, piece.type,"Piece.Type not changed to Valid");
        }

        [TestMethod]
        public void SetNewPieceToSham()
        {
            var piece = new Piece();
            piece.type = PieceType.sham;
            Assert.AreEqual(PieceType.sham, piece.type, "Piece created with default constructor did not changed type to Sham");
        }
    }
}
