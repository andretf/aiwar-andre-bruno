using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AIWar.Core;

namespace AIWar
{
    public partial class Form1 : Form
    {
        protected int[,] GameMatrix = new int[21, 6] {
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 }
        };
        protected PictureBox[,] GamePictureMatrix = new PictureBox[21, 6];
        protected int ObjectsSize = 15;
        protected int CelulaTabuleiroSize = 18;
        private GameObjectCollection Pecas;
        
        public Form1()
        {
            InitializeComponent();
            initGame();
        }

        protected bool initGame()
        {
            Pecas = new GameObjectCollection();

            if (Draw())
                return true;
            return false;
        }

        protected bool Draw() {
            pictureBox1.Image = Properties.Resources.tabuleiro;
            
            for (int i = 0; i < 21; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (GameMatrix[i, j] > 0) {
                        GamePictureMatrix[i, j] = new PictureBox()
                        {
                            BackColor = System.Drawing.Color.Transparent,
                            Size = new Size(ObjectsSize, ObjectsSize),
                            Location = new Point(CelulaTabuleiroSize * i, CelulaTabuleiroSize * j),
                            Image = Pecas.GetImage(GameMatrix[i, j])
                        };
                    }
                    

                }
            }


            return false;
        }

    }
}
