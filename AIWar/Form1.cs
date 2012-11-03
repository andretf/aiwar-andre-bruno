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
		#region "Properties, Globals"
		[Obsolete]
		public int[,] GameMatrix = new int[21, 6] {
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
		public int[] TabuleiroVetor = new int[115] {
			  0, 0, 1, 0, 0, 
			0, 0, 3, 3, 0, 0,
			  0, 2, 2, 2, 0, 
			0, 0, 3, 3, 0, 0,
			  0, 0, 2, 0, 0, 
			0, 0, 0, 0, 0, 0,
			  0, 0, 0, 0, 0, 
			0, 0, 0, 0, 0, 0,
			  0, 0, 0, 0, 0, 
			0, 0, 0, 0, 0, 0,
			  0, 0, 0, 0, 0, 
			0, 0, 0, 0, 0, 0,
			  0, 0, 0, 0, 0, 
			0, 0, 0, 0, 0, 0,
			  0, 0, 0, 0, 0, 
			0, 0, 0, 0, 0, 0,
			  0, 0, 5, 0, 0, 
			0, 0, 6, 6, 0, 0,
			  0, 5, 5, 5, 0, 
			0, 0, 6, 6, 0, 0,
			  0, 0, 4, 0, 0
		};

		protected int ObjectsSize = 15;
		private GameObjectCollection Pecas;
        private int posicaoAtual = 0;
		#endregion

		#region "Initializers"
		public Form1()
		{
			InitializeComponent();
			initGame();
		}
		#endregion

		#region "Events"
        private void PecaClick(object sender, EventArgs e){
            PictureBox peca = (PictureBox)sender;
            //MessageBox.Show(peca.Name + " Click");

            // primeira limpa todas as pecas previamente marcadas
            if (posicaoAtual > 0) {
                PictureBox pecaAntiga = (PictureBox)this.Controls.Find("peca" + posicaoAtual.ToString(), true).First();
                if (pecaAntiga != null){
                    int posicaoAntiga = Convert.ToInt32(pecaAntiga.Name.Replace("peca", ""));
                    pecaAntiga.Image = GetImage(posicaoAntiga);
                }
            }

            int position = Convert.ToInt32(peca.Name.Replace("peca", ""));
            posicaoAtual = position;

            peca.Image = GetBlinkedImage(position);

            Core.Core core = new Core.Core();
            MessageBox.Show("getVisaoDiagonalMaior: " + ListaItens(core.getVisaoDiagonalMaior(TabuleiroVetor, position)));
            MessageBox.Show("getVisaoDiagonalMenor: " + ListaItens(core.getVisaoDiagonalMenor(TabuleiroVetor, position)));
            MessageBox.Show("getVisaoVertical: " + ListaItens(core.getVisaoVertical(TabuleiroVetor, position)));
            

        }

        private String ListaItens(int[] Tabuleiro)
        {
            String lista = "";

            foreach (int num in Tabuleiro)
            {
                lista += num.ToString() + " ";
            }

            return lista;
        }

        private void PecaHover(object sender, EventArgs e){
            PictureBox peca = (PictureBox)sender;
            peca.Cursor = Cursors.Hand;


        }
        private void PecaLeave(object sender, EventArgs e){
            PictureBox peca = (PictureBox)sender;
            peca.BackColor = TransparencyKey;
            peca.Cursor = Cursors.Default;
        }
        #endregion

		#region "Main Methods"
		protected bool initGame(){
			Pecas = new GameObjectCollection();

			if (desenhaPecas())
				return true;
			return false;
		}

		protected bool desenhaPecas() {
			for (int i = 0; i < 115; i++) {
				PictureBox e = new PictureBox();
                e.Name = "peca" + i.ToString();
				e.Parent = panel1;
				e.BackColor = System.Drawing.Color.Transparent;
				e.Margin = new Padding(0);
				e.TabIndex = 0;
				e.Size = new Size(15, 15);
				e.Location = new Point(Positions.VetorPeca[i, 0] + panel1.Left, Positions.VetorPeca[i, 1] + panel1.Top);

                // Eventos
                e.Click += new System.EventHandler(this.PecaClick);
                e.MouseHover += new System.EventHandler(this.PecaHover);
                //e.MouseLeave += new System.EventHandler(this.PecaLeave);
                //e.Mouse += new System.EventHandler(this.PecaHover);

                // Imagem da Peca
                e.Image = GetImage(i);

                this.panel1.Controls.Add(e);
            }
			return false;
		}

        protected Bitmap GetBlinkedImage(int i)
        {
            switch (TabuleiroVetor[i])
            {
                case 1: return Resources.tokenBlackNeutron1;
                case 2: return Resources.tokenBlackEletron1;
                case 3: return Resources.tokenBlackPositron1;
                case 4: return Resources.tokenWhiteNeutron1;
                case 5: return Resources.tokenWhiteEletron1;
                case 6: return Resources.tokenWhitePositron1;
            }

            return null;
        }
        protected Bitmap GetImage(int i)
        {
            switch (TabuleiroVetor[i])
            {
                case 1: return Resources.tokenBlackNeutron;
                case 2: return Resources.tokenBlackEletron;
                case 3: return Resources.tokenBlackPositron;
                case 4: return Resources.tokenWhiteNeutron;
                case 5: return Resources.tokenWhiteEletron;
                case 6: return Resources.tokenWhitePositron;
            }
            return null;
        }
		#endregion





        [Obsolete("Usar desenhaPecas()")]
		protected bool Draw() {
			for (int i = 0; i < 21; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					if (GameMatrix[i, j] >= 0) {
						int offsetLeft = 0;
						int offsetTop = 0;
						if (i % 2 == 0)
						{
							if (j == 5) break;
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
							offsetTop += (int)(i/2) * 36;
						}
						
						PictureBox e = new PictureBox();
						e.Parent = panel1;
						e.BackColor = System.Drawing.Color.Transparent;
						e.Margin = new Padding(0);
						e.TabIndex = 0;
						e.Size = new Size(15, 15);
						e.Location = new Point(offsetLeft + panel1.Left,
												offsetTop + panel1.Top);

						switch(GameMatrix[i, j]){
							case 1: e.Image = Resources.tokenBlackNeutron; break;
							case 2: e.Image = Resources.tokenBlackEletron; break;
							case 3: e.Image = Resources.tokenBlackPositron; break;
							case 4: e.Image = Resources.tokenWhiteNeutron; break;
							case 5: e.Image = Resources.tokenWhiteEletron; break;
							case 6: e.Image = Resources.tokenWhitePositron; break;
						}
						
						this.panel1.Controls.Add(e);
					}
					

				}
			}

			return false;
		}
	}
}
