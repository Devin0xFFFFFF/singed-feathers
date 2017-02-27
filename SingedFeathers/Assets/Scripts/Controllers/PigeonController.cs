using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Controllers {
    public class PigeonController {
        private const int MAX_HEALTH = 100;
        private const int FIRE_DAMAGE = 10;

        private float _mapWidth;
        private float _mapHeight;

        public void SetDimensions(float width, float height) {
            _mapWidth = width;
            _mapHeight = height;
        }

        public Vector3 UpdatePosition(TileManager[,] map, ref Vector3 currPos) {
            TileManager currTile = map[(int) currPos.x, (int) currPos.y];
            List<Vector3> positions = GetNeighbouringPositions(map, currPos);

            if (currTile.IsOnFire()) {
                Vector3 move;

                foreach (Vector3 pos in positions) {
                    if (!map[(int) pos.x, (int) pos.y].IsOnFire()) {
                        move = pos - currPos;

                        move.x *= _mapWidth;
                        move.y *= _mapHeight;

                        currPos = pos;
                        return move;
                    }
                }
            }
            return new Vector3(0, 0, 0);
        }

        public int UpdateHealth(TileManager[,] map, Vector3 currPos, int health) {
            TileManager currTile = map[(int) currPos.x, (int) currPos.y];
            List<Vector3> positions = GetNeighbouringPositions(map, currPos);

            if (currTile.IsOnFire()) {
                health -= FIRE_DAMAGE * 2;
            }

            foreach (Vector3 pos in positions) {
                if (map[(int) pos.x, (int) pos.y].IsOnFire()) {
                    health -= FIRE_DAMAGE;
                }
            }
            return health;
        }

        private List<Vector3> GetNeighbouringPositions(TileManager[,] map, Vector3 pos) {
            List<Vector3> tiles = new List<Vector3>();

            if (pos.y + 1 < map.GetLength(1)) {
                tiles.Add(new Vector3(pos.x, pos.y + 1, 1));
            }
            if (pos.x + 1 < map.GetLength(0)) {
                tiles.Add(new Vector3(pos.x + 1, pos.y, 1));
            }
            if (pos.y - 1 >= 0) {
                tiles.Add(new Vector3(pos.x, pos.y - 1, 1));
            }
            if (pos.x - 1 >= 0) {
                tiles.Add(new Vector3(pos.x - 1, pos.y, 1));
            }
            return tiles;
        }
    }
}