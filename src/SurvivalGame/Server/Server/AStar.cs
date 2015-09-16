using Mentula.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mentula.Server
{
    public static class AStar
    {
        private static IntVector2 Default;

        static AStar()
        {
            Default = new IntVector2(int.MinValue);
        }

        public static IntVector2 Next4(ref Map map)
        {
            if (!map.Fitted) map.FitLength();
            List<IntVector2> open = new List<IntVector2>();
            List<IntVector2> closed = new List<IntVector2>();
            IntVector2 toCheck = map.End;
            open.Add(toCheck);

            bool endFound = false;
            while (true)
            {
                if (open.Contains(toCheck)) open.Remove(toCheck);
                closed.Add(toCheck);

                KeyValuePair<IntVector2, Node>[] ajNodes = GetAjasonNodes4(toCheck, ref map, ref closed);
                for (int i = 0; i < ajNodes.Length; i++)
                {
                    if (ajNodes[i].Key == map.Start)
                    {
                        endFound = true;
                        break;
                    }
                }

                for (int i = 0; i < ajNodes.Length; i++)
                {
                    Node cur = ajNodes[i].Value;
                    IntVector2 curKey = ajNodes[i].Key;

                    if (cur.Parent == Default) cur.SetParent(toCheck, 1);
                    if (map.GetNode(toCheck).G + 1 < cur.G) cur.SetParent(toCheck, 1);
                    if (!open.Contains(curKey)) open.Add(curKey);
                }

                if (open.Count == 0) break;
                toCheck = GetLowestF(ref open, ref map);
            }

            if (!endFound) return map.Start;
            return toCheck;
        }

        public static IntVector2 Next8(ref Map map)
        {
            if (!map.Fitted) map.FitLength();
            List<IntVector2> open = new List<IntVector2>();
            List<IntVector2> closed = new List<IntVector2>();
            IntVector2 toCheck = map.End;
            open.Add(toCheck);

            bool endFound = false;
            while (true)
            {
                if (open.Contains(toCheck)) open.Remove(toCheck);
                closed.Add(toCheck);

                KeyValuePair<IntVector2, Node>[] ajNodes = GetAjasonNodes4(toCheck, ref map, ref closed);
                for (int i = 0; i < ajNodes.Length; i++)
                {
                    if (ajNodes[i].Key == map.Start)
                    {
                        endFound = true;
                        break;
                    }
                }

                for (int i = 0; i < ajNodes.Length; i++)
                {
                    Node cur = ajNodes[i].Value;
                    IntVector2 curKey = ajNodes[i].Key;
                    float move = GetMovement(curKey, toCheck);

                    if (cur.Parent == Default) cur.SetParent(toCheck, move);
                    if (map.GetNode(toCheck).G + move < cur.G) cur.SetParent(toCheck, move);
                    if (!open.Contains(curKey)) open.Add(curKey);
                }

                if (open.Count == 0) break;
                toCheck = GetLowestF(ref open, ref map);
            }

            if (!endFound) return map.Start;
            return toCheck;
        }

        public static IntVector2[] Route4(ref Map map)
        {
            if (!map.Fitted) map.FitLength();
            List<IntVector2> open = new List<IntVector2>();
            List<IntVector2> closed = new List<IntVector2>();
            IntVector2 toCheck = map.Start;
            open.Add(toCheck);

            bool endFound = false;
            while (true)
            {
                if (open.Contains(toCheck)) open.Remove(toCheck);
                closed.Add(toCheck);

                KeyValuePair<IntVector2, Node>[] ajNodes = GetAjasonNodes4(toCheck, ref map, ref closed);
                for (int i = 0; i < ajNodes.Length; i++)
                {
                    if (ajNodes[i].Key == map.Start)
                    {
                        endFound = true;
                        break;
                    }
                }

                for (int i = 0; i < ajNodes.Length; i++)
                {
                    Node cur = ajNodes[i].Value;
                    IntVector2 curKey = ajNodes[i].Key;

                    if (cur.Parent == Default) cur.SetParent(toCheck, 1);
                    if (map.GetNode(toCheck).G + 1 < cur.G) cur.SetParent(toCheck, 1);
                    if (!open.Contains(curKey)) open.Add(curKey);
                }

                if (open.Count == 0) break;
                toCheck = GetLowestF(ref open, ref map);
            }

            if (!endFound) return new IntVector2[0];

            map.GetNode(map.End).SetParent(toCheck, 1);
            return Recall(ref map);
        }

        public static IntVector2[] Route8(ref Map map)
        {
            if (!map.Fitted) map.FitLength();
            List<IntVector2> open = new List<IntVector2>();
            List<IntVector2> closed = new List<IntVector2>();
            IntVector2 toCheck = map.Start;
            open.Add(toCheck);

            bool endFound = false;
            while (true)
            {
                if (open.Contains(toCheck)) open.Remove(toCheck);
                closed.Add(toCheck);

                KeyValuePair<IntVector2, Node>[] ajNodes = GetAjasonNodes4(toCheck, ref map, ref closed);
                for (int i = 0; i < ajNodes.Length; i++)
                {
                    if (ajNodes[i].Key == map.Start)
                    {
                        endFound = true;
                        break;
                    }
                }

                for (int i = 0; i < ajNodes.Length; i++)
                {
                    Node cur = ajNodes[i].Value;
                    IntVector2 curKey = ajNodes[i].Key;
                    float move = GetMovement(curKey, toCheck);

                    if (cur.Parent == Default) cur.SetParent(toCheck, move);
                    if (map.GetNode(toCheck).G + move < cur.G) cur.SetParent(toCheck, move);
                    if (!open.Contains(curKey)) open.Add(curKey);
                }

                if (open.Count == 0) break;
                toCheck = GetLowestF(ref open, ref map);
            }

            if (!endFound) return new IntVector2[0];

            map.GetNode(map.End).SetParent(toCheck, GetMovement(map.End, toCheck));
            return Recall(ref map);
        }

        private static IntVector2[] Recall(ref Map map)
        {
            List<IntVector2> result = new List<IntVector2>();
            IntVector2 curKey = map.End;
            Node curNode = map.GetNode(curKey);

            while (curNode.Parent != map.Start)
            {
                result.Add(curKey);
                curKey = curNode.Parent;
                curNode = map.GetNode(curKey);
            }

            return result.ToArray();
        }

        private static float GetMovement(IntVector2 cur, IntVector2 target)
        {
            float xD = Math.Abs(cur.X - target.X);
            float yD = Math.Abs(cur.Y - target.Y);

            if (xD > 0 && yD > 0) return 1.41421f;
            else return 1.0f;
        }

        private static IntVector2 GetLowestF(ref List<IntVector2> open, ref Map map)
        {
            IntVector2 minKey = open[0];
            float minF = map.GetNode(minKey).F;

            for (int i = 0; i < open.Count; i++)
            {
                IntVector2 curKey = open[i];

                if (minKey != curKey)
                {
                    float curF = map.GetNode(curKey).F;
                    if (curF < minF)
                    {
                        minKey = curKey;
                        minF = curF;
                    }
                }
            }

            return minKey;
        }

        private static KeyValuePair<IntVector2, Node>[] GetAjasonNodes4(IntVector2 pos, ref Map map, ref List<IntVector2> closed)
        {
            Dictionary<IntVector2, Node> result = new Dictionary<IntVector2, Node>();
            Node cur;
            IntVector2 toCheck;

            if (map.TryGetNode(toCheck = new IntVector2(pos.X - 1, pos.Y), out cur))
            {
                if (cur.WalkAble && !closed.Contains(toCheck)) result.Add(toCheck, cur);
            }
            if (map.TryGetNode(toCheck = new IntVector2(pos.X + 1, pos.Y), out cur))
            {
                if (cur.WalkAble && !closed.Contains(toCheck)) result.Add(toCheck, cur);
            }
            if (map.TryGetNode(toCheck = new IntVector2(pos.X, pos.Y - 1), out cur))
            {
                if (cur.WalkAble && !closed.Contains(toCheck)) result.Add(toCheck, cur);
            }
            if (map.TryGetNode(toCheck = new IntVector2(pos.X, pos.Y + 1), out cur))
            {
                if (cur.WalkAble && !closed.Contains(toCheck)) result.Add(toCheck, cur);
            }

            return result.ToArray();
        }

        private static KeyValuePair<IntVector2, Node>[] GetAjasonNodes8(IntVector2 pos, ref Map map, ref List<IntVector2> closed)
        {
            Dictionary<IntVector2, Node> result = new Dictionary<IntVector2, Node>();
            Node cur;
            IntVector2 toCheck;

            if (map.TryGetNode(toCheck = new IntVector2(pos.X - 1, pos.Y), out cur))
            {
                if (cur.WalkAble && !closed.Contains(toCheck)) result.Add(toCheck, cur);
            }
            if (map.TryGetNode(toCheck = new IntVector2(pos.X + 1, pos.Y), out cur))
            {
                if (cur.WalkAble && !closed.Contains(toCheck)) result.Add(toCheck, cur);
            }
            if (map.TryGetNode(toCheck = new IntVector2(pos.X, pos.Y - 1), out cur))
            {
                if (cur.WalkAble && !closed.Contains(toCheck)) result.Add(toCheck, cur);
            }
            if (map.TryGetNode(toCheck = new IntVector2(pos.X, pos.Y + 1), out cur))
            {
                if (cur.WalkAble && !closed.Contains(toCheck)) result.Add(toCheck, cur);
            }
            if (map.TryGetNode(toCheck = new IntVector2(pos.X - 1, pos.Y - 1), out cur))
            {
                if (cur.WalkAble && !closed.Contains(toCheck)) result.Add(toCheck, cur);
            }
            if (map.TryGetNode(toCheck = new IntVector2(pos.X - 1, pos.Y + 1), out cur))
            {
                if (cur.WalkAble && !closed.Contains(toCheck)) result.Add(toCheck, cur);
            }
            if (map.TryGetNode(toCheck = new IntVector2(pos.X + 1, pos.Y - 1), out cur))
            {
                if (cur.WalkAble && !closed.Contains(toCheck)) result.Add(toCheck, cur);
            }
            if (map.TryGetNode(toCheck = new IntVector2(pos.X + 1, pos.Y + 1), out cur))
            {
                if (cur.WalkAble && !closed.Contains(toCheck)) result.Add(toCheck, cur);
            }

            return result.ToArray();
        }

        public class Map
        {
            public IntVector2 Start;
            public IntVector2 End;
            public bool Fitted { get; private set; }

            private KeyValuePair<IntVector2, Node>[] _Nodes;
            private int _Index;

            public Map()
            {
                _Nodes = new KeyValuePair<IntVector2, Node>[0];
            }

            public Map(IntVector2 start, IntVector2 end)
            {
                _Nodes = new KeyValuePair<IntVector2, Node>[0];
                Start = start;
                End = end;
            }

            public Node GetNode(IntVector2 pos)
            {
                for (int i = 0; i < _Nodes.Length; i++)
                {
                    KeyValuePair<IntVector2, Node> cur = _Nodes[i];

                    if (cur.Key == pos) return cur.Value;
                }

                return null;
            }

            public bool TryGetNode(IntVector2 pos, out Node result)
            {
                for (int i = 0; i < _Nodes.Length; i++)
                {
                    KeyValuePair<IntVector2, Node> cur = _Nodes[i];

                    if (cur.Key == pos)
                    {
                        result = cur.Value;
                        return true;
                    }
                }

                result = null;
                return false;
            }

            public void SetLength(int length)
            {
                Array.Resize(ref _Nodes, length);
            }

            public void FitLength()
            {
                Fitted = true;
                int newLength = _Nodes.Length;

                for (int i = 0; i < _Nodes.Length; i++)
                {
                    if (_Nodes[i].Value == null)
                    {
                        newLength = i;
                        break;
                    }
                }

                Array.Resize(ref _Nodes, newLength);
            }

            public void AddNode(IntVector2 pos)
            {
                if (NodeExist(pos)) return;

                Fitted = false;
                if (_Index < _Nodes.Length) _Nodes[_Index++] = new KeyValuePair<IntVector2, Node>(pos, new Node());
                else
                {
                    Array.Resize(ref _Nodes, _Nodes.Length + 10);
                    _Nodes[_Index++] = new KeyValuePair<IntVector2, Node>(pos, new Node());
                }
            }

            public void AddNode(IntVector2 pos, float nodeCost, bool pathAble = true)
            {
                if (NodeExist(pos)) return;

                Fitted = false;
                if (_Index < _Nodes.Length) _Nodes[_Index++] = new KeyValuePair<IntVector2, Node>(pos, new Node(nodeCost, pathAble));
                else
                {
                    Array.Resize(ref _Nodes, _Nodes.Length + 10);
                    _Nodes[_Index++] = new KeyValuePair<IntVector2, Node>(pos, new Node(nodeCost, pathAble));
                }
            }

            private bool NodeExist(IntVector2 pos)
            {
                for (int i = 0; i < _Nodes.Length; i++)
                {
                    KeyValuePair<IntVector2, Node> cur = _Nodes[i];

                    if (cur.Key == pos) return true;
                }

                return false;
            }
        }

        public class Node
        {
            public IntVector2 Parent { get { return _Parent; } }
            public float F { get { return G + Heuristic; } }
            public float Heuristic;
            public float G;
            public bool WalkAble;

            private IntVector2 _Parent;

            public Node()
            {
                WalkAble = true;
                _Parent = Default;
            }

            public Node(float g, bool pathAble)
            {
                G = g;
                WalkAble = pathAble;
                _Parent = Default;
            }

            public void SetHeuristic(IntVector2 node, IntVector2 end)
            {
                Heuristic = IntVector2.Distance(node, end);
            }

            public void SetParent(IntVector2 parent, float cost)
            {
                _Parent = parent;
                G += cost;
            }
        }
    }
}