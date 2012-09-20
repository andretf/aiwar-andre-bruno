using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIWar.Core
{
    class GameObject
    {
        private int id;
        private string name;

        public int ID {
            get {
                return id;
            }
        }

        public string Name
        {
            get {
                return name;
            }
        }

        public GameObject(int setId, string setName){
            this.id = setId;
            this.name = setName;
        }
    }
}
