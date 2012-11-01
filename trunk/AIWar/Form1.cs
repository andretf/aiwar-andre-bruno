using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AIWar.Properties;
using AIWar.Core;

namespace AIWar
{
    public partial class Form1 : Form
    {
        protected int[,] GameMatrix = new int[21, 6] {
            {  0, 0, 1, 0, 0, 0 },
            {0, 0, 3, 3, 0, 0 },
            {  0, 2, 2, 2, 0, 0 },
            {0, 0, 3, 3, 0, 0 },
            {  0, 0, 2, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {  0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {  0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {  0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {  0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {  0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0 },
            {  0, 0, 0, 0, 0, 0 },
            {0, 0, 6, 6, 0, 0 },
            {  0, 5, 5, 5, 0, 0 },
            {0, 0, 6, 6, 0, 0 },
            {  0, 0, 4, 0, 0, 0 }
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
            //pictureBox1.Image = Properties.Resources.tabuleiro;
            //pictureBox1.SendToBack();
            pictureBox1.Hide();
            
            for (int i = 0; i < 21; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (GameMatrix[i, j] > 0) {
                        PictureBox e = new PictureBox();
                        e.BackColor = System.Drawing.Color.Transparent;
                        e.Location = new Point((int)(Math.Round(CelulaTabuleiroSize * j * 0.66)) + pictureBox1.Left,
                                                (int)(Math.Round(CelulaTabuleiroSize * i * 0.66)) + pictureBox1.Top);
                        e.Size = new Size(ObjectsSize, ObjectsSize);
                        e.BringToFront();

                        switch(GameMatrix[i, j]){
                            case 1: e.Image = Resources.tokenBlackNeutron; break;
                            case 2: e.Image = Resources.tokenBlackEletron; break;
                            case 3: e.Image = Resources.tokenBlackPositron; break;
                            case 4: e.Image = Resources.tokenWhiteNeutron; break;
                            case 5: e.Image = Resources.tokenWhiteEletron; break;
                            case 6: e.Image = Resources.tokenWhitePositron; break;
                        }
                        
                        
                        this.Controls.Add(e);
                    }
                    

                }
            }


            return false;
        }

    }
}
