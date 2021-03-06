﻿using System.Collections;
using System.Linq;
using Assets.Scripts.Views;
using Assets.UITest.Test_Runner_Scripts;
using CoreGame.Models;
using UnityEngine;

namespace Assets.UITest.AcceptanceTests {
    public class GameAcceptanceTest : Test_Runner_Scripts.UITest {
        [UISetUp]
        public IEnumerable SetUp() {
            PlayerPrefs.SetString("MapID", "Map3");
            PlayerPrefs.SetInt("NumPlayers", 1);
#if UNITY_EDITOR
            yield return LoadSceneByPath("Assets/Scenes/GameScene.unity");
#elif !UNITY_EDITOR
            yield return LoadScene("GameSelectScene");
#endif
        }

        [UITest]
        public IEnumerable TestAppearance() {
            // Active
            yield return WaitFor(new ObjectAppeared("Main Camera"));
            yield return WaitFor(new ObjectAppeared("GameView"));
            yield return WaitFor(new ObjectAppeared("GameHUD"));
            yield return WaitFor(new ObjectAppeared("TurnCountLabel"));
            yield return WaitFor(new ObjectAppeared("PigeonCountLabel"));
            yield return WaitFor(new ObjectAppeared("FireButton"));
            yield return WaitFor(new ObjectAppeared("WaterButton"));
            yield return WaitFor(new ObjectAppeared("UndoButton"));
            yield return WaitFor(new ObjectAppeared("EndTurnButton"));
            yield return WaitFor(new ObjectAppeared("OptionsButton"));
            yield return WaitFor(new ObjectAppeared("SideChosenText"));
            yield return WaitFor(new ObjectAppeared("EventSystem"));
            yield return WaitFor(new ObjectAppeared("InputImage"));
            yield return WaitFor(new ObjectAppeared("Pigeon(Clone)"));

            // Inactive
            yield return WaitFor(new ObjectDisappeared("WaitingPanel"));
            yield return WaitFor(new ObjectDisappeared("GameMenuCanvas"));
            yield return WaitFor(new ObjectDisappeared("HowToPlayCanvas"));
            yield return WaitFor(new ObjectDisappeared("ActionNotAllowedText"));
        }

        [UITest]
        public IEnumerable TestFireButton() {
            yield return WaitFor(new ObjectAppeared("FlammableGrassTile(Clone)"));
            TileView[] tiles = FindObjectsOfType<TileView>();
            TileView tile = tiles.FirstOrDefault(x => x.Type.Equals(TileType.Grass));
            GameInputView inputView = FindObjectOfType<GameInputView>();

            yield return Press("FireButton");
            inputView.HandleMapInput(tile);

            yield return WaitFor(new ObjectAppeared("RedTileSelectBorder(Clone)"));
        }

        [UITest]
        public IEnumerable TestWaterButton() {
            yield return WaitFor(new ObjectAppeared("FlammableGrassTile(Clone)"));
            TileView[] tiles = FindObjectsOfType<TileView>();
            TileView tile = tiles.FirstOrDefault(x => x.Type.Equals(TileType.Grass));
            GameInputView inputView = FindObjectOfType<GameInputView>();

            yield return Press("WaterButton");
            inputView.HandleMapInput(tile);

            yield return WaitFor(new ObjectAppeared("BlueTileSelectBorder(Clone)"));
        }

        [UITest]
        public IEnumerable TestAddToOccupiedTile() {
            yield return WaitFor(new ObjectAppeared("FlammableGrassTile(Clone)"));
            TileView[] tiles = FindObjectsOfType<TileView>();

            // Get occupied tile
            TileView tile = tiles.FirstOrDefault(x => x.IsOccupied());
            GameInputView inputView = FindObjectOfType<GameInputView>();

            // Try add fire to occupied tile -- not allowed, warning text should appear
            yield return Press("FireButton");
            inputView.HandleMapInput(tile);

            yield return WaitFor(new ObjectAppeared("ActionNotAllowedText"));
            yield return AssertLabel("ActionNotAllowedText", "Move not allowed! This tile is occupied.");
            yield return WaitFor(new ObjectDisappeared("ActionNotAllowedText"));

            // Try add water to occupied tile -- allowed; no warning text should appear
            yield return Press("WaterButton");
            inputView.HandleMapInput(tile);
            yield return WaitFor(new ObjectDisappeared("ActionNotAllowedText"));
            yield return WaitFor(new ObjectAppeared("BlueTileSelectBorder(Clone)"));
        }

        [UITest]
        public IEnumerable TestEndTurn() {
            yield return WaitFor(new ObjectAppeared("FlammableGrassTile(Clone)"));
            yield return AssertLabel("TurnCountLabel", "Turns Left: 10");

            // Just hit End Turn
            yield return Press("EndTurnButton");
            yield return AssertLabel("TurnCountLabel", "Turns Left: 9");
        }

        [UITest]
        public IEnumerable TestOptionsButton() {
            // More detailed testing in GameOptionsAcceptanceTest
            yield return Press("OptionsButton");
            yield return WaitFor(new ObjectDisappeared("GameHUD"));
            yield return WaitFor(new ObjectAppeared("GameMenuCanvas"));
        }
    }
}