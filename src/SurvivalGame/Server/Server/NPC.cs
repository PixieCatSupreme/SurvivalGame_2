using Mentula.Utilities;
using Microsoft.Xna.Framework;

namespace Mentula.Server
{
    public class NPC : Creature
    {
        private Vector2 targetArea;
        private double lastAttackTime;

        public void dostuff(float deltaTime, Creature[] players, int Index)
        {
            GetTarget(players, Index);
        }

        private bool GetTarget(Creature[] players, int Index)
        {
            bool t = false;
            for (int i = 0; i < Index; i++)
            {
                float d0 = Vector2.Distance(players[i].ChunkPos + players[i].Pos, base.ChunkPos + base.Pos);
                if (true)
                {

                }
            }
            return t;
        }

    }
}
