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
            pictureBox1.Hide();
            for (int i = 0; i < 21; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (GameMatrix[i, j] >= 0) {
                        int offsetLeft = 0;
                        int offsetTop = 0;
                        if (i % 2 == 0)
                        {
                            offsetLeft += 28;               // Offset Fixo
                            offsetLeft += j * 56;           // Offset Dinamicamo: i * k
                            offsetTop += 0;                 // Offset Fixo
                            offsetTop += i/2 * 36;          // Offset Dinamicamo: i * k
                        }
                        else
                        {
                            offsetLeft += 0;
                            offsetLeft += j * 56;
                            offsetTop += 18;
                            offsetTop += (i-1) * 36;
                        }
                        
                        PictureBox e = new PictureBox();
                        e.Parent = pictureBox1;
                        e.BackColor = System.Drawing.Color.Transparent;
                        e.Margin = new Padding(0);
                        e.TabIndex = 0;
                        if (GameMatrix[i, j] == 0){
                            e.Size = new Size(40, 38);
                            e.Location = new Point(offsetLeft + pictureBox1.Left,
                                                    offsetTop + pictureBox1.Top);
                        }
                        else{
                            e.Size = new Size(15, 15);
                            e.Location = new Point(offsetLeft + pictureBox1.Left + 8,
                                                    offsetTop + pictureBox1.Top + 8);
                        }
                        switch(GameMatrix[i, j]){
                            case 1: e.Image = Resources.tokenBlackNeutron; break;
                            case 2: e.Image = Resources.tokenBlackEletron; break;
                            case 3: e.Image = Resources.tokenBlackPositron; break;
                            case 4: e.Image = Resources.tokenWhiteNeutron; break;
                            case 5: e.Image = Resources.tokenWhiteEletron; break;
                            case 6: e.Image = Resources.tokenWhitePositron; break;
                            default: e.Image = Resources.empty; break;
                        }
                        
                        
                        this.Controls.Add(e);
                    }
                    

                }
            }


            return false;
        }

    }
}
