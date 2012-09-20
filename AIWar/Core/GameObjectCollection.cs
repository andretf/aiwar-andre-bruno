using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIWar.Core
{
    class GameObjectCollection
    {
        protected List<GameObject> Lista;

        public GameObjectCollection(){
            this.Lista.Add(new GameObject(1, "tokenBlackNeutron"));
            this.Lista.Add(new GameObject(2, "tokenBlackEletron"));
            this.Lista.Add(new GameObject(3, "tokenBlackPositron"));
            this.Lista.Add(new GameObject(4, "tokenWhiteNeutron"));
            this.Lista.Add(new GameObject(5, "tokenWhiteEletron"));
            this.Lista.Add(new GameObject(6, "tokenWhitePositron"));
        }

        public System.Drawing.Image GetImage(int id)
        {
            return (from img in Lista
                    where img.ID = id
                    select Properties.Resources.ResourceManager.GetObject(img.Image));
        }
    }
}
